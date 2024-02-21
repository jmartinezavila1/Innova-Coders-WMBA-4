using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using WMBA_4.CustomControllers;
using WMBA_4.Data;
using WMBA_4.Models;
using WMBA_4.Utilities;
using WMBA_4.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WMBA_4.Controllers
{
    public class GameController : ElephantController
    {
        private readonly WMBA_4_Context _context;

        public GameController(WMBA_4_Context context)
        {
            _context = context;
        }

        // GET: Game
        public async Task<IActionResult> Index(int? divisionID, string SearchString, int? GameTypeID, int? page, int? pageSizeID,
            string actionButton, string sortDirection = "asc", string sortField = "Location")
        {
            IQueryable<Game> games = _context.Games
            .Include(g => g.GameType)
            .Include(g => g.Location)
            .Include(g => g.Season)
            .Include(t => t.TeamGames)
                .ThenInclude(t => t.Team)
                    .ThenInclude(d => d.Division)            
            .Where(s => s.Status == true);

            //if (!string.IsNullOrEmpty(seasonName))
            //{
            //    games = games.Where(g => g.Season.SeasonName.ToLower().Contains(seasonName.ToLower()));
            //}

            //sorting sortoption array
            string[] sortOptions = new[] { "Division", "Location", "GameType", "Date" };

            //filter
            if (divisionID.HasValue)
            {
                games = games.Where(g => g.TeamGames.Any(t => t.Team.DivisionID == divisionID));
            }
            if (GameTypeID.HasValue)
            {
                games = games.Where(g => g.GameTypeID == GameTypeID);
            }
            if (!System.String.IsNullOrEmpty(SearchString))
            {
                games = games.Where(l => l.Location.LocationName.ToUpper().Contains(SearchString.ToUpper()));
            }

            if (!System.String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1;//Reset page to start
                         //sorting
                if (!System.String.IsNullOrEmpty(actionButton)) //Form Submitted!
                {
                    if (sortOptions.Contains(actionButton))//Change of sort is requested
                    {
                        if (actionButton == sortField) //Reverse order on same field
                        {
                            sortDirection = sortDirection == "asc" ? "desc" : "asc";
                        }
                        sortField = actionButton;//Sort by the button clicked
                    }
                }
                if (sortField == "Division")
                {
                    if (sortDirection == "asc")
                    {
                        games = games
                            .OrderBy(l => l.TeamGames.FirstOrDefault().Team.Division.DivisionName);
                    }
                    else
                    {
                        games = games
                            .OrderByDescending(l => l.TeamGames.FirstOrDefault().Team.Division.DivisionName);
                    }
                }
                else
                if (sortField == "Location")
                {
                    if (sortDirection == "asc")
                    {
                        games = games
                            .OrderBy(l => l.Location.LocationName);
                    }
                    else
                    {
                        games = games
                            .OrderByDescending(l => l.Location.LocationName);
                    }
                }
                else if (sortField == "GameType")
                {
                    if (sortDirection == "asc")
                    {
                        games = games
                            .OrderBy(g => g.GameType.Description);
                    }
                    else
                    {
                        games = games
                            .OrderByDescending(p => p.GameType.Description);
                    }
                }
                else
                {
                    if (sortDirection == "asc")
                    {
                        games = games
                            .OrderBy(g => g.GameType.Description);
                    }
                    else
                    {
                        games = games
                            .OrderByDescending(g => g.GameType.Description);
                    }
                }
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            //ViewData["LocationID"] = new SelectList(_context.Teams, "ID", "LocationName");
            
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName");
            ViewData["SearchString"] = SearchString;            
            ViewData["GameTypeID"] = new SelectList(_context.GameTypes, "ID", "Description");

            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID);
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);            
            var pagedData = await PaginatedList<Game>.CreateAsync(games, page ?? 1, pageSize);           


            //return View(await games.ToListAsync());
            return View(pagedData);
        }

        // GET: Game/Details/5
        public async Task<IActionResult> Details(int? id, int team)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.GameType)
                .Include(g => g.Location)
                .Include(g => g.Season)
                .Include(t => t.TeamGames)
                    .ThenInclude(t => t.Team)
                        .ThenInclude(d => d.Division)
                .FirstOrDefaultAsync(m => m.ID == id);



            if (game == null)
            {
                return NotFound();
            }

            PopulatePlayersAssignedTeam(game, team);
            ViewBag.TeamID = team;

            return View(game);
        }
        // this is for LineUp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(string[] selectedOptions, int? id, int team, Game game)
        {
            // Obtener el juego para actualizar
            var GameToUpdate = await _context.Games
                .Include(p => p.GameLineUps).ThenInclude(p => p.Player)
                .FirstOrDefaultAsync(p => p.ID == id);

            ViewData["GameTypeID"] = new SelectList(_context.GameTypes, "ID", "Description");
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "LocationName");
            ViewData["SeasonID"] = new SelectList(_context.Seasons, "ID", "SeasonName");

            // Actualizar la alineación del juego
            UpdateGameLineUp(selectedOptions, GameToUpdate, team);


            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = id, team = team });
        }
        
        // GET: Game/Create
        public IActionResult Create(int id)
        {
            //Game game = new Game();
            var game = new Game
            {
                TeamGames = new List<TeamGame> // Create a new List<TeamGame> to hold the collection of TeamGame objects
                {
                    _context.TeamGame.Include(tg => tg.Team).ThenInclude(t => t.Division).FirstOrDefault()
                }
            };

            var divisions = _context.Divisions.ToList();
            var firstDivisionId = divisions.FirstOrDefault()?.ID;

            
            
            ViewBag.Teams = new SelectList(_context.Teams.Where(t => t.DivisionID == firstDivisionId), "TeamID", "TeamName");
            //ViewData["Divisions"] = new SelectList(_context.TeamGame.Include(tg => tg.Team).ThenInclude(t => t.Division).Select(tg => tg.Team.Division).Distinct(), "DivisionID", "DivisionName");
            ViewData["GameTypeID"] = new SelectList(_context.GameTypes, "ID", "Description");
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "LocationName");
            ViewData["SeasonID"] = new SelectList(_context.Seasons, "ID", "SeasonName");
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName");
            ViewBag.Teams = new SelectList(_context.Teams, "ID", "Name");
            ViewBag.TeamID = id;

            return View(game);
        }

        // POST: Game/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Date,LocationID,SeasonID,GameTypeID,TeamID")] Game game, int Team1, int Team2, int teamId)
        {
            //Game
            if (ModelState.IsValid)
            {
                try
                {
                    //TeamGame (team1)
                    var teamGame1 = new TeamGame
                    {
                        IsHomeTeam = true,
                        IsVisitorTeam = false,
                        TeamID = Team1,
                        GameID = game.ID
                    };
                    if (!_context.TeamGame.Local.Any(e => e.GameID == teamGame1.GameID && e.TeamID == teamGame1.TeamID))
                    {
                        game.TeamGames.Add(teamGame1);
                    }

                    //TeamGame (team2)
                    var teamGame2 = new TeamGame
                    {
                        IsHomeTeam = false,
                        IsVisitorTeam = true,
                        TeamID = Team2,
                        GameID = game.ID
                    };
                    if (!_context.TeamGame.Local.Any(e => e.GameID == teamGame2.GameID && e.TeamID == teamGame2.TeamID))
                    {
                        game.TeamGames.Add(teamGame2);
                    }

                    _context.Add(game);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +                        
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName");
            ViewData["GameTypeID"] = new SelectList(_context.GameTypes, "ID", "Description", game.GameTypeID);
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "LocationName", game.LocationID);
            ViewData["SeasonID"] = new SelectList(_context.Seasons, "ID", "SeasonName", game.SeasonID);
            ViewBag.Teams = new SelectList(_context.Teams, "ID", "Name");
            
            return View(game);
        }

        // GET: Game/Edit/5
        public async Task<IActionResult> Edit(int? id, int team)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(tg => tg.TeamGames)
                    .ThenInclude(t => t.Team)
                        .Include(tg => tg.TeamGames)
                            .ThenInclude(t => t.Team.Division)
                .FirstOrDefaultAsync(g => g.ID == id);

            if (game == null)
            {
                return NotFound();
            }

            var teamGames = await _context.TeamGame.Where(tg => tg.GameID == id).ToListAsync();
                if (teamGames.Count != 2)
                {
                    // Handle the case where there are not exactly two teams for the game
                }

            var teamGame1 = teamGames[0];
            var teamGame2 = teamGames[1];


            ViewData["GameTypeID"] = new SelectList(_context.GameTypes, "ID", "Description", game.GameTypeID);
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "LocationName", game.LocationID);
            ViewData["SeasonID"] = new SelectList(_context.Seasons, "ID", "SeasonName", game.SeasonID);            
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName");
            ViewData["Team1ID"] = new SelectList(_context.Teams, "ID", "Name", teamGame1.TeamID);
            ViewData["Team2ID"] = new SelectList(_context.Teams, "ID", "Name", teamGame2.TeamID);            
            
            ViewData["Score1"] = teamGame1.score;
            ViewData["Score2"] = teamGame2.score;
            ViewBag.TeamID = team;
            return View(game);
        }

        // POST: Game/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Date,Status,LocationID,SeasonID,GameTypeID")] Game game, int team, int team1Id, int team2Id, int score1, int score2)
        {
            if (id != game.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var teamGames = await _context.TeamGame.Where(tg => tg.GameID == id).ToListAsync();
                if (teamGames.Count != 2)
                {
                    // Handle the case where there are not exactly two teams for the game                    
                }

                var teamGame1 = teamGames[0];
                var teamGame2 = teamGames[1];
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        // Delete the existing TeamGames
                        _context.TeamGame.Remove(teamGame1);
                        _context.TeamGame.Remove(teamGame2);
                        await _context.SaveChangesAsync();

                        // Create new TeamGames with the new TeamIDs
                        var newTeamGame1 = new TeamGame
                        {
                            GameID = id,
                            TeamID = team1Id,
                            score = score1,
                            IsHomeTeam = true,
                            IsVisitorTeam = false
                        };
                        _context.TeamGame.Add(newTeamGame1);

                        var newTeamGame2 = new TeamGame
                        {
                            GameID = id,
                            TeamID = team2Id,
                            score = score2,
                            IsHomeTeam = false,
                            IsVisitorTeam = true
                        };
                        _context.TeamGame.Add(newTeamGame2);

                        // Update the game
                        _context.Update(game);
                        await _context.SaveChangesAsync();

                        // Commit the transaction
                        transaction.Commit();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        // Roll back the transaction
                        transaction.Rollback();

                        if (!GameExists(game.ID))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        // Roll back the transaction
                        transaction.Rollback();
                        
                        ModelState.AddModelError("", "Unable to save changes. " +                        
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                }
            }
            ViewData["GameTypeID"] = new SelectList(_context.GameTypes, "ID", "Description", game.GameTypeID);
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "LocationName", game.LocationID);
            ViewData["SeasonID"] = new SelectList(_context.Seasons, "ID", "SeasonName", game.SeasonID);
            //ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", game.DivisionID); 
            //ViewData["DivisionID"] = new SelectList(_context.TeamGame.Include(tg => tg.Team).ThenInclude(t => t.Division).Select(tg => tg.Team.Division).Distinct(), "DivisionID", "DivisionName");
            ViewData["Team1ID"] = new SelectList(_context.Teams, "ID", "Name", team1Id);
            ViewData["Team2ID"] = new SelectList(_context.Teams, "ID", "Name", team2Id);

            ViewData["Score1"] = score1;
            ViewData["Score2"] = score2;
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
                .Include(g => g.GameType)
                .Include(g => g.Location)
                .Include(g => g.Season)
                .Include(t => t.TeamGames)
                    .ThenInclude(t => t.Team)
                    .ThenInclude(d => d.Division)
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
        public async Task<IActionResult> DeleteConfirmed(int id, int teamId)
        {
            if (_context.Games == null)
            {
                return Problem("Entity set 'WMBA_4_Context.Games' is null.");
            }
            var game = await _context.Games
                .Include(g => g.GameType)
                .Include(g => g.Location)
                .Include(g => g.Season)
                .Include(t => t.TeamGames)
                    .ThenInclude(t => t.Team)
                    .ThenInclude(d => d.Division)
                .FirstOrDefaultAsync(m => m.ID == id);
            try
            {
                if (game != null)
                {
                    game.Status = false;
                    _context.Games.Update(game);

                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists see your system administrator.");
            }
            await _context.SaveChangesAsync();
            //return RedirectToAction("RedirectToTeamList", new { teamId });
            return RedirectToAction(nameof(Index));

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RedirectToTeamList(int teamId)
        {
            return RedirectToAction("Details", "Team", new { id = teamId });
        }

        private void PopulatePlayersAssignedTeam(Game game, int team)
        {

            var allOptions = _context.Players
               .Where(m => m.TeamID == team && m.Status == true);

            var currentOptionIDs = _context.GameLineUps
                                            .Where(m => m.TeamID == team && m.GameID == game.ID);
            var teamName = _context.Teams
                .Where(m => m.ID == team)
                .Select(m => m.Name)
                .FirstOrDefault();

            var checkBoxes = new List<CheckOptionVM>();
            var CheckBoxesOrdered = new List<CheckOptionVM>();
            foreach (var option in allOptions)
            {
                checkBoxes.Add(new CheckOptionVM
                {
                    ID = option.ID,
                    DisplayText = option.FullName,
                    BattingOrder = currentOptionIDs.Where(c => c.PlayerID == option.ID).Select(c => c.BattingOrder).FirstOrDefault(),
                    Assigned = currentOptionIDs.Where(c => c.PlayerID == option.ID).Any(),
                });
            }

            CheckBoxesOrdered = checkBoxes
            .OrderBy(b => b.BattingOrder == 0 ? 1 : 0)
            .ThenBy(b => b.BattingOrder)
            .ToList();
            ViewData["PlayersOptions"] = CheckBoxesOrdered;
            ViewData["TeamName"] = teamName;
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


            int battingOrder = 1;

            var gameLineUpsToRemove = _context.GameLineUps
                                  .Where(m => m.TeamID == team && m.GameID == GameLineUpToUpdate.ID);

            _context.GameLineUps.RemoveRange(gameLineUpsToRemove);
         
            foreach (var option in selectedOptionsHS)
            {

                var newGameLineUp = new GameLineUp { GameID = GameLineUpToUpdate.ID, BattingOrder = battingOrder, PlayerID = int.Parse(option), TeamID = team };
                GameLineUpToUpdate.GameLineUps.Add(newGameLineUp);

                battingOrder += 1;
                
            }
            _context.SaveChanges();
        }

        public async Task<IActionResult> GetTeams(int divisionId)
        {
            var teams = await _context.Teams
                .Where(t => t.DivisionID == divisionId)
                .Select(t => new { teamId = t.ID, teamName = t.Name })
                .ToListAsync();

            return Json(teams);
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.ID == id);
        }
    }
}
