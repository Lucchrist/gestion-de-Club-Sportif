using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Stage.Services;

namespace Stage.Controllers
{
    public class EmailController : Controller
    {
        private readonly EmailService _emailService;

        public EmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("TestEmail")]
        public async Task<IActionResult> TestEmail()
        {
            try
            {
                // Exemple de destinataire et contenu
                string toEmail = "tchombachristian0@gmail.com";
                string subject = "Rappel d'entraînement : Football";
                string message = @"
                    <p>Bonjour,</p>
                    <p>Ceci est un rappel pour l'entraînement de football prévu demain :</p>
                    <ul>
                        <li><strong>Date :</strong> 25/11/2024</li>
                        <li><strong>Heure :</strong> 14:00</li>
                        <li><strong>Lieu :</strong> Stade Central</li>
                    </ul>
                    <p>Merci de confirmer votre présence.</p>";

                // Appel du service d'envoi d'email
                await _emailService.SendEmailAsync(toEmail, subject, message);

                // Confirmation de l'envoi
                return Ok("Email envoyé avec succès !");
            }
            catch (Exception ex)
            {
                // Gestion des erreurs
                return BadRequest($"Erreur lors de l'envoi de l'email : {ex.Message}");
            }
        }
    }
}
