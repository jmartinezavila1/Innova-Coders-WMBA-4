using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        public async Task<IActionResult> Index(int? divisionID, string SearchString, int? GameTypeID, bool isActive, bool isInactive, int? page, int? pageSizeID, int? year, int? month, int? day,
            string actionButton, string sortDirection = "asc", string sortField = "Location")
        {

            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;

            IQueryable<Game> games = _context.Games
            .Include(g => g.GameType)
            .Include(g => g.Location)
            .Include(g => g.Season)
            .Include(t => t.TeamGames)
                .ThenInclude(t => t.Team)
                    .ThenInclude(d => d.Division);         
            //.Where(s => s.Status == true);

            //sorting sortoption array
            string[] sortOptions = new[] { "Division", "Location", "GameType", "Date" };

            //filter
            if (divisionID.HasValue)
            {
                games = games.Where(g => g.TeamGames.Any(t => t.Team.DivisionID == divisionID));
                numberFilters++;
            }
            if (GameTypeID.HasValue)
            {
                games = games.Where(g => g.GameTypeID == GameTypeID);
                numberFilters++;
            }
            if (!System.String.IsNullOrEmpty(SearchString))
            {
                games = games.Where(l => l.Location.LocationName.ToUpper().Contains(SearchString.ToUpper()));
                numberFilters++;
            }    
            if (year.HasValue)
            {
                games = games.Where(g => g.Date.Year == year.Value);
                numberFilters++;
            }
            if (month.HasValue)
            {
                games = games.Where(g => g.Date.Month == month.Value);
                numberFilters++;
            }
            if (day.HasValue)
            {
                games = games.Where(g => g.Date.Day == day.Value);
                numberFilters++;
            }

            if (isActive == true)
            {
                games = games.Where(g => g.Status == true);
                numberFilters++;
            }
            if (isInactive == true)
            {
                games = games.Where(g => g.Status == false);
                numberFilters++;
            }

            if (numberFilters != 0)
            {
                //Toggle the Open/Closed state of the collapse depending on if we are filtering
                ViewData["Filtering"] = " btn-danger";
                //Show how many filters have been applied
                ViewData["numberFilters"] = "(" + numberFilters.ToString()
                    + " Filter" + (numberFilters > 1 ? "s" : "") + " Applied)";
                //Keep the Bootstrap collapse open
                //@ViewData["ShowFilter"] = " show";
            }

            games = games.OrderByDescending(g => g.Status) // Active games first
                    .ThenBy(g => g.Date);             // Order by last name


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
                if (sortField == "Date")
                {
                    if (sortDirection == "asc")
                    {
                        games = games
                            .OrderByDescending(d => d.Status)
                            .ThenBy(d => d.Date);
                    }
                    else
                    {
                        games = games
                         .OrderByDescending(d => d.Status)
                         .ThenByDescending(d => d.Date);
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

        public async Task<IActionResult> Activate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            // Set the player's status to active
            game.Status = true;
            _context.Games.Update(game);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Game/Details/5
        public async Task<IActionResult> Details(int? gameid, int team)
        {
            if (gameid == null || _context.Games == null)
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
                .FirstOrDefaultAsync(m => m.ID == gameid);



            if (game == null)
            {
                return NotFound();
            }

            PopulatePlayersAssignedTeam(game, team);
            ViewBag.TeamID = team;
            ViewBag.GameID = gameid;

            return View(game);
        }
        // this is for LineUp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(string[] selectedOptions, int? gameid, int team, Game game)
        {
            // Obtener el juego para actualizar
            var GameToUpdate = await _context.Games
                .Include(p => p.GameLineUps).ThenInclude(p => p.Player)
                .FirstOrDefaultAsync(p => p.ID == gameid);

            ViewData["GameTypeID"] = new SelectList(_context.GameTypes, "ID", "Description");
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "LocationName");
            ViewData["SeasonID"] = new SelectList(_context.Seasons, "ID", "SeasonName");

            // Actualizar la alineación del juego
            UpdateGameLineUp(selectedOptions, GameToUpdate, team);


            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Game", new { gameid = gameid, team = team });
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
        public async Task<IActionResult> Create([Bind("Date,LocationID,SeasonID,GameTypeID,TeamID")] Game game, int Team1, int Team2, int teamId /*, string LocationName*/)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            if (Team1 == Team2)
            {
                ModelState.AddModelError("", "A team cannot play against itself. Please select two different teams.");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            // Verifica si el LocationID existe en la base de datos
            //var location = await _context.Locations.FindAsync(game.LocationID);
            //if (location == null)
            //{
            //    // Si no existe, crea una nueva ubicación y guárdala en la base de datos
            //    location = new Models.Location { ID = game.LocationID, LocationName = LocationName, CityID = 1 };
            //    _context.Locations.Add(location);
            //    await _context.SaveChangesAsync();

            //    game.LocationID = location.ID;    

            //    ModelState.Remove("LocationID");            
            //}
            
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
                    return Redirect(ViewData["returnURL"].ToString());
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

            var divisionId1 = teamGame1.Team.DivisionID; // Get the DivisionID from Team1 of the teams
            

            ViewData["GameTypeID"] = new SelectList(_context.GameTypes, "ID", "Description", game.GameTypeID);
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "LocationName", game.LocationID);
            ViewData["SeasonID"] = new SelectList(_context.Seasons, "ID", "SeasonName", game.SeasonID);            
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", divisionId1); // Set the selected DivisionID
            ViewData["Team1ID"] = new SelectList(_context.Teams.Where(t => t.DivisionID == divisionId1), "ID", "Name", teamGame1.TeamID); // Filter the teams by DivisionID
            ViewData["Team2ID"] = new SelectList(_context.Teams.Where(t => t.DivisionID == divisionId1), "ID", "Name", teamGame2.TeamID); // Filter the teams by DivisionID
       
            
            ViewData["Score1"] = teamGame1.score;
            ViewData["Score2"] = teamGame2.score;
            ViewBag.TeamID = team;
            //return View(game);

            var viewModel = new GameEditVM
            {
                Game = game,
                DivisionID = divisionId1,
                Team1ID = teamGame1.TeamID,
                Team2ID = teamGame2.TeamID,
                Score1 = teamGame1.score,
                Score2 = teamGame2.score
            };

            return View(viewModel);
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
                        return Redirect(ViewData["returnURL"].ToString());
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
                ModelState.AddModelError("", "Unable to Inactive record. Try again, and if the problem persists see your system administrator.");
            }
            await _context.SaveChangesAsync();
            //return RedirectToAction("RedirectToTeamList", new { teamId });
            return Redirect(ViewData["returnURL"].ToString());

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

            foreach (var option in selectedOptionsHS)
            {
                var playerId = int.Parse(option);
                var existingGameLineUp = _context.GameLineUps
                    .FirstOrDefault(m => m.TeamID == team && m.GameID == GameLineUpToUpdate.ID && m.PlayerID == playerId);

                if (existingGameLineUp != null)
                {
                    // Update the batting order of the existing player
                    existingGameLineUp.BattingOrder = battingOrder;

                }
                else
                {
                    // Add the new player to the game lineup
                    var newGameLineUp = new GameLineUp { GameID = GameLineUpToUpdate.ID, BattingOrder = battingOrder, PlayerID = playerId, TeamID = team };
                    GameLineUpToUpdate.GameLineUps.Add(newGameLineUp);
                }

                battingOrder += 1;
            }

            var playersToRemove = GameLineUpToUpdate.GameLineUps
                .Where(m => !selectedOptionsHS.Contains(m.PlayerID.ToString()))
                .ToList();

            foreach (var playerToRemove in playersToRemove)
            {
                if (playerToRemove.TeamID == team)
                {
                    GameLineUpToUpdate.GameLineUps.Remove(playerToRemove);
                }

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

        [HttpPost]
        public async Task<IActionResult> CreateLocation(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Name is required.");
            }

            var location = new Models.Location { LocationName = name };
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();

            return Ok(location.ID);
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.ID == id);
        }
    }
}
