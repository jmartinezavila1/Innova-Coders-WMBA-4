using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMBA_4.Data;
using WMBA_4.Models;

namespace WMBA_4.Controllers
{
    public class DivisionController : Controller
    {
        private readonly WMBA_4_Context _context;

        public DivisionController(WMBA_4_Context context)
        {
            _context = context;
        }

        // GET: Division
        public async Task<IActionResult> Index()
        {
            var wMBA_4_Context = _context.Divisions.Include(d => d.Club);
            return View(await wMBA_4_Context.ToListAsync());
        }

        // GET: Division/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Divisions == null)
            {
                return NotFound();
            }

            var division = await _context.Divisions
                .Include(d => d.Club)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (division == null)
            {
                return NotFound();
            }

            return View(division);
        }

        // GET: Division/Create
        public IActionResult Create()
        {
            ViewData["ClubID"] = new SelectList(_context.Clubs, "ID", "ClubName");
            return View();
        }

        // POST: Division/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,DivisionName,ClubID")] Division division)
        {
            if (ModelState.IsValid)
            {
                _context.Add(division);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClubID"] = new SelectList(_context.Clubs, "ID", "ClubName", division.ClubID);
            return View(division);
        }

        // GET: Division/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Divisions == null)
            {
                return NotFound();
            }

            var division = await _context.Divisions.FindAsync(id);
            if (division == null)
            {
                return NotFound();
            }
            ViewData["ClubID"] = new SelectList(_context.Clubs, "ID", "ClubName", division.ClubID);
            return View(division);
        }

        // POST: Division/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,DivisionName,ClubID")] Division division)
        {
            if (id != division.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(division);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DivisionExists(division.ID))
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
            ViewData["ClubID"] = new SelectList(_context.Clubs, "ID", "ClubName", division.ClubID);
            return View(division);
        }

        // GET: Division/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Divisions == null)
            {
                return NotFound();
            }

            var division = await _context.Divisions
                .Include(d => d.Club)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (division == null)
            {
                return NotFound();
            }

            return View(division);
        }

        // POST: Division/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Divisions == null)
            {
                return Problem("Entity set 'WMBA_4_Context.Divisions'  is null.");
            }
            var division = await _context.Divisions.FindAsync(id);
            if (division != null)
            {
                division.Status = false;
                _context.Divisions.Update(division);
                
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DivisionExists(int id)
        {
          return _context.Divisions.Any(e => e.ID == id);
        }
    }
}
