using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stage.Data;
using Stage.Models;
using Stage.Services;

namespace Stage.Controllers
{
    public class AbonnementsController : Controller
    {
        private readonly ClubSportifDbContext _context;
        private readonly AbonnementService _abonnementService;

        public AbonnementsController(ClubSportifDbContext context, AbonnementService abonnementService)
        {
            _context = context;
            _abonnementService = abonnementService;
        }

        // GET: Abonnements/Create
        public IActionResult Create()
        {
            ViewData["MembreId"] = new SelectList(_context.Membres, "Id", "Email");
            return View();
        }

        // POST: Abonnements/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Abonnement abonnement)
        {
            if (ModelState.IsValid)
            {
                // Redirige vers PayPal pour le paiement
                var approvalUrl = _abonnementService.EffectuerPaiementPayPal(abonnement.Montant, abonnement.TypeAbonnement);
                TempData["Abonnement"] = abonnement;
                return Redirect(approvalUrl);
            }
            ViewData["MembreId"] = new SelectList(_context.Membres, "Id", "Email", abonnement.MembreId);
            return View(abonnement);
        }

        // Action de retour après le paiement PayPal
        public async Task<IActionResult> CreateConfirmation(string paymentId, string token, string PayerID)
        {
            // Récupération de l'abonnement depuis TempData
            var abonnement = TempData["Abonnement"] as Abonnement;
            if (abonnement == null) return RedirectToAction("Create");

            // Vérifier l'état du paiement
            var payment = _abonnementService.ValiderPaiementPayPal(paymentId, PayerID);
            if (payment.state== "approved")
            {
                // Enregistrer l’abonnement
                abonnement.Statut = "Payé";
                _context.Add(abonnement);
                await _context.SaveChangesAsync();

                // Envoyer l'email de confirmation
                var membre = await _context.Membres.FindAsync(abonnement.MembreId);
                if (membre != null)
                {
                    _abonnementService.EnvoyerEmailConfirmationPaiement(membre, abonnement.Montant);
                }

                TempData["Message"] = "Abonnement créé avec succès.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Error"] = "Le paiement a échoué. Veuillez réessayer.";
                return RedirectToAction(nameof(Create));
            }
        }

        // GET: Abonnements
        public async Task<IActionResult> Index()
        {
            var abonnements = await _context.Abonnements.Include(c => c.Membre).ToListAsync();
            return View(abonnements);
        }

        // GET: Abonnements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue) return NotFound();

            var abonnement = await _context.Abonnements
                .Include(c => c.Membre)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (abonnement == null) return NotFound();

            return View(abonnement);
        }

        // GET: Abonnements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue) return NotFound();

            var abonnement = await _context.Abonnements.FindAsync(id);
            if (abonnement == null) return NotFound();

            ViewData["MembreId"] = new SelectList(_context.Membres, "Id", "Email", abonnement.MembreId);
            return View(abonnement);
        }

        // POST: Abonnements/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Abonnement abonnement)
        {
            if (id != abonnement.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(abonnement);
                    await _context.SaveChangesAsync();

                    // Envoi de l'email de confirmation de paiement si le statut est "Payé"
                    var membre = await _context.Membres.FindAsync(abonnement.MembreId);
                    if (membre != null && abonnement.Statut == "Payé")
                    {
                        _abonnementService.EnvoyerEmailConfirmationPaiement(membre, abonnement.Montant);
                    }

                    TempData["Message"] = "Abonnement mis à jour avec succès.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AbonnementExists(abonnement.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["MembreId"] = new SelectList(_context.Membres, "Id", "Email", abonnement.MembreId);
            return View(abonnement);
        }

        private bool AbonnementExists(int id)
        {
            return _context.Abonnements.Any(e => e.Id == id);
        }
    }
}
