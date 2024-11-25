using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stage.Data;
using Stage.Models;
using Stage.Services;

namespace Stage.Controllers
{
    public class MembresController : Controller
    {
        private readonly ClubSportifDbContext _context;
        private readonly EmailService _emailService;

        public MembresController(ClubSportifDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: Membres
        public async Task<IActionResult> Index()
        {
            return View(await _context.Membres.ToListAsync());
        }

        // Méthode pour vérifier le statut et envoyer des emails
        [HttpPost]
        public async Task<IActionResult> VerifierEtEnvoyerEmails()
        {
            var membresExpirés = await _context.Membres
                .Where(m => m.StatutAdhesion.ToLower() == "expire") // Vérifie le statut
                .ToListAsync();

            int emailsEnvoyes = 0;
            int emailsEchoues = 0;

            foreach (var membre in membresExpirés)
            {
                var sujet = "Votre adhésion a expiré";
                var corps = $@"
             <p>Bonjour {membre.Nom},</p>
                        <p>Nous vous informons que votre adhésion au club est actuellement <strong>expirée</strong>.</p>
                        <p>Pour continuer à bénéficier de nos services, nous vous invitons à renouveler votre abonnement :</p>
                        <ul>
                            <li><strong>Abonnement mensuel :</strong> 30$</li>
                            <li><strong>Abonnement annuel :</strong> 300$</li>
                        </ul>
                        <p>Rendez-vous sur votre espace membre pour effectuer le renouvellement.</p>
                        <p>Cordialement,<br>ClubMaster</p>";

                try
                {
                    await _emailService.SendEmailAsync(membre.Email, sujet, corps);
                    emailsEnvoyes++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de l'envoi de l'email à {membre.Email} : {ex.Message}");
                    emailsEchoues++;
                }
            }

            TempData["Message"] = $"Emails envoyés : {emailsEnvoyes}, Échecs : {emailsEchoues}";
            return RedirectToAction(nameof(Index));
        }


        // GET: Membres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membre = await _context.Membres.FirstOrDefaultAsync(m => m.Id == id);
            if (membre == null)
            {
                return NotFound();
            }

            return View(membre);
        }

        // GET: Membres/Create
        public IActionResult Create()
        {
            ViewBag.StatutAdhesionList = new SelectList(new List<string> { "Actif", "Expire" });
            return View();
        }

        // POST: Membres/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nom,Email,Telephone,DateAdhesion,StatutAdhesion")] Membre membre)
        {
            if (ModelState.IsValid)
            {
                _context.Add(membre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.StatutAdhesionList = new SelectList(new List<string> { "Actif", "Expire" });
            return View(membre);
        }

        // GET: Membres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membre = await _context.Membres.FindAsync(id);
            if (membre == null)
            {
                return NotFound();
            }

            ViewBag.StatutAdhesionList = new SelectList(new List<string> { "Actif", "Expire" }, membre.StatutAdhesion);
            return View(membre);
        }

        // POST: Membres/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nom,Email,Telephone,DateAdhesion,StatutAdhesion")] Membre membre)
        {
            if (id != membre.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(membre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MembreExists(membre.Id))
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

            ViewBag.StatutAdhesionList = new SelectList(new List<string> { "Actif", "Expire" }, membre.StatutAdhesion);
            return View(membre);
        }

        
       
        // GET: Membres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membre = await _context.Membres
                .FirstOrDefaultAsync(m => m.Id == id);
            if (membre == null)
            {
                return NotFound();
            }

            return View(membre);
        }

        // POST: Membres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var membre = await _context.Membres.FindAsync(id);
            if (membre != null)
            {
                _context.Membres.Remove(membre);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Vérifie si un membre existe
        private bool MembreExists(int id)
        {
            return _context.Membres.Any(e => e.Id == id);
        }
    }
}
