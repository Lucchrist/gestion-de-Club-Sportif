using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stage.Data;
using Stage.Models;

namespace Stage.Controllers
{
    public class ParticipationsController : Controller
    {
        private readonly ClubSportifDbContext _context;
        private readonly ILogger<ParticipationsController> _logger;

        public ParticipationsController(ClubSportifDbContext context, ILogger<ParticipationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Participations/Index
        public async Task<IActionResult> Index()
        {
            // Supprimer les participations des membres expirés
            await SupprimerParticipationsMembresExpirés();

            // Charger les participations valides
            var participations = await _context.Participations
                                                .Include(p => p.Membre)
                                                .Include(p => p.Entrainement)
                                                .ToListAsync();

            return View(participations);
        }

        // Méthode privée pour supprimer les participations des membres expirés
        private async Task SupprimerParticipationsMembresExpirés()
        {
            var participationsExpirées = await _context.Participations
                .Include(p => p.Membre)
                .Where(p => p.Membre.StatutAdhesion.ToLower() == "expire")
                .ToListAsync();

            if (participationsExpirées.Any())
            {
                _context.Participations.RemoveRange(participationsExpirées);
                await _context.SaveChangesAsync();
            }
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
            // Vérifier si le membre est expiré
            var membre = await _context.Membres.FindAsync(membreId);
            if (membre == null || membre.StatutAdhesion.ToLower() == "expire")
            {
                TempData["Message"] = "Impossible d'ajouter des participations pour un membre avec un statut 'Expire'.";
                return RedirectToAction(nameof(Index));
            }

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

            ViewBag.MembreName = participation.Membre.Nom;
            return View(participation);
        }

        // POST: Participations/ModifierStatut
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModifierStatut(int participationId, string statutParticipation)
        {
            var participation = await _context.Participations.FindAsync(participationId);
            if (participation != null)
            {
                participation.StatutParticipation = statutParticipation;
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", new { id = participationId });
            }

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


        [HttpPost("api/participations/add")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> ApiAddParticipations([FromBody] JsonElement data)
        {
            try
            {
                // Vérification des propriétés dans le JsonElement
                if (!data.TryGetProperty("MembreId", out var membreIdElement) || membreIdElement.ValueKind != JsonValueKind.Number ||
                    !data.TryGetProperty("EntrainementIds", out var entrainementIdsElement) || entrainementIdsElement.ValueKind != JsonValueKind.Array)
                {
                    return BadRequest(new { Message = "Données invalides pour l'ajout de participations. Vérifiez MembreId et EntrainementIds." });
                }

                // Extraction des valeurs
                int membreId = membreIdElement.GetInt32();
                var entrainementIds = entrainementIdsElement.EnumerateArray()
                                                            .Where(e => e.ValueKind == JsonValueKind.Number)
                                                            .Select(e => e.GetInt32())
                                                            .ToList();

                if (entrainementIds.Count == 0)
                {
                    return BadRequest(new { Message = "La liste des EntrainementIds est vide ou invalide." });
                }

                // Validation du membre
                var membre = await _context.Membres.FindAsync(membreId);
                if (membre == null || membre.StatutAdhesion.ToLower() == "expire")
                {
                    return BadRequest(new { Message = "Le membre est introuvable ou son statut est expiré." });
                }

                // Création des participations
                foreach (var entrainementId in entrainementIds)
                {
                    var participation = new Participation
                    {
                        MembreId = membreId,
                        EntrainementId = entrainementId,
                        StatutParticipation = "Actif"
                    };
                    _context.Participations.Add(participation);
                }

                await _context.SaveChangesAsync();
                return Ok(new { Message = "Participations ajoutées avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erreur interne du serveur.", Error = ex.Message });
            }
        }



        [HttpDelete("api/participations/{id}/unregister")]
        [Produces("application/json")]
        public async Task<IActionResult> ApiUnregisterParticipation(int id)
        {
            var participation = await _context.Participations.FindAsync(id);

            if (participation == null)
            {
                return NotFound(new { Message = "Participation introuvable." });
            }

            try
            {
                _context.Participations.Remove(participation);
                await _context.SaveChangesAsync();
                return Ok(new { Message = "Participation supprimée avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erreur lors de la suppression de la participation.", Error = ex.Message });
            }
        }


    }
}
