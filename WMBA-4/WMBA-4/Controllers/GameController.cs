using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMBA_4.Data;
using WMBA_4.Models;
using WMBA_4.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public async Task<IActionResult> Index(string seasonName)
        {
            IQueryable<Game> games = _context.Games
        .Include(g => g.Location)
        .Include(g => g.Season)
        .Include(t => t.TeamGames).ThenInclude(t => t.Team);


            if (!string.IsNullOrEmpty(seasonName))
            {
                games = games.Where(g => g.Season.SeasonName.ToLower().Contains(seasonName.ToLower()));
            }
           
            return View(await games.ToListAsync());
        }

        // GET: Game/Details/5
        public async Task<IActionResult> Details(int? id,int team)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Location)
                .Include(g => g.Season)
                .Include(t => t.TeamGames)
                .ThenInclude(t => t.Team)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (game == null)
            {
                return NotFound();
            }

            PopulatePlayersAssignedTeam(game,team);
            ViewBag.TeamID = team;

            return View(game);
        }


        // this is for LineUp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(string[] selectedOptions,int? id,int team,Game game)
        {
            // get the game to update
            var GameToUpdate = await _context.Games
                .Include(p => p.GameLineUps).ThenInclude(p => p.Player)
                .FirstOrDefaultAsync(p => p.ID == id);

            
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "LocationName");
            ViewData["SeasonID"] = new SelectList(_context.Seasons, "ID", "SeasonName");

            // Update thhe lineUp of the game
            UpdateGameLineUp(selectedOptions, GameToUpdate, team);
           
            await _context.SaveChangesAsync();
           
            return RedirectToAction("Details", new { id = id,team=team });
        }

        // GET: Game/Create
        public IActionResult Create(int id)
        {
            Game game = new Game();
            
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "LocationName");
            ViewData["SeasonID"] = new SelectList(_context.Seasons, "ID", "SeasonName");
            ViewBag.Teams = new SelectList(_context.Teams, "ID", "Name");
            ViewBag.TeamID = id;

            return View(game);
        }

        // POST: Game/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Date,LocationID,SeasonID,GameTypeID,TeamID")] Game game,int Team1,int Team2)
        {
            //Game
            if (ModelState.IsValid)
            {
                //Game
                //_context.Add(game);

                //TeamGame (team1)
                var teamGame1 = new TeamGame
                {
                    IsHomeTeam=true,
                    IsVisitorTeam=false,
                    TeamID = Team1,
                    GameID = game.ID
                };
                game.TeamGames.Add(teamGame1);


                //TeamGame (team2)
                var teamGame2 = new TeamGame
                {
                    IsHomeTeam = false,
                    IsVisitorTeam = true,
                    TeamID = Team2,
                    GameID = game.ID

                };
                game.TeamGames.Add(teamGame2);

                _context.Add(game);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
            }

            
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "LocationName", game.LocationID);
            ViewData["SeasonID"] = new SelectList(_context.Seasons, "ID", "SeasonName", game.SeasonID);
            ViewData["TeamID"] = new SelectList(_context.Teams, "ID", "Name");
            return RedirectToAction("Details", "Team", new { id = Team1 });
        }

        // GET: Game/Edit/5
         public async Task<IActionResult> Edit(int? id, int team)
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
           
            
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "LocationName", game.LocationID);
            ViewData["SeasonID"] = new SelectList(_context.Seasons, "ID", "SeasonName", game.SeasonID);
            ViewBag.TeamID = team;
            return View(game);
        }

        // POST: Game/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Date,score,LocationID,SeasonID,GameTypeID")] Game game, int team)
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
            
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "LocationName", game.LocationID);
            ViewData["SeasonID"] = new SelectList(_context.Seasons, "ID", "SeasonName", game.SeasonID);
            ViewBag.TeamID = team;
            return View(game);
        }

        // GET: Game/Delete/5
        public async Task<IActionResult> Delete(int? id, int team)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Location)
                .Include(g => g.Season)
                .Include(t => t.TeamGames).ThenInclude(t => t.Team)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (game == null)
            {
                return NotFound();
            }
            ViewBag.TeamID = team;
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
                .Include(g => g.Location)
                .Include(g => g.Season)
                .Include(t => t.TeamGames).ThenInclude(t => t.Team)
                .FirstOrDefaultAsync(m=>m.ID==id);
            if (game != null)
            {
                game.Status = false;
                _context.Games.Update(game);
             
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private void PopulatePlayersAssignedTeam(Game game, int team)
        {

            var allOptions = _context.Players
                .Where(m => m.TeamID == team);
            var currentOptionIDs = _context.GameLineUps
                                            .Where(m => m.TeamID == team);
            var checkBoxes = new List<CheckOptionVM>();
            foreach (var option in allOptions)
            {
                checkBoxes.Add(new CheckOptionVM
                {
                    ID = option.ID,
                    DisplayText = option.FullName,
                    Assigned = currentOptionIDs.Where(c => c.PlayerID == option.ID).Any(),
                });
            }
            ViewData["PlayersOptions"] = checkBoxes;
        }

        private void UpdateGameLineUp(string[] selectedOptions, Game GameLineUpToUpdate, int team)
        {
            if (selectedOptions == null)
            {
                GameLineUpToUpdate.GameLineUps = new List<GameLineUp>();
                return;
            }

            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var PlayersOptions = new HashSet<int>
                (GameLineUpToUpdate.GameLineUps.Select(c => c.PlayerID));//IDs of the currently selected conditions

            var playersList = _context.Players
               .Where(m => m.TeamID == team);

            foreach (var option in playersList)
            {
                if (selectedOptionsHS.Contains(option.ID.ToString())) //It is checked
                {
                    if (!PlayersOptions.Contains(option.ID))  //but not currently in the history
                    {
                        GameLineUpToUpdate.GameLineUps.Add(new GameLineUp { GameID = GameLineUpToUpdate.ID, PlayerID = option.ID, TeamID = team });
                        
                    }
                }
                
                else
                {
                    //Checkbox Not checked
                    if (PlayersOptions.Contains(option.ID)) //but it is currently in the history - so remove it
                    {
                        GameLineUp playerToRemove = GameLineUpToUpdate.GameLineUps.SingleOrDefault(c => c.PlayerID == option.ID);
                        _context.Remove(playerToRemove);
                    }
                }
            }
        }

        private bool GameExists(int id)
        {
          return _context.Games.Any(e => e.ID == id);
        }
    }
}
