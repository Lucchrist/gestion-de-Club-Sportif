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

        public async Task<IActionResult> EnvoyerRappel(int id)
        {
            // Récupérer l'entraînement et les membres inscrits
            var entrainement = await _context.Entrainements.Include(e => e.Participations)
                                                           .ThenInclude(p => p.Membre)
                                                           .FirstOrDefaultAsync(e => e.Id == id);
            if (entrainement == null)
            {
                return NotFound();
            }

            // Envoi des rappels aux membres inscrits
            foreach (var participation in entrainement.Participations)
            {
                var membre = participation.Membre;
                var subject = "Rappel d'entraînement";
                var message = $"Cher {membre.Nom}, n'oubliez pas l'entraînement {entrainement.Titre} prévu pour le {entrainement.DateDebut:dd/MM/yyyy} à {entrainement.HeureDebut}. Lieu : {entrainement.Lieu}.";

                await _emailService.SendEmailAsync(membre.Email, subject, message);
            }

            return RedirectToAction(nameof(Index));
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
