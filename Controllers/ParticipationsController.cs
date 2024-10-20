using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stage.Data;
using Stage.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Stage.Controllers
{
    public class ParticipationsController : Controller
    {
        private readonly ClubSportifDbContext _context;

        public ParticipationsController(ClubSportifDbContext context)
        {
            _context = context;
        }

        // GET: Participations/Index
        public async Task<IActionResult> Index()
        {
            var participations = await _context.Participations
                                                .Include(p => p.Membre)
                                                .Include(p => p.Entrainement)
                                                .ToListAsync();
            return View(participations);
        }

        // GET: Participations/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var participation = await _context.Participations
                                              .Include(p => p.Membre)
                                              .Include(p => p.Entrainement)
                                              .FirstOrDefaultAsync(p => p.Id == id);

            if (participation == null)
            {
                return NotFound();
            }

            return View(participation);
        }

        // GET: Participations/Create
        public async Task<IActionResult> Create()
        {
            ViewData["Membres"] = await _context.Membres.ToListAsync();
            ViewData["Entrainements"] = await _context.Entrainements.ToListAsync();
            return View();
        }

        // POST: Participations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int membreId, int[] entrainementIds, string statut)
        {
            if (ModelState.IsValid)
            {
                foreach (var entrainementId in entrainementIds)
                {
                    var participation = new Participation
                    {
                        MembreId = membreId,
                        EntrainementId = entrainementId,
                        StatutParticipation = statut
                    };
                    _context.Add(participation);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Membres"] = await _context.Membres.ToListAsync();
            ViewData["Entrainements"] = await _context.Entrainements.ToListAsync();
            return View();
        }
        // GET: Participations/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var participation = await _context.Participations
                                              .Include(p => p.Membre)
                                              .FirstOrDefaultAsync(p => p.Id == id);

            if (participation == null)
            {
                return NotFound();
            }

            // Only passing the necessary data (Membre and StatutParticipation)
            ViewBag.MembreName = participation.Membre.Nom;

            return View(participation);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string statut)
        {
            // Vérification que la participation existe déjà
            var existingParticipation = await _context.Participations.FirstOrDefaultAsync(p => p.Id == id);
            if (existingParticipation == null)
            {
                return NotFound();
            }

            // Mise à jour du statut de participation
            if (ModelState.IsValid)
            {
                // Mettre à jour uniquement le statut de participation
                existingParticipation.StatutParticipation = statut;

                try
                {
                    // Sauvegarde des modifications dans la base de données
                    _context.Update(existingParticipation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ParticipationExists(existingParticipation.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // Redirection vers l'index après mise à jour réussie
                return RedirectToAction(nameof(Index));
            }

            // Si on arrive ici, il y a un problème avec la validation des données
            return View(existingParticipation);
        }

        private bool ParticipationExists(int id)
        {
            return _context.Participations.Any(e => e.Id == id);
        }



        // GET: Participations/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var participation = await _context.Participations
                                              .Include(p => p.Membre)
                                              .Include(p => p.Entrainement)
                                              .FirstOrDefaultAsync(p => p.Id == id);

            if (participation == null)
            {
                return NotFound();
            }

            return View(participation);
        }

        // POST: Participations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var participation = await _context.Participations.FindAsync(id);
            if (participation != null)
            {
                _context.Participations.Remove(participation);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        
    }
}
