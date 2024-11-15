using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Stage.Services;

namespace Stage.Controllers
{
    public class EmailTestController : Controller
    {
        private readonly EmailService _emailService;

        public EmailTestController(EmailService emailService)
        {
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendTestEmail()
        {
            // Adresse e-mail de test
            string toEmail = "tchombachristian0@gmail.com"; // Remplacez par votre adresse pour recevoir l'email
            string subject = "Test d'envoi d'email";
            string message = "Ceci est un test pour vérifier si le service d'envoi d'email fonctionne.";

            try
            {
                await _emailService.SendEmailAsync(toEmail, subject, message);
                ViewBag.Message = "L'email de test a été envoyé avec succès.";
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Échec de l'envoi de l'email : {ex.Message}";
            }

            return View("Index"); // Retourne à la page d'accueil ou une vue de test
        }
    }
}
