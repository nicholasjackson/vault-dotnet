using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VaultDotNet.Data;
using VaultDotNet.Models;

namespace VaultDotNet.Controllers
{
    public class FightersController : Controller
    {
        private readonly Fighters _context;

        public FightersController(Fighters context)
        {
            _context = context;
        }

        // GET: Fighters
        public async Task<IActionResult> Index()
        {
              return _context.Fighter != null ? 
                          View(await _context.Fighter.ToListAsync()) :
                          Problem("Entity set 'Fighters.Fighter'  is null.");
        }

        // GET: Fighters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Fighter == null)
            {
                return NotFound();
            }

            var fighter = await _context.Fighter
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fighter == null)
            {
                return NotFound();
            }

            return View(fighter);
        }

        // GET: Fighters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fighters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,ImageURL,Hates,Likes,Height,Weight,ReleaseDate")] Fighter fighter)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fighter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fighter);
        }

        // GET: Fighters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Fighter == null)
            {
                return NotFound();
            }

            var fighter = await _context.Fighter.FindAsync(id);
            if (fighter == null)
            {
                return NotFound();
            }
            return View(fighter);
        }

        // POST: Fighters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ImageURL,Hates,Likes,Height,Weight,ReleaseDate")] Fighter fighter)
        {
            if (id != fighter.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fighter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FighterExists(fighter.Id))
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
            return View(fighter);
        }

        // GET: Fighters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Fighter == null)
            {
                return NotFound();
            }

            var fighter = await _context.Fighter
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fighter == null)
            {
                return NotFound();
            }

            return View(fighter);
        }

        // POST: Fighters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Fighter == null)
            {
                return Problem("Entity set 'Fighters.Fighter'  is null.");
            }
            var fighter = await _context.Fighter.FindAsync(id);
            if (fighter != null)
            {
                _context.Fighter.Remove(fighter);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FighterExists(int id)
        {
          return (_context.Fighter?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
