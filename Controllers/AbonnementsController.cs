using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stage.Data;
using Stage.Models;

namespace Stage.Controllers
{
    public class AbonnementsController : Controller
    {
        private readonly ClubSportifDbContext _context;

        public AbonnementsController(ClubSportifDbContext context)
        {
            _context = context;
        }

        // Vue : Afficher tous les abonnements
        public async Task<IActionResult> Index()
        {
            var abonnements = await _context.Abonnements
                .Include(a => a.Membre)
                .ToListAsync();
            return View(abonnements);
        }

        // Vue : Détails d'un abonnement
        public async Task<IActionResult> Details(int id)
        {
            var abonnement = await _context.Abonnements
                .Include(a => a.Membre)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (abonnement == null)
            {
                return NotFound();
            }

            return View(abonnement);
        }

        // Vue : Ajouter un abonnement
        public IActionResult Create()
        {
            ViewData["Membres"] = _context.Membres.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MembreId,TypeAbonnement,Montant,DateFin,Commentaire")] Abonnement abonnement)
        {
            if (ModelState.IsValid)
            {
                abonnement.DateDebut = DateTime.Now;
                abonnement.Statut = "Actif";

                _context.Abonnements.Add(abonnement);

                // Mettre à jour le statut du membre
                var membre = await _context.Membres.FindAsync(abonnement.MembreId);
                if (membre != null)
                {
                    membre.StatutAdhesion = "Actif";
                    _context.Membres.Update(membre);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Membres"] = _context.Membres.ToList();
            return View(abonnement);
        }

        // Vue : Modifier un abonnement
        public async Task<IActionResult> Edit(int id)
        {
            var abonnement = await _context.Abonnements.FindAsync(id);
            if (abonnement == null)
            {
                return NotFound();
            }

            ViewData["Membres"] = _context.Membres.ToList();
            return View(abonnement);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MembreId,TypeAbonnement,Montant,Statut,DateFin,Commentaire")] Abonnement abonnement)
        {
            if (id != abonnement.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(abonnement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Abonnements.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["Membres"] = _context.Membres.ToList();
            return View(abonnement);
        }

        // Vue : Supprimer un abonnement
        public async Task<IActionResult> Delete(int id)
        {
            var abonnement = await _context.Abonnements
                .Include(a => a.Membre)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (abonnement == null)
            {
                return NotFound();
            }

            return View(abonnement);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var abonnement = await _context.Abonnements.FindAsync(id);
            if (abonnement != null)
            {
                _context.Abonnements.Remove(abonnement);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // API : Simuler un paiement
        [HttpPost("api/pay")]
          [Produces("application/json")]
        [Consumes("application/json")]
       
        public async Task<IActionResult> Pay([FromBody] dynamic data)
        {
            if (data == null || data.MembreId == null || data.Montant == null || data.TypeAbonnement == null)
            {
                return BadRequest(new { Message = "Données invalides pour le paiement." });
            }

            try
            {
                int membreId = (int)data.MembreId;
                decimal montant = (decimal)data.Montant;
                string typeAbonnement = (string)data.TypeAbonnement;

                var membre = await _context.Membres.FindAsync(membreId);
                if (membre == null)
                {
                    return NotFound(new { Message = "Membre introuvable." });
                }

                // Créer un nouvel abonnement
                var abonnement = new Abonnement
                {
                    MembreId = membreId,
                    TypeAbonnement = typeAbonnement,
                    Montant = montant,
                    Statut = "Actif",
                    DateDebut = DateTime.Now,
                    DateFin = typeAbonnement.ToLower() == "mensuel"
                        ? DateTime.Now.AddMonths(1)
                        : typeAbonnement.ToLower() == "annuel"
                            ? DateTime.Now.AddYears(1)
                            : null
                };

                _context.Abonnements.Add(abonnement);

                // Mettre à jour le statut du membre
                membre.StatutAdhesion = "Actif";
                _context.Membres.Update(membre);

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Paiement réussi. Abonnement ajouté.",
                    AbonnementId = abonnement.Id,
                    DateFin = abonnement.DateFin
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erreur lors du paiement.", Error = ex.Message });
            }
        }
    }
}
