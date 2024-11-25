using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stage.Data;
using Stage.Services;

namespace Stage.Services
{
    public class RappelMembreService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public RappelMembreService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ClubSportifDbContext>();
                    var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

                    // Récupérer les membres expirés
                    var membresExpirés = await dbContext.Membres
                        .Where(m => m.StatutAdhesion.ToLower() == "expire")
                        .ToListAsync();

                    foreach (var membre in membresExpirés)
                    {
                        try
                        {
                            var sujet = "Votre adhésion a expiré";
                            var corps = $@"
                                <p>Bonjour {membre.Nom},</p>
                                <p>Nous vous informons que votre adhésion au club est actuellement <strong>expirée</strong>.</p>
                                <p>Pour continuer à bénéficier de nos services, nous vous invitons à renouveler votre abonnement :</p>
                                <ul>
                                    <li><strong>Abonnement mensuel :</strong> 30$</li>
                                    <li><strong>Abonnement annuel :</strong> 300$</li>
                                </ul>
                                <p>Rendez-vous sur votre espace membre pour effectuer le renouvellement.</p>
                                <p>Cordialement,<br>ClubMaster</p>";

                            // Envoi de l'email
                            await emailService.SendEmailAsync(membre.Email, sujet, corps);
                            Console.WriteLine($"[SUCCESS] Email envoyé à {membre.Email}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[ERROR] Erreur lors de l'envoi de l'email à {membre.Email}: {ex.Message}");
                        }
                    }
                }

                // Attendre 24 heures avant la prochaine exécution
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}
