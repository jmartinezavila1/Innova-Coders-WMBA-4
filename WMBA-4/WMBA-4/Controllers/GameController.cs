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
    public class GameController : Controller
    {
        private readonly WMBA_4_Context _context;

        public GameController(WMBA_4_Context context)
        {
            _context = context;
        }

        // GET: Game
        public async Task<IActionResult> Index()
        {
            var games = _context.Games
                .Include(g => g.GameType)
                .Include(g => g.Location)
                .Include(g => g.Season)
                .Include(t=>t.TeamGames).ThenInclude(t=>t.Team);
            return View(await games.ToListAsync());
        }

        // GET: Game/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.GameType)
                .Include(g => g.Location)
                .Include(g => g.Season)
                .Include(t => t.TeamGames).ThenInclude(t => t.Team)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // GET: Game/Create
        public IActionResult Create()
        {
            ViewData["GameTypeID"] = new SelectList(_context.GameTypes, "ID", "Description");
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "LocationName");
            ViewData["SeasonID"] = new SelectList(_context.Seasons, "ID", "SeasonName");
            return View();
        }

        // POST: Game/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Date,score,LocationID,SeasonID,GameTypeID")] Game game)
        {
            if (ModelState.IsValid)
            {
                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GameTypeID"] = new SelectList(_context.GameTypes, "ID", "Description", game.GameTypeID);
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "LocationName", game.LocationID);
            ViewData["SeasonID"] = new SelectList(_context.Seasons, "ID", "SeasonName", game.SeasonID);
            return View(game);
        }

        // GET: Game/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            ViewData["GameTypeID"] = new SelectList(_context.GameTypes, "ID", "Description", game.GameTypeID);
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "LocationName", game.LocationID);
            ViewData["SeasonID"] = new SelectList(_context.Seasons, "ID", "SeasonName", game.SeasonID);
            return View(game);
        }

        // POST: Game/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Date,score,LocationID,SeasonID,GameTypeID")] Game game)
        {
            if (id != game.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.ID))
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
            ViewData["GameTypeID"] = new SelectList(_context.GameTypes, "ID", "Description", game.GameTypeID);
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "LocationName", game.LocationID);
            ViewData["SeasonID"] = new SelectList(_context.Seasons, "ID", "SeasonName", game.SeasonID);
            return View(game);
        }

        // GET: Game/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.GameType)
                .Include(g => g.Location)
                .Include(g => g.Season)
                .Include(t => t.TeamGames).ThenInclude(t => t.Team)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Game/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Games == null)
            {
                return Problem("Entity set 'WMBA_4_Context.Games'  is null.");
            }
            var game = await _context.Games
                .Include(g => g.GameType)
                .Include(g => g.Location)
                .Include(g => g.Season)
                .Include(t => t.TeamGames).ThenInclude(t => t.Team)
                .FirstOrDefaultAsync(m=>m.ID==id);
            if (game != null)
            {
                _context.Games.Remove(game);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
          return _context.Games.Any(e => e.ID == id);
        }
    }
}
