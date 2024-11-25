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
    public class EmailTestBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EmailTestBackgroundService> _logger;

        public EmailTestBackgroundService(IServiceProvider serviceProvider, ILogger<EmailTestBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ClubSportifDbContext>();
                    var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

                    var participations = await dbContext.Participations
                        .Include(p => p.Membre)
                        .Include(p => p.Entrainement)
                        .Where(p => p.Entrainement.DateDebut >= DateTime.Now) // Entraînements à venir
                        .ToListAsync();

                    foreach (var participation in participations)
                    {
                        try
                        {
                            var membre = participation.Membre;
                            var entrainement = participation.Entrainement;

                            var sujet = "Rappel : Votre entraînement approche";
                            var corps = $@"
                                <p>Bonjour {membre.Nom},</p>
                                <p>Nous vous rappelons que vous avez un entraînement prévu :</p>
                                <ul>
                                    <li><strong>Date :</strong> {entrainement.DateDebut:dd/MM/yyyy}</li>
                                    <li><strong>Heure :</strong> {entrainement.DateDebut:HH:mm}</li>
                                    <li><strong>Lieu :</strong> {entrainement.Lieu}</li>
                                </ul>
                                <p>Merci de confirmer votre participation si ce n'est pas déjà fait.</p>
                                <p>Cordialement,<br>ClubMaster</p>";

                            await emailService.SendEmailAsync(membre.Email, sujet, corps);
                            _logger.LogInformation($"[SUCCESS] Email envoyé à {membre.Email} pour l'entraînement {entrainement.Titre}.");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"[ERROR] Erreur lors de l'envoi de l'email de rappel.");
                        }
                    }
                }

                // Attendre 10 secondes avant la prochaine exécution
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
