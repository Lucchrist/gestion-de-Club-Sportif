using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PayPal.Api;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Stage.Models;
using Stage.Data;
using PayPal;

public class CotisationService
{
    private readonly IConfiguration _configuration;
    private readonly ClubSportifDbContext _context;

    public CotisationService(IConfiguration configuration, ClubSportifDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    // Envoyer un email via SMTP
    public void EnvoyerEmail(string destinataire, string sujet, string corps)
    {
        var smtpHost = _configuration["Smtp:Host"];
        var smtpPort = int.Parse(_configuration["Smtp:Port"]);
        var enableSsl = bool.Parse(_configuration["Smtp:EnableSsl"]);
        var userName = _configuration["Smtp:UserName"];
        var password = _configuration["Smtp:Password"];

        var mail = new MailMessage();
        mail.To.Add(destinataire);
        mail.From = new MailAddress(userName);
        mail.Subject = sujet;
        mail.Body = corps;

        using (var smtp = new SmtpClient(smtpHost, smtpPort))
        {
            smtp.Credentials = new NetworkCredential(userName, password);
            smtp.EnableSsl = enableSsl;
            smtp.Send(mail);
        }
    }

    // Envoyer un email de confirmation de paiement
    public void EnvoyerEmailConfirmationPaiement(Membre membre, decimal montant)
    {
        var sujet = "Confirmation de votre paiement";
        var corps = $"Bonjour {membre.Nom},\n\nNous confirmons la réception de votre paiement de {montant:C}. Merci de renouveler votre cotisation à temps.";
        EnvoyerEmail(membre.Email, sujet, corps);
    }

    // Envoyer un email de rappel de paiement
    public void EnvoyerEmailRappel(Membre membre, DateTime dateExpiration)
    {
        var sujet = "Renouvellement de votre cotisation";
        var corps = $"Bonjour {membre.Nom},\n\nVotre cotisation a expiré le {dateExpiration.ToString("yyyy-MM-dd")}. Veuillez renouveler votre abonnement pour continuer à participer aux activités.";
        EnvoyerEmail(membre.Email, sujet, corps);
    }

    // Effectuer un paiement via PayPal
    public string EffectuerPaiementPayPal(decimal montant, string typeAbonnement)
    {
        try
        {
            var config = new Dictionary<string, string>
            {
                { "clientId", _configuration["PayPal:ClientId"] },
                { "clientSecret", _configuration["PayPal:ClientSecret"] },
                { "mode", _configuration["PayPal:Mode"] }
            };

            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            var apiContext = new APIContext(accessToken);

            var payment = new Payment
            {
                intent = "sale",
                payer = new Payer { payment_method = "paypal" },
                transactions = new List<Transaction>
                {
                    new Transaction
                    {
                        description = $"Paiement pour {typeAbonnement}",
                        invoice_number = Guid.NewGuid().ToString(),
                        amount = new Amount
                        {
                            currency = "USD",
                            total = montant.ToString("F2")
                        }
                    }
                },
                redirect_urls = new RedirectUrls
                {
                    cancel_url = "https://votre-site/cancel",
                    return_url = "https://votre-site/success"
                }
            };

            var createdPayment = payment.Create(apiContext);
            var approvalUrl = createdPayment.links.FirstOrDefault(l => l.rel.Equals("approval_url", StringComparison.OrdinalIgnoreCase))?.href;

            return approvalUrl;
        }
        catch (PayPalException ex)
        {
            // Log de l'erreur ou traitement de l'erreur
            throw new Exception("Une erreur est survenue lors du paiement avec PayPal.", ex);
        }
    }

    // Vérifier les cotisations expirées et envoyer des rappels
    public async Task VerifierCotisationsExpirees()
    {
        var cotisationsExpirees = await _context.Cotisations
                                    .Include(c => c.Membre)
                                    .Where(c => c.EstExpiree)
                                    .ToListAsync();

        foreach (var cotisation in cotisationsExpirees)
        {
            EnvoyerEmailRappel(cotisation.Membre, cotisation.DateExpiration);
        }
    }
}
