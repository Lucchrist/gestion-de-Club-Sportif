using Microsoft.AspNetCore.Mvc;
using Stage.Data;
using Stage.Models;
using Stage.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Stage.Controllers
{
    public class AdhesionsController : Controller
    {
        private readonly ClubSportifDbContext _context;
        private readonly StripeService _stripeService;

        public AdhesionsController(ClubSportifDbContext context, StripeService stripeService)
        {
            _context = context;
            _stripeService = stripeService;
        }

        // Affiche la liste des adhésions
        public IActionResult Index()
        {
            var adhesions = _context.Adhesions.Include(a => a.Membre).ToList();
            return View(adhesions);
        }

        // Charge le formulaire de création d'adhésion avec la liste des membres
        public IActionResult Create()
        {
            ViewData["MembreId"] = new SelectList(_context.Membres, "Id", "Nom"); // Récupère la liste des membres pour le dropdown
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Adhesion adhesion)
        {
            if (ModelState.IsValid)
            {
                adhesion.DateDebut = DateTime.Now;
                adhesion.DateFin = adhesion.TypeAbonnement == "Annuel" ? DateTime.Now.AddYears(1) : DateTime.Now.AddMonths(1);
                adhesion.Statut = "En attente";

                _context.Adhesions.Add(adhesion);
                _context.SaveChanges();

                // Créer un PaymentIntent pour Stripe
                var paymentIntent = _stripeService.CreatePaymentIntent(adhesion.Montant);

                // Passer les informations nécessaires à la vue de paiement
                ViewBag.ClientSecret = paymentIntent.ClientSecret;
                ViewBag.PublicKey = _stripeService.GetPublicKey(); // Assurez-vous que GetPublicKey() retourne la clé publique

                return View("Payment", adhesion); // Redirige vers la vue de paiement
            }

            // Si le modèle n'est pas valide, rechargez la liste des membres
            ViewData["MembreId"] = new SelectList(_context.Membres, "Id", "Nom", adhesion.MembreId);
            return View(adhesion);
        }
    }
}
