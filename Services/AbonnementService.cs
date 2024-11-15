using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PayPal.Api;
using Microsoft.EntityFrameworkCore;
using Stage.Models;
using Stage.Data;
using PayPal;

namespace Stage.Services
{
    public class AbonnementService
    {
        private readonly IConfiguration _configuration;
        private readonly ClubSportifDbContext _context;

        public AbonnementService(IConfiguration configuration, ClubSportifDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        // Méthode pour envoyer un email via SMTP
        private void EnvoyerEmail(string destinataire, string sujet, string corps)
        {
            var smtpHost = _configuration["Smtp:Host"];
            var smtpPort = int.Parse(_configuration["Smtp:Port"]);
            var enableSsl = bool.Parse(_configuration["Smtp:EnableSsl"]);
            var userName = _configuration["Smtp:UserName"];
            var password = _configuration["Smtp:Password"];

            using (var mail = new MailMessage())
            using (var smtp = new SmtpClient(smtpHost, smtpPort))
            {
                mail.To.Add(destinataire);
                mail.From = new MailAddress(userName);
                mail.Subject = sujet;
                mail.Body = corps;
                smtp.Credentials = new NetworkCredential(userName, password);
                smtp.EnableSsl = enableSsl;
                smtp.Send(mail);
            }
        }

        // Envoyer un email de confirmation de paiement
        public void EnvoyerEmailConfirmationPaiement(Membre membre, decimal montant)
        {
            var sujet = "Confirmation de votre paiement";
            var corps = $"Bonjour {membre.Nom},\n\nNous confirmons la réception de votre paiement de {montant:C}. Merci pour votre renouvellement d'abonnement.";
            EnvoyerEmail(membre.Email, sujet, corps);
        }

        // Envoyer un email de rappel de paiement
        public void EnvoyerEmailRappel(Membre membre, DateTime dateExpiration)
        {
            var sujet = "Renouvellement de votre abonnement";
            var corps = $"Bonjour {membre.Nom},\n\nVotre abonnement a expiré le {dateExpiration:yyyy-MM-dd}. Veuillez renouveler votre abonnement pour continuer à participer aux activités.";
            EnvoyerEmail(membre.Email, sujet, corps);
        }

        // Initialiser un paiement via PayPal et obtenir le lien d'approbation
        public string EffectuerPaiementPayPal(decimal montant, string typeAbonnement)
        {
            try
            {
                var clientId = _configuration["PayPal:ClientId"];
                var clientSecret = _configuration["PayPal:ClientSecret"];
                var mode = _configuration["PayPal:Mode"];

                var config = new Dictionary<string, string>
                {
                    { "clientId", clientId },
                    { "clientSecret", clientSecret },
                    { "mode", mode }
                };

                var accessToken = new OAuthTokenCredential(clientId, clientSecret).GetAccessToken();
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
                        cancel_url = "https://localhost:7058/Participations",
                        return_url = "https://localhost:7058/Membres"
                    }
                };

                var createdPayment = payment.Create(apiContext);
                var approvalUrl = createdPayment.links.FirstOrDefault(l => l.rel == "approval_url")?.href;

                return approvalUrl;
            }
            catch (PayPalException ex)
            {
                throw new Exception("Une erreur est survenue lors du paiement avec PayPal.", ex);
            }
        }

        // Confirmer le paiement PayPal
        public Payment ValiderPaiementPayPal(string paymentId, string payerId)
        {
            var apiContext = GetApiContext();
            var payment = new Payment() { id = paymentId };

            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            return payment.Execute(apiContext, paymentExecution);
        }

        // Créer une nouvelle cotisation pour un membre
        public async Task<Abonnement> CreerAbonnement(int membreId, string typeAbonnement, decimal montant)
        {
            var abonnement = new Abonnement
            {
                MembreId = membreId,
                TypeAbonnement = typeAbonnement,
                Montant = montant,
                Statut = "En attente",
                DateDebut = DateTime.Now,
                DateFin = CalculerDateFin(typeAbonnement)
            };

            _context.Abonnements.Add(abonnement);
            await _context.SaveChangesAsync();

            return abonnement;
        }

        // Méthode pour calculer la date de fin en fonction du type d'abonnement
        private DateTime CalculerDateFin(string typeAbonnement)
        {
            return typeAbonnement switch
            {
                "Mensuel" => DateTime.Now.AddMonths(1),
                "Hebdomadaire" => DateTime.Now.AddDays(7),
                "Annuel" => DateTime.Now.AddYears(1),
                _ => throw new ArgumentException("Type d'abonnement non valide")
            };
        }

        private APIContext GetApiContext()
        {
            var clientId = _configuration["PayPal:ClientId"];
            var clientSecret = _configuration["PayPal:ClientSecret"];
            var config = new Dictionary<string, string>
            {
                { "clientId", clientId },
                { "clientSecret", clientSecret },
                { "mode", _configuration["PayPal:Mode"] }
            };

            var accessToken = new OAuthTokenCredential(clientId, clientSecret).GetAccessToken();
            return new APIContext(accessToken);
        }
    }
}
