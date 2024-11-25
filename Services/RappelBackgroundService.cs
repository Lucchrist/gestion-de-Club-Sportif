using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Stage.Controllers;

namespace Stage.Services
{
    public class RappelBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RappelBackgroundService> _logger;

        public RappelBackgroundService(IServiceProvider serviceProvider, ILogger<RappelBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RappelBackgroundService démarré à {time}", DateTime.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    try
                    {
                        // Obtenir le contrôleur des e-mails
                        var emailController = scope.ServiceProvider.GetRequiredService<EmailController>();

                        _logger.LogInformation("Tentative d'envoi de rappels à {time}", DateTime.Now);

                        // Appeler la méthode d'envoi de mails
                        var result = await emailController.TestEmail();

                        if (result is OkObjectResult)
                        {
                            _logger.LogInformation("Emails envoyés avec succès à {time}", DateTime.Now);
                        }
                        else if (result is BadRequestObjectResult badResult)
                        {
                            _logger.LogWarning("Erreur lors de l'envoi d'emails : {message} à {time}", badResult.Value, DateTime.Now);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Une erreur s'est produite lors de l'envoi des rappels à {time}", DateTime.Now);
                    }
                }

                // Attendre 10 secondes avant la prochaine exécution
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogWarning("RappelBackgroundService interrompu à {time}", DateTime.Now);
                }
            }

            _logger.LogInformation("RappelBackgroundService arrêté à {time}", DateTime.Now);
        }
    }
}
