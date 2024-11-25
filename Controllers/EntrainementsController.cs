using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stage.Data;
using Stage.Models;
using Stage.Services;

namespace Stage.Controllers
{
    public class EntrainementsController : Controller
    {
        private readonly ClubSportifDbContext _context;
        private readonly EmailService _emailService; // Service email injecté

        public EntrainementsController(ClubSportifDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }


        // Méthode pour envoyer des rappels 24h avant un entraînement
        public async Task EnvoyerRappels24h()
        {
            var demain = DateTime.Now.AddDays(1).Date;

            // Récupérer les entraînements programmés pour dans 24 heures
            var entrainements = await _context.Entrainements
                .Include(e => e.Participations)
                .ThenInclude(p => p.Membre)
                .Where(e => e.DateDebut.Date == demain)
                .ToListAsync();

            if (!entrainements.Any())
            {
                // Aucun entraînement prévu pour demain
                Console.WriteLine("Aucun entraînement n'est prévu dans les prochaines 24 heures.");
                return;
            }

            foreach (var entrainement in entrainements)
            {
                foreach (var participation in entrainement.Participations)
                {
                    var membre = participation.Membre;

                    if (membre == null || string.IsNullOrEmpty(membre.Email))
                    {
                        Console.WriteLine($"Le membre {participation.MembreId} n'a pas d'adresse email valide.");
                        continue;
                    }

                    // Préparation de l'email
                    var sujet = $"Rappel d'entraînement : {entrainement.Titre}";
                    var corps = $@"
                        <p>Bonjour {membre.Nom},</p>
                        <p>Nous vous rappelons que vous avez un entraînement prévu dans 24 heures :</p>
                        <ul>
                            <li><strong>Type d'entraînement :</strong> {entrainement.TypeEvenement}</li>
                            <li><strong>Titre :</strong> {entrainement.Titre}</li>
                            <li><strong>Description :</strong> {entrainement.Description}</li>
                            <li><strong>Date :</strong> {entrainement.DateDebut:dd/MM/yyyy}</li>
                            <li><strong>Heure :</strong> {entrainement.HeureDebut:hh\\:mm}</li>
                            <li><strong>Lieu :</strong> {entrainement.Lieu}</li>
                        </ul>
                        <p>Merci de confirmer votre présence et d'être ponctuel.</p>
                        <p>Cordialement,<br>ClubMaster</p>";

                    try
                    {
                        await _emailService.SendEmailAsync(membre.Email, sujet, corps);
                        Console.WriteLine($"Email envoyé avec succès à {membre.Email}.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erreur lors de l'envoi de l'email à {membre.Email} : {ex.Message}");
                    }
                }
            }
        }

        // Méthode pour exécuter EnvoyerRappels24h au démarrage
        public static async Task InitEmailRappels(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ClubSportifDbContext>();
                var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

                var controller = new EntrainementsController(context, emailService);
                await controller.EnvoyerRappels24h();
            }
        }

        // Méthode pour envoyer un rappel pour un entraînement spécifique
        public async Task<IActionResult> EnvoyerRappel(int id)
        {
            var entrainement = await _context.Entrainements
                .Include(e => e.Participations)
                .ThenInclude(p => p.Membre)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entrainement == null)
            {
                return NotFound();
            }

            foreach (var participation in entrainement.Participations)
            {
                var membre = participation.Membre;

                if (membre == null || string.IsNullOrEmpty(membre.Email))
                {
                    continue;
                }

                var sujet = $"Rappel d'entraînement : {entrainement.Titre}";
                var corps = $@"
                    Bonjour {membre.Nom},
                    Ceci est un rappel concernant l'entraînement prévu le {entrainement.DateDebut:dd/MM/yyyy} à {entrainement.HeureDebut:hh\\:mm} au {entrainement.Lieu}.
                ";

                try
                {
                    await _emailService.SendEmailAsync(membre.Email, sujet, corps);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de l'envoi de l'email à {membre.Email} : {ex.Message}");
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeclencherRappels()
        {
            await EnvoyerRappels24h(); // Appelle la méthode pour envoyer les rappels
            TempData["Message"] = "Les rappels ont été envoyés avec succès !";
            return RedirectToAction("Index"); // Redirige vers la page principale ou une autre vue
        }








        // GET: Entrainements
        public async Task<IActionResult> Index(int? year, int? month)
        {
            int selectedYear = year ?? DateTime.Now.Year;
            int selectedMonth = month ?? DateTime.Now.Month;

            // Ajuste l'année si le mois dépasse les limites
            if (selectedMonth < 1)
            {
                selectedMonth = 12;
                selectedYear--;
            }
            else if (selectedMonth > 12)
            {
                selectedMonth = 1;
                selectedYear++;
            }

            ViewBag.SelectedYear = selectedYear;
            ViewBag.SelectedMonth = selectedMonth;

            var entrainements = await _context.Entrainements
                .Where(e => e.DateDebut.Year == selectedYear && e.DateDebut.Month == selectedMonth)
                .ToListAsync();

            return View(entrainements);
        }




        [HttpPost]
        public async Task<IActionResult> AddEvent(Entrainement newEvent)
        {
            if (ModelState.IsValid)
            {
                _context.Entrainements.Add(newEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction("Calendar");
            }

            return View("Calendar");
        }



        // GET: Entrainements/Calendar
        public async Task<IActionResult> Calendar(int? year, int? month)
        {
            int selectedYear = year ?? DateTime.Now.Year;
            int selectedMonth = month ?? DateTime.Now.Month;

            // Ajuste l'année si le mois dépasse les limites
            if (selectedMonth < 1)
            {
                selectedMonth = 12;
                selectedYear--;
            }
            else if (selectedMonth > 12)
            {
                selectedMonth = 1;
                selectedYear++;
            }

            ViewBag.SelectedYear = selectedYear;
            ViewBag.SelectedMonth = selectedMonth;

            var entrainements = await _context.Entrainements
                .Where(e => e.DateDebut.Year == selectedYear && e.DateDebut.Month == selectedMonth)
                .ToListAsync();

            return View(entrainements);
        }






        // GET: Entrainements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entrainement = await _context.Entrainements
                .FirstOrDefaultAsync(m => m.Id == id);
            if (entrainement == null)
            {
                return NotFound();
            }

            return View(entrainement);
        }

        // GET: Entrainements/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Entrainements/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titre,Description,DateDebut,DateFin,HeureDebut,HeureFin,Lieu,TypeEvenement")] Entrainement entrainement)
        {
            if (ModelState.IsValid)
            {
                _context.Add(entrainement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(entrainement);
        }

        // GET: Entrainements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entrainement = await _context.Entrainements.FindAsync(id);
            if (entrainement == null)
            {
                return NotFound();
            }
            return View(entrainement);
        }

        // POST: Entrainements/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titre,Description,DateDebut,DateFin,HeureDebut,HeureFin,Lieu,TypeEvenement")] Entrainement entrainement)
        {
            if (id != entrainement.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(entrainement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntrainementExists(entrainement.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(entrainement);
        }

        // GET: Entrainements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entrainement = await _context.Entrainements
                .FirstOrDefaultAsync(m => m.Id == id);
            if (entrainement == null)
            {
                return NotFound();
            }

            return View(entrainement);
        }

        // POST: Entrainements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entrainement = await _context.Entrainements.FindAsync(id);
            if (entrainement != null)
            {
                _context.Entrainements.Remove(entrainement);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EntrainementExists(int id)
        {
            return _context.Entrainements.Any(e => e.Id == id);
        }
    }
}
