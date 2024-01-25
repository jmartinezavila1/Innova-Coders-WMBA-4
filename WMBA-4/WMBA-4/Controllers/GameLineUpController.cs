using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMBA_4.Data;
using WMBA_4.Models;
using WMBA_4.ViewModels;

namespace WMBA_4.Controllers
{
    public class GameLineUpController : Controller
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
        public IActionResult Create()
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
        public async Task<IActionResult> Edit(int id, [Bind("ID,BattingOrder,GameID,PlayerID,TeamID")] GameLineUp gameLineUp)
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
            return RedirectToAction(nameof(Index));
        }

        private void PopulatePlayersAssignedTeam(Game game,int team)
        {

            var allOptions = _context.Players
                .Where(m => m.TeamID == team);
            var currentOptionIDs = new HashSet<int>(game.GameLineUps.Select(b => b.GameID));
            var checkBoxes = new List<CheckOptionVM>();
            foreach (var option in allOptions)
            {
                checkBoxes.Add(new CheckOptionVM
                {
                    ID = option.ID,
                    DisplayText = option.FullName,
                    Assigned = currentOptionIDs.Contains(option.ID)
                });
            }
            ViewData["ConditionOptions"] = checkBoxes;
        }

        private void UpdatePatientConditions(string[] selectedOptions, GameLineUp GameLineUpToUpdate)
        {
            //if (selectedOptions == null)
            //{
            //    GameLineUpToUpdate.GameLineUps = new List<GameLineUp>();
            //    return;
            //}

            //var selectedOptionsHS = new HashSet<string>(selectedOptions);
            //var patientOptionsHS = new HashSet<int>
            //    (GameLineUpToUpdate..Select(c => c.ConditionID));//IDs of the currently selected conditions
            //foreach (var option in _context.Conditions)
            //{
            //    if (selectedOptionsHS.Contains(option.ID.ToString())) //It is checked
            //    {
            //        if (!patientOptionsHS.Contains(option.ID))  //but not currently in the history
            //        {
            //            patientToUpdate.PatientConditions.Add(new PatientCondition { PatientID = patientToUpdate.ID, ConditionID = option.ID });
            //        }
            //    }
            //    else
            //    {
            //        //Checkbox Not checked
            //        if (patientOptionsHS.Contains(option.ID)) //but it is currently in the history - so remove it
            //        {
            //            PatientCondition conditionToRemove = patientToUpdate.PatientConditions.SingleOrDefault(c => c.ConditionID == option.ID);
            //            _context.Remove(conditionToRemove);
            //        }
            //    }
            //}
        }
        private bool GameLineUpExists(int id)
        {
          return _context.GameLineUps.Any(e => e.ID == id);
        }
    }
}
