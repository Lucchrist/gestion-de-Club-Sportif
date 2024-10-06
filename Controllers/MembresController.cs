using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stage.Data;
using Stage.Models;

namespace Stage.Controllers
{
    public class MembresController : Controller
    {
        private readonly ClubSportifDbContext _context;

        public MembresController(ClubSportifDbContext context)
        {
            _context = context;
        }

        // GET: Membres
        public async Task<IActionResult> Index()
        {
            return View(await _context.Membres.ToListAsync());
        }

        // GET: Membres/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Membres/Create
        public IActionResult Create()
        {
            // Créer une liste déroulante pour le statut d'adhésion
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
            // Si la validation échoue, on renvoie la liste déroulante avec les options
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

            // Liste déroulante pour le statut d'adhésion
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

            // Si la validation échoue, on renvoie la liste déroulante avec la sélection actuelle
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

        private bool MembreExists(int id)
        {
            return _context.Membres.Any(e => e.Id == id);
        }
    }
}
