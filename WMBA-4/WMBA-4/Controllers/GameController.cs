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
        public async Task<IActionResult> Index(string SearchString, int? GameTypeID, int? page, int? pageSizeID,
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
            string[] sortOptions = new[] { "Location", "GameType", "Date" };

            //filter
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
            Game game = new Game();

            ViewData["GameTypeID"] = new SelectList(_context.GameTypes, "ID", "Description");
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
        public async Task<IActionResult> Create([Bind("Date,LocationID,SeasonID,GameTypeID,TeamID")] Game game, int Team1, int Team2, int teamId)
        {
            //Game
            if (ModelState.IsValid)
            {
                //Game
                //_context.Add(game);

                //TeamGame (team1)
                var teamGame1 = new TeamGame
                {
                    IsHomeTeam = true,
                    IsVisitorTeam = false,
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
                //return RedirectToAction("RedirectToTeamList", new { teamId });
                return RedirectToAction(nameof(Index));
            }

            ViewData["GameTypeID"] = new SelectList(_context.GameTypes, "ID", "Description", game.GameTypeID);
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "LocationName", game.LocationID);
            ViewData["SeasonID"] = new SelectList(_context.Seasons, "ID", "SeasonName", game.SeasonID);
            ViewData["TeamID"] = new SelectList(_context.Teams, "ID", "Name");
            //return RedirectToAction("Details", "Team", new { id = Team1 });
            return View(game);
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

            ViewData["GameTypeID"] = new SelectList(_context.GameTypes, "ID", "Description", game.GameTypeID);
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
        public async Task<IActionResult> Edit(int id, [Bind("ID,Date,Status,LocationID,SeasonID,GameTypeID")] Game game, int teamId, int team)
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
                //return RedirectToAction("RedirectToTeamList", new { teamId });
                return RedirectToAction(nameof(Index));
            }
            ViewData["GameTypeID"] = new SelectList(_context.GameTypes, "ID", "Description", game.GameTypeID);
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
                .Where(m => m.TeamID == team);
            var currentOptionIDs = _context.GameLineUps
                                            .Where(m => m.TeamID == team);
            var teamName = _context.Teams
                .Where(m => m.ID == team)
                .Select(m => m.Name)
                .FirstOrDefault();

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
                _context.SaveChanges();
            }
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.ID == id);
        }
    }
}
