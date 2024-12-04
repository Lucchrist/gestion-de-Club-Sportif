using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Stage.Data;
using Stage.Services;

namespace Stage.Services
{
    public class RappelMembreService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RappelMembreService> _logger;

        public RappelMembreService(IServiceProvider serviceProvider, ILogger<RappelMembreService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<ClubSportifDbContext>();
                        var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

                        // Vérifiez si la base de données est accessible
                        if (!await dbContext.Database.CanConnectAsync(stoppingToken))
                        {
                            _logger.LogWarning("La base de données n'est pas accessible. Réessai dans 10 minutes.");
                            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                            continue;
                        }

                        // Récupérer les membres expirés
                        var membresExpirés = await dbContext.Membres
                            .Where(m => m.StatutAdhesion.ToLower() == "expire")
                            .ToListAsync(stoppingToken);

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
                                _logger.LogInformation($"Email envoyé avec succès à {membre.Email}");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Erreur lors de l'envoi de l'email à {membre.Email}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Une erreur s'est produite dans le service RappelMembreService.");
                }

                // Attendre 24 heures avant la prochaine exécution
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}
