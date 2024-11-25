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
    public class RappelParticipationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RappelParticipationService> _logger;

        public RappelParticipationService(IServiceProvider serviceProvider, ILogger<RappelParticipationService> logger)
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
                        .Where(p => p.Entrainement.DateDebut > DateTime.Now) // Entraînements à venir
                        .ToListAsync();

                    foreach (var participation in participations)
                    {
                        try
                        {
                            var membre = participation.Membre;
                            var entrainement = participation.Entrainement;

                            // Calcul du temps restant avant l'entraînement
                            var tempsRestant = entrainement.DateDebut - DateTime.Now;

                            // Envoyer le rappel à 72h, 48h, 24h et 0h
                            if (tempsRestant.TotalHours <= 72 && tempsRestant.TotalHours > 48 ||
                                tempsRestant.TotalHours <= 48 && tempsRestant.TotalHours > 24 ||
                                tempsRestant.TotalHours <= 24 && tempsRestant.TotalHours > 0 ||
                                tempsRestant.TotalHours <= 0 && tempsRestant.TotalMinutes > -10) // 10 minutes après le début
                            {
                                var sujet = "Rappel : Votre entraînement approche";
                                var corps = $@"
                                    <p>Bonjour {membre.Nom},</p>
                                    <p>Voici les détails de votre entraînement :</p>
                                    <ul>
                                        <li><strong>Titre :</strong> {entrainement.Titre}</li>
                                        <li><strong>Description :</strong> {entrainement.Description}</li>
                                        <li><strong>Date :</strong> {entrainement.DateDebut:dd/MM/yyyy}</li>
                                        <li><strong>Heure :</strong> {entrainement.DateDebut:HH:mm}</li>
                                        <li><strong>Lieu :</strong> {entrainement.Lieu}</li>
                                    </ul>
                                    <p>Merci de confirmer votre participation si ce n'est pas déjà fait.</p>
                                    <p>Cordialement,<br>ClubMaster</p>";

                                await emailService.SendEmailAsync(membre.Email, sujet, corps);
                                _logger.LogInformation($"[SUCCESS] Email envoyé à {membre.Email} pour l'entraînement {entrainement.Titre}.");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"[ERROR] Erreur lors de l'envoi de l'email de rappel.");
                        }
                    }
                }

                // Attendre 1 heure avant la prochaine vérification
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
