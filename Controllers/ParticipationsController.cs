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

        // POST: Participations/ModifierStatut
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModifierStatut(int participationId, string StatutParticipation)
        {
            var participation = await _context.Participations.FindAsync(participationId);
            if (participation != null)
            {
                participation.StatutParticipation = StatutParticipation;
                await _context.SaveChangesAsync();

                // Rediriger vers la page des détails après modification
                return RedirectToAction("Details", new { id = participationId });
            }

            // Si la participation n'est pas trouvée, retourner une erreur
            return NotFound();
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

        private bool ParticipationExists(int id)
        {
            return _context.Participations.Any(e => e.Id == id);
        }
    }
}
