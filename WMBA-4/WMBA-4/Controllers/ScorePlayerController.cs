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
    public class ScorePlayerController : Controller
    {
        private readonly WMBA_4_Context _context;

        public ScorePlayerController(WMBA_4_Context context)
        {
            _context = context;
        }

        // GET: ScorePlayer
        public async Task<IActionResult> Index()
        {
            var wMBA_4_Context = _context.ScorePlayers.Include(s => s.Game).Include(s => s.Player);
            return View(await wMBA_4_Context.ToListAsync());
        }

        // GET: ScorePlayer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ScorePlayers == null)
            {
                return NotFound();
            }

            var scorePlayer = await _context.ScorePlayers
                .Include(s => s.Game)
                .Include(s => s.Player)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (scorePlayer == null)
            {
                return NotFound();
            }

            return View(scorePlayer);
        }

        // GET: ScorePlayer/Create
        public IActionResult Create()
        {
            ViewData["GameID"] = new SelectList(_context.Games, "ID", "ID");
            ViewData["PlayerID"] = new SelectList(_context.Players, "ID", "FirstName");
            return View();
        }

        // POST: ScorePlayer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,InningNumber,H,RBI,R,StrikeOut,GroundOut,PopOut,Flyout,Singles,Doubles,Triples,HR,BB,HBP,SB,SAC,PA,AB,GameID,PlayerID")] ScorePlayer scorePlayer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(scorePlayer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GameID"] = new SelectList(_context.Games, "ID", "ID", scorePlayer.GameID);
            ViewData["PlayerID"] = new SelectList(_context.Players, "ID", "FirstName", scorePlayer.PlayerID);
            return View(scorePlayer);
        }

        // GET: ScorePlayer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ScorePlayers == null)
            {
                return NotFound();
            }

            var scorePlayer = await _context.ScorePlayers.FindAsync(id);
            if (scorePlayer == null)
            {
                return NotFound();
            }
            ViewData["GameID"] = new SelectList(_context.Games, "ID", "ID", scorePlayer.GameID);
            ViewData["PlayerID"] = new SelectList(_context.Players, "ID", "FirstName", scorePlayer.PlayerID);
            return View(scorePlayer);
        }

        // POST: ScorePlayer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,InningNumber,H,RBI,R,StrikeOut,GroundOut,PopOut,Flyout,Singles,Doubles,Triples,HR,BB,HBP,SB,SAC,PA,AB,GameID,PlayerID")] ScorePlayer scorePlayer)
        {
            if (id != scorePlayer.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(scorePlayer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScorePlayerExists(scorePlayer.ID))
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
            ViewData["GameID"] = new SelectList(_context.Games, "ID", "ID", scorePlayer.GameID);
            ViewData["PlayerID"] = new SelectList(_context.Players, "ID", "FirstName", scorePlayer.PlayerID);
            return View(scorePlayer);
        }

        // GET: ScorePlayer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ScorePlayers == null)
            {
                return NotFound();
            }

            var scorePlayer = await _context.ScorePlayers
                .Include(s => s.Game)
                .Include(s => s.Player)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (scorePlayer == null)
            {
                return NotFound();
            }

            return View(scorePlayer);
        }

        // POST: ScorePlayer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ScorePlayers == null)
            {
                return Problem("Entity set 'WMBA_4_Context.ScorePlayers'  is null.");
            }
            var scorePlayer = await _context.ScorePlayers.FindAsync(id);
            if (scorePlayer != null)
            {
                _context.ScorePlayers.Remove(scorePlayer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScorePlayerExists(int id)
        {
          return _context.ScorePlayers.Any(e => e.ID == id);
        }
    }
}
