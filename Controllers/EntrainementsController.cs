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
    public class EntrainementsController : Controller
    {
        private readonly ClubSportifDbContext _context;

        public EntrainementsController(ClubSportifDbContext context)
        {
            _context = context;
        }

        // GET: Entrainements
        public async Task<IActionResult> Index()
        {
            return View(await _context.Entrainements.ToListAsync());
        }

        // GET: Entrainements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entrainement = await _context.Entrainements
                .FirstOrDefaultAsync(m => m.Id == id);
            if (entrainement == null)
            {
                return NotFound();
            }

            return View(entrainement);
        }

        // GET: Entrainements/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Entrainements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titre,Description,DateHeure,Lieu,TypeEvenement")] Entrainement entrainement)
        {
            if (ModelState.IsValid)
            {
                _context.Add(entrainement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(entrainement);
        }

        // GET: Entrainements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entrainement = await _context.Entrainements.FindAsync(id);
            if (entrainement == null)
            {
                return NotFound();
            }
            return View(entrainement);
        }

        // POST: Entrainements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titre,Description,DateHeure,Lieu,TypeEvenement")] Entrainement entrainement)
        {
            if (id != entrainement.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(entrainement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntrainementExists(entrainement.Id))
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
            return View(entrainement);
        }

        // GET: Entrainements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entrainement = await _context.Entrainements
                .FirstOrDefaultAsync(m => m.Id == id);
            if (entrainement == null)
            {
                return NotFound();
            }

            return View(entrainement);
        }

        // POST: Entrainements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entrainement = await _context.Entrainements.FindAsync(id);
            if (entrainement != null)
            {
                _context.Entrainements.Remove(entrainement);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EntrainementExists(int id)
        {
            return _context.Entrainements.Any(e => e.Id == id);
        }
    }
}
