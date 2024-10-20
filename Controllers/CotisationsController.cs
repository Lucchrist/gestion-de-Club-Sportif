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
    public class CotisationsController : Controller
    {
        private readonly ClubSportifDbContext _context;

        public CotisationsController(ClubSportifDbContext context)
        {
            _context = context;
        }

        // GET: Cotisations
        public async Task<IActionResult> Index()
        {
            var clubSportifDbContext = _context.Cotisations.Include(c => c.Membre);
            return View(await clubSportifDbContext.ToListAsync());
        }

        // GET: Cotisations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cotisation = await _context.Cotisations
                .Include(c => c.Membre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cotisation == null)
            {
                return NotFound();
            }

            return View(cotisation);
        }

        // GET: Cotisations/Create
        public IActionResult Create()
        {
            ViewData["MembreId"] = new SelectList(_context.Membres, "Id", "Email");
            return View();
        }

        // POST: Cotisations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MembreId,Montant,Type,DatePaiement,DateExpiration")] Cotisation cotisation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cotisation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MembreId"] = new SelectList(_context.Membres, "Id", "Email", cotisation.MembreId);
            return View(cotisation);
        }

        // GET: Cotisations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cotisation = await _context.Cotisations.FindAsync(id);
            if (cotisation == null)
            {
                return NotFound();
            }
            ViewData["MembreId"] = new SelectList(_context.Membres, "Id", "Email", cotisation.MembreId);
            return View(cotisation);
        }

        // POST: Cotisations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MembreId,Montant,Type,DatePaiement,DateExpiration")] Cotisation cotisation)
        {
            if (id != cotisation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cotisation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CotisationExists(cotisation.Id))
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
            ViewData["MembreId"] = new SelectList(_context.Membres, "Id", "Email", cotisation.MembreId);
            return View(cotisation);
        }

        // GET: Cotisations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cotisation = await _context.Cotisations
                .Include(c => c.Membre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cotisation == null)
            {
                return NotFound();
            }

            return View(cotisation);
        }

        // POST: Cotisations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cotisation = await _context.Cotisations.FindAsync(id);
            if (cotisation != null)
            {
                _context.Cotisations.Remove(cotisation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CotisationExists(int id)
        {
            return _context.Cotisations.Any(e => e.Id == id);
        }
    }
}
