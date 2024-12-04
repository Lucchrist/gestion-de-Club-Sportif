using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stage.Data;
using Stage.Models;
using Stage.Services;

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
    public async Task<IActionResult> Index(string searchTerm, string statutFilter)
    {
        var membres = _context.Membres.AsQueryable();

        // Filtrer par statut
        if (!string.IsNullOrEmpty(statutFilter))
        {
            membres = membres.Where(m => m.StatutAdhesion == statutFilter);
        }

        // Recherche
        if (!string.IsNullOrEmpty(searchTerm))
        {
            membres = membres.Where(m =>
                m.Nom.Contains(searchTerm) ||
                m.Email.Contains(searchTerm) ||
                m.Telephone.Contains(searchTerm));
        }

        ViewBag.StatutFilter = statutFilter;
        ViewBag.SearchTerm = searchTerm;


        
            
          return View(await membres.ToListAsync());
    }
        // GET: Membres/Details/5
        public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var membre = await _context.Membres.FirstOrDefaultAsync(m => m.Id == id);
        if (membre == null) return NotFound();

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
            membre.DateAdhesion = DateTime.Now;
            _context.Add(membre);
            await _context.SaveChangesAsync();

            // Envoyer un email de bienvenue
            var sujet = "Bienvenue dans le club sportif";
            var corps = $@"
    <p>Bonjour {membre.Nom},</p>
    <p>Nous sommes ravis de vous compter parmi nous au sein du club sportif.</p>
    <p>Voici vos informations d'adhésion :</p>
    <ul>
        <li><strong>Nom :</strong> {membre.Nom}</li>
        <li><strong>Email :</strong> {membre.Email}</li>
        <li><strong>Date d'adhésion :</strong> {membre.DateAdhesion:dd/MM/yyyy}</li>
    </ul>
    <p>En tant que nouveau membre, vous bénéficiez de <strong>10 jours gratuits</strong> pour profiter de toutes nos activités : entraînements, compétitions, jeux, et bien d'autres.</p>
    <p>Après cette période, pensez à vous abonner pour continuer à profiter de nos services :</p>
    <ul>
        <li><strong>30 $</strong> par mois</li>
        <li><strong>300 $</strong> par an</li>
    </ul>
    <p>Si vous ne vous abonnez pas, votre statut passera automatiquement à <strong>Expire</strong>, ce qui limitera votre accès aux activités.</p>
    <p>Nous restons à votre disposition pour toute question ou assistance concernant l'abonnement.</p>
    <p>Cordialement,<br>L'équipe du club sportif</p>";


            try
            {
                await _emailService.SendEmailAsync(membre.Email, sujet, corps);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Erreur lors de l'envoi de l'email : {ex.Message}");
            }

            return RedirectToAction(nameof(Index));
        }

        ViewBag.StatutAdhesionList = new SelectList(new List<string> { "Actif", "Expire" });
        return View(membre);
    }

    // GET: Membres/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var membre = await _context.Membres.FindAsync(id);
        if (membre == null) return NotFound();

        ViewBag.StatutAdhesionList = new SelectList(new List<string> { "Actif", "Expire" }, membre.StatutAdhesion);
        return View(membre);
    }

    // POST: Membres/Edit/5
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
                var membreDb = await _context.Membres.FindAsync(id);
                if (membreDb == null)
                {
                    return NotFound();
                }

                // Vérifiez si le statut a changé
                bool statutChanged = membreDb.StatutAdhesion != membre.StatutAdhesion;

                // Mettre à jour les champs
                membreDb.Nom = membre.Nom;
                membreDb.Email = membre.Email;
                membreDb.Telephone = membre.Telephone;
                membreDb.StatutAdhesion = membre.StatutAdhesion;

                _context.Update(membreDb);
                await _context.SaveChangesAsync();

                // Si le statut change et devient "Expire", envoyez un email immédiatement
                if (statutChanged && membre.StatutAdhesion == "Expire")
                {
                    try
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

                        await _emailService.SendEmailAsync(membre.Email, sujet, corps);
                        TempData["SuccessMessage"] = "Email envoyé au membre pour le statut expiré.";
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = $"Erreur lors de l'envoi de l'email : {ex.Message}";
                    }
                }

                return RedirectToAction(nameof(Index));
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
        }

        ViewBag.StatutAdhesionList = new SelectList(new List<string> { "Actif", "Expire" }, membre.StatutAdhesion);
        return View(membre);
    }


    // GET: Membres/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var membre = await _context.Membres.FirstOrDefaultAsync(m => m.Id == id);
        if (membre == null) return NotFound();

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
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("api/members/create")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> ApiCreateMember([FromBody] Membre membre)
    {
        if (membre == null || string.IsNullOrWhiteSpace(membre.Nom) || string.IsNullOrWhiteSpace(membre.Email))
        {
            return BadRequest(new { Message = "Données invalides pour le membre." });
        }

        membre.StatutAdhesion = "Actif";
        membre.DateAdhesion = DateTime.Now;

        try
        {
            // Ajouter le membre dans la base de données
            _context.Add(membre);
            await _context.SaveChangesAsync();

            // Envoyer un email de bienvenue
            var sujet = "Bienvenue dans le club sportif";
            var corps = $@"
            <p>Bonjour {membre.Nom},</p>
            <p>Nous sommes ravis de vous compter parmi nous au sein du club sportif.</p>
            <p>Voici vos informations d'adhésion :</p>
            <ul>
                <li><strong>Nom :</strong> {membre.Nom}</li>
                <li><strong>Email :</strong> {membre.Email}</li>
                <li><strong>Date d'adhésion :</strong> {membre.DateAdhesion:dd/MM/yyyy}</li>
            </ul>
            <p>Vous avez 10 jours gratuits pour profiter des entraînements, compétitions, jeux et bien d'autres.</p>
            <p>Après cette période, pensez à vous abonner :</p>
            <ul>
                <li>30$ mensuel</li>
                <li>300$ par année</li>
            </ul>
            <p>Si vous ne vous abonnez pas, votre statut sera expiré.</p>
            <p>Cordialement,<br>L'équipe du club sportif</p>";

            try
            {
                await _emailService.SendEmailAsync(membre.Email, sujet, corps);
            }
            catch (Exception ex)
            {
                // Gérer les erreurs d'envoi d'email
                return StatusCode(500, new { Message = "Membre créé avec succès, mais l'email n'a pas pu être envoyé.", Error = ex.Message });
            }

            return Ok(new { Message = "Membre créé avec succès. Email envoyé." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Erreur lors de la création du membre.", Error = ex.Message });
        }
    }



    // API: Récupérer les détails d'un membre
    [HttpGet("api/members/{id}")]
    [Produces("application/json")]
    public async Task<IActionResult> ApiGetMemberDetails(int id)
    {
        var membre = await _context.Membres.FindAsync(id);

        if (membre == null)
        {
            return NotFound(new { Message = "Membre introuvable." });
        }

        return Ok(new
        {
            membre.Id,
            membre.Nom,
            membre.Email,
            membre.Telephone,
            membre.StatutAdhesion,
            membre.DateAdhesion
        });
    }

    // API: Modifier un membre
    [HttpPut("api/members/{id}/update")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> ApiUpdateMember(int id, [FromBody] Membre updatedMembre)
    {
        var membre = await _context.Membres.FindAsync(id);

        if (membre == null)
        {
            return NotFound(new { Message = "Membre introuvable." });
        }

        if (!string.IsNullOrWhiteSpace(updatedMembre.Nom)) membre.Nom = updatedMembre.Nom;
        if (!string.IsNullOrWhiteSpace(updatedMembre.Email)) membre.Email = updatedMembre.Email;
        if (!string.IsNullOrWhiteSpace(updatedMembre.Telephone)) membre.Telephone = updatedMembre.Telephone;

        try
        {
            _context.Update(membre);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Membre mis à jour avec succès.", MemberId = membre.Id });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Erreur lors de la mise à jour du membre.", Error = ex.Message });
        }
    }

    // Vérifie si un membre existe
    private bool MembreExists(int id)
    {
        return _context.Membres.Any(e => e.Id == id);
    }
}
