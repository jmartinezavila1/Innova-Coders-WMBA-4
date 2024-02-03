using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMBA_4.CustomControllers;
using WMBA_4.Data;
using WMBA_4.Models;
using WMBA_4.ViewModels;

namespace WMBA_4.Controllers
{
    public class GameLineUpController : ElephantController
    {
        private readonly WMBA_4_Context _context;

        public GameLineUpController(WMBA_4_Context context)
        {
            _context = context;
        }

        // GET: GameLineUp
        public async Task<IActionResult> Index(int? id)
        {
            var wMBA_4_Context = _context.GameLineUps
                .Include(g => g.Game)
                .Include(g => g.Player)
                .Include(g => g.Team)
                .Where(m => m.TeamID == id)
                .ToListAsync();
            return View(await wMBA_4_Context);
        }

        // GET: GameLineUp/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GameLineUps == null)
            {
                return NotFound();
            }

            var gameLineUp = await _context.GameLineUps
                .Include(g => g.Game)
                .Include(g => g.Player)
                .Include(g => g.Team)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (gameLineUp == null)
            {
                return NotFound();
            }

            return View(gameLineUp);
        }

        // GET: GameLineUp/Create
        public IActionResult Create(int team)
        {
            
            ViewData["GameID"] = new SelectList(_context.Games, "ID", "ID");
            ViewData["PlayerID"] = new SelectList(_context.Players, "ID", "FirstName");
            ViewData["TeamID"] = new SelectList(_context.Teams, "ID", "Name");
           
            return View();
        }

        // POST: GameLineUp/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,BattingOrder,GameID,PlayerID,TeamID")] GameLineUp gameLineUp)
        {

            if (ModelState.IsValid)
            {
                _context.Add(gameLineUp);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var gameL = new GameLineUp();
            ViewData["GameID"] = new SelectList(_context.Games, "ID", "ID", gameLineUp.GameID);
            ViewData["PlayerID"] = new SelectList(_context.Players, "ID", "FirstName", gameLineUp.PlayerID);
            ViewData["TeamID"] = new SelectList(_context.Teams, "ID", "Name", gameLineUp.TeamID);
            
            return View(gameLineUp);
        }

        // GET: GameLineUp/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GameLineUps == null)
            {
                return NotFound();
            }

            var gameLineUp = await _context.GameLineUps.FindAsync(id);
            if (gameLineUp == null)
            {
                return NotFound();
            }
            ViewData["GameID"] = new SelectList(_context.Games, "ID", "ID", gameLineUp.GameID);
            ViewData["PlayerID"] = new SelectList(_context.Players, "ID", "FirstName", gameLineUp.PlayerID);
            ViewData["TeamID"] = new SelectList(_context.Teams, "ID", "Name", gameLineUp.TeamID);
            return View(gameLineUp);
        }

        // POST: GameLineUp/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,BattingOrder,GameID,PlayerID,TeamID")] GameLineUp gameLineUp, string[] selectedOptions, int team)
        {
            if (id != gameLineUp.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gameLineUp);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameLineUpExists(gameLineUp.ID))
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
           
            ViewData["GameID"] = new SelectList(_context.Games, "ID", "ID", gameLineUp.GameID);
            ViewData["PlayerID"] = new SelectList(_context.Players, "ID", "FirstName", gameLineUp.PlayerID);
            ViewData["TeamID"] = new SelectList(_context.Teams, "ID", "Name", gameLineUp.TeamID);
            
            return View(gameLineUp);
        }

        // GET: GameLineUp/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GameLineUps == null)
            {
                return NotFound();
            }

            var gameLineUp = await _context.GameLineUps
                .Include(g => g.Game)
                .Include(g => g.Player)
                .Include(g => g.Team)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (gameLineUp == null)
            {
                return NotFound();
            }

            return View(gameLineUp);
        }

        // POST: GameLineUp/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GameLineUps == null)
            {
                return Problem("Entity set 'WMBA_4_Context.GameLineUps'  is null.");
            }
            var gameLineUp = await _context.GameLineUps.FindAsync(id);
            if (gameLineUp != null)
            {
                _context.GameLineUps.Remove(gameLineUp);
            }
            
            await _context.SaveChangesAsync();
            return Redirect(ViewData["returnURL"].ToString());
        }

        
        private bool GameLineUpExists(int id)
        {
          return _context.GameLineUps.Any(e => e.ID == id);
        }
    }
}
