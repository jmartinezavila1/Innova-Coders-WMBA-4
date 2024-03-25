using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using WMBA_4.Data;
using WMBA_4.Models;
using WMBA_4.Utilities;
using WMBA_4.ViewModels;

namespace WMBA_4.Controllers
{
    [Authorize]
    public class ScorePlayerController : Controller
    {
        private readonly WMBA_4_Context _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ScorePlayerController(WMBA_4_Context context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ScorePlayer
        public async Task<IActionResult> Index(int? TeamID, int? divisionID, int? LocationID, int? GameTypeID, DateTime GameDate, int? page, int? pageSizeID,
            string actionButton, string sortDirection = "asc", string sortField = "Location")
        {
            var userEmail = User.Identity.Name;

            List<int> teamIds = new List<int>();

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (await _userManager.IsInRoleAsync(user, "Coach"))
            {
                teamIds = await GetCoachTeamsAsync(userEmail);
            }
            else if (await _userManager.IsInRoleAsync(user, "Scorekeeper"))
            {
                teamIds = await GetScorekeeperTeamsAsync(userEmail);
            }
            else
            {
                var convenorDivisions = await GetConvenorDivisionsAsync(userEmail);
                teamIds = await _context.Teams
                    .Where(t => convenorDivisions.Contains(t.Division.DivisionName))
                    .Select(t => t.ID)
                    .ToListAsync();
            }

            IQueryable<Game> games = _context.Games
                .Include(g => g.GameType)
                .Include(g => g.Location)
                .Include(g => g.Season)
                .Include(tg => tg.TeamGames)
                    .ThenInclude(t => t.Team)
                        .ThenInclude(d => d.Division)
                .Where(s => s.Status == true && s.Date >= DateTime.Today && s.TeamGames.Any(tg => teamIds.Contains(tg.TeamID)))
                .OrderBy(a => a.Date);

            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;

            //sorting sortoption array
            string[] sortOptions = new[] { "Date" };

            //filter
            if (TeamID.HasValue)
            {
                games = games.Where(g => g.TeamGames.Any(t => t.TeamID == TeamID));
                numberFilters++;
            }
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
            if (LocationID.HasValue)
            {
                games = games.Where(g => g.LocationID == LocationID);
                numberFilters++;
            }
            if (GameDate >= DateTime.Today)
            {
                games = games.Where(g => g.Date >= GameDate);
                ViewData["GameDate"] = "03/02/2024";
                //ViewData["GameDate"] = DateTime.Now.ToString("yyyy-MM-dd");
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

            // Sorting
            if (!string.IsNullOrEmpty(actionButton))
            {
                page = 1; // Reset page to start

                if (sortOptions.Contains(actionButton))
                {
                    if (actionButton == sortField)
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton;
                }
            }
            if (sortField == "Date")
            {
                games = sortDirection == "asc" ? games.OrderBy(g => g.Date) : games.OrderByDescending(g => g.Date);
            }

            //Filter by DivisionName - TeamName
            ViewBag.TeamID = new SelectList(_context.Teams
                .Include(t => t.Division)
                .OrderBy(t => t.Division.ID)
                .ThenBy(t => t.Name)
                .Select(t => new {
                    t.ID,
                    TeamName = t.Division.DivisionName + " - " + t.Name
                }), "ID", "TeamName");

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName");
            ViewData["GameTypeID"] = new SelectList(_context.GameTypes.OrderBy(gt => gt.Description), "ID", "Description");
            ViewData["LocationID"] = new SelectList(_context.Locations.OrderBy(l => l.LocationName), "ID", "LocationName");

            // Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID);
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Game>.CreateAsync(games, page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: ScorePlayer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ScorePlayers == null)
            {
                return NotFound();
            }

            var scorePlayer = await _context.ScorePlayers
                .Include(s => s.GameLineUp)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (scorePlayer == null)
            {
                return NotFound();
            }

            return View(scorePlayer);
        }

        // GET: For selecting the team
        public async Task<IActionResult> SelectTeam(int? id)
        {
            var teams = await _context.TeamGame
                        .Include(t => t.Team)
                        .Where(t => t.GameID == id)
                        .Select(t => new { t.Team.ID, t.Team.Name })
                        .Distinct()
                        .ToListAsync();
            if (teams.Count == 0)
            {
                TempData["Message"] = "No Line Up for these teams.";
                return RedirectToAction("Index");
            }
            ViewData["TeamID"] = new SelectList(teams, "ID", "Name");
            ViewData["GameID"] = id;

            var model = new GameLineUp();

            return View(model);
        }

        // This method is for creating a new inning
        public async Task<IActionResult> Inning(int teamId, int gameId, int inplayId)
        {
            var inplay = _context.Inplays.Include(i => i.Inning).FirstOrDefault(i => i.ID == inplayId);

            var inning = new Inning
            {
                InningNumber = inplay.Inning.InningNumber + 1,
                TeamID = teamId,
                GameID = gameId
            };

            // Add the inning to the context and save the changes
            _context.Innings.Add(inning);
            await _context.SaveChangesAsync();

            //Refresh Inplay data           
            inplay.Runs = 0;
            inplay.Strikes = 0;
            inplay.Outs = 0;
            inplay.Fouls = 0;
            inplay.Balls = 0;
            inplay.InningID = inning.ID;
            inplay.NextPlayer = 0;
            inplay.PlayerBattingID = inplay.PlayerBattingID;
            inplay.PlayerInBase1ID = null;
            inplay.PlayerInBase2ID = null;
            inplay.PlayerInBase3ID = null;


            _context.Inplays.Update(inplay);
            await _context.SaveChangesAsync();

            var inplay2 = _context.Inplays
                        .Include(i => i.Inning).ThenInclude(inn => inn.Team)
                        .Include(i => i.Inning).ThenInclude(inn => inn.Game)
                        .Include(i => i.PlayerBatting)
                        .Where(i => i.InningID == inning.ID)
                        .FirstOrDefault();

            if (inplay2 == null)
            {
                return NotFound();
            }

            // Get the scores for each team
            var scores = _context.TeamGame
                .Where(tg => tg.GameID == gameId)
                .ToList();

            var playeratBat = _context.Players
              .Where(p => p.ID == inplay.PlayerBattingID)
              .Select(p => p.FullName)
              .FirstOrDefault();

            var inplayData = new
            {
                ID = inplay2.ID,
                Runs = scores.FirstOrDefault(s => s.IsHomeTeam == true)?.score,
                Runs2 = scores.FirstOrDefault(s => s.IsVisitorTeam == true)?.score,
                Strikes = inplay2.Strikes,
                Outs = inplay2.Outs,
                Fouls = inplay2.Fouls,
                Balls = inplay2.Balls,
                InningNumber = inplay2.Inning.InningNumber,
                FirstPlayer = playeratBat,
                Inning = inning.ScorePerInning
            };

            var lineupPlayers = _context.GameLineUps
                .Where(g => g.GameID == gameId && g.TeamID == teamId)
                .Select(p => p.ID)
                .ToList();

            foreach (var p in lineupPlayers)
            {
                var scoreplayer = new ScorePlayer
                {
                    GameLineUpID = p,
                };
                _context.ScorePlayers.Add(scoreplayer);

            }
            await _context.SaveChangesAsync();

            return Json(new { Inplay = inplayData });
        }

        // This method is for the Start Button
        public async void NextInning(int teamId, int gameId, int inplayId)
        {
            var inplay = _context.Inplays.Include(i => i.Inning).FirstOrDefault(i => i.ID == inplayId);


            var inning = new Inning
            {
                InningNumber = inplay.Inning.InningNumber + 1,
                TeamID = teamId,
                GameID = gameId
            };

            // Add the inning to the context and save the changes
            _context.Innings.Add(inning);
            await _context.SaveChangesAsync();


            //Refresh Inplay data           
            inplay.Runs = 0;
            inplay.Strikes = 0;
            inplay.Outs = 0;
            inplay.Fouls = 0;
            inplay.Balls = 0;
            inplay.InningID = inning.ID;
            inplay.NextPlayer = 0;
            inplay.PlayerBattingID = inplay.PlayerBattingID;
            inplay.PlayerInBase1ID = null;
            inplay.PlayerInBase2ID = null;
            inplay.PlayerInBase3ID = null;

            _context.Inplays.Update(inplay);
            await _context.SaveChangesAsync();


            var lineupPlayers = _context.GameLineUps
                .Where(g => g.GameID == gameId && g.TeamID == teamId)
                .Select(p => p.ID)
                .ToList();

            foreach (var p in lineupPlayers)
            {
                var scoreplayer = new ScorePlayer
                {
                    GameLineUpID = p,
                };
                _context.ScorePlayers.Add(scoreplayer);

            }
            await _context.SaveChangesAsync();

        }
        public async Task<IActionResult> Start(int teamId, int gameId)
        {
            try
            {
                // Buscar en la tabla `Innings` un `Inning` con el `GameID` y `TeamID` proporcionados
                var existingInning = _context.Innings
                    .Where(i => i.GameID == gameId && i.TeamID == teamId)
                    .OrderByDescending(i => i.InningNumber)
                    .FirstOrDefault();
                var inplay = new Inplay();
                object inplayData = null;
                string player = "";


                //Si existe, obtener el último `Inning` y consultar la tabla `Inplay`
                if (existingInning != null)
                {
                    inplay = _context.Inplays
                            .Include(i => i.Inning).ThenInclude(inn => inn.Team)
                            .Include(i => i.Inning).ThenInclude(inn => inn.Game)
                            //.Include(i => i.PlayerBatting)
                            .Where(i => i.InningID == existingInning.ID)
                            .FirstOrDefault();

                    //inplay = await _context.Inplays.FindAsync(inplayId);

                    if (inplay.PlayerBattingID == null)
                    {
                        inplay.PlayerBattingID = inplay.NextPlayer;
                        await _context.SaveChangesAsync();

                        player = _context.Players
                           .Where(p => p.ID == inplay.NextPlayer)
                           .Select(p => p.FullName)
                           .FirstOrDefault();
                    }
                    else
                    {
                        player = _context.Players
                           .Where(p => p.ID == inplay.PlayerBattingID)
                           .Select(p => p.FullName)
                           .FirstOrDefault();

                    }

                    // Adding players to the ScorePlayer table if they don't exist
                    var lineupPlayers = _context.GameLineUps
                        .Where(g => g.GameID == gameId && g.TeamID == teamId)
                        .Select(p => p.ID)
                        .ToList();

                    foreach (var p in lineupPlayers)
                    {
                        // Check if a ScorePlayer with the same GameLineUpID already exists
                        var existingScorePlayer = _context.ScorePlayers
                            .FirstOrDefault(sp => sp.GameLineUpID == p);

                        // If not, add a new ScorePlayer
                        if (existingScorePlayer == null)
                        {
                            var scoreplayer = new ScorePlayer
                            {
                                GameLineUpID = p,
                            };
                            _context.ScorePlayers.Add(scoreplayer);
                        }
                    }

                    // Save changes once after all ScorePlayers have been added
                    await _context.SaveChangesAsync();


                    if (inplay != null)
                    {
                        // Get the scores for each team
                        var scores = _context.TeamGame
                            .Where(tg => tg.GameID == gameId)
                            .ToList();


                        // Actualizar la información de la vista y retornar
                        inplayData = new
                        {
                            ID = inplay.ID,
                            Runs = scores.FirstOrDefault(s => s.IsHomeTeam == true)?.score,
                            Runs2 = scores.FirstOrDefault(s => s.IsVisitorTeam == true)?.score,
                            Strikes = inplay.Strikes,
                            Outs = inplay.Outs,
                            Fouls = inplay.Fouls,
                            Balls = inplay.Balls,
                            InningNumber = inplay.Inning.InningNumber,
                            FirstPlayer = player

                        };

                        return Json(new { Inplay = inplayData });
                    }
                }
                else
                {
                    if (!_context.GameLineUps.Any(g => g.GameID == gameId && g.TeamID == teamId))
                    {
                        return Json(new
                        {
                            success = false,
                            message = "It is necessary to make a lineup for this game."
                        });
                    }

                    // Creating a new inning cuando no existe

                    var inning = new Inning
                    {
                        InningNumber = 1,
                        TeamID = teamId,
                        GameID = gameId
                    };

                    // Add the inning to the context and save the changes
                    _context.Innings.Add(inning);
                    await _context.SaveChangesAsync();

                    // load the lineup data
                    var firstPlayer = _context.GameLineUps
                      .Include(p => p.Player)
                      .Where(g => g.GameID == gameId && g.TeamID == teamId && g.BattingOrder == 1)
                      .Select(p => p.Player)
                      .FirstOrDefault();

                    //load inplay data
                    inplay = new Inplay();
                    var scoreplayer = new ScorePlayer();

                    if (firstPlayer == null)
                    {
                        return NotFound();
                    }

                    inplay = new Inplay
                    {
                        InningID = inning.ID,
                        PlayerBattingID = firstPlayer.ID
                    };
                    _context.Inplays.Add(inplay);
                    await _context.SaveChangesAsync();

                    var inplay2 = _context.Inplays
                                .Include(i => i.Inning).ThenInclude(inn => inn.Team)
                                .Include(i => i.Inning).ThenInclude(inn => inn.Game)
                                .Include(i => i.PlayerBatting)
                                .Where(i => i.InningID == inning.ID)
                                .FirstOrDefault();



                    if (inplay2 == null)
                    {
                        return NotFound();
                    }

                    // Get the scores for each team
                    var scores = _context.TeamGame
                        .Where(tg => tg.GameID == gameId)
                        .ToList();


                    inplayData = new
                    {
                        ID = inplay2.ID,
                        Runs = scores.FirstOrDefault(s => s.IsHomeTeam == true)?.score,
                        Runs2 = scores.FirstOrDefault(s => s.IsVisitorTeam == true)?.score,
                        Strikes = inplay2.Strikes,
                        Outs = inplay2.Outs,
                        Fouls = inplay2.Fouls,
                        Balls = inplay2.Balls,
                        InningNumber = inplay2.Inning.InningNumber,
                        FirstPlayer = firstPlayer != null ? firstPlayer.FullName : "N/A"
                    };

                    //Adding players to the ScorePlayer table
                    var lineupPlayers = _context.GameLineUps
                        .Where(g => g.GameID == gameId && g.TeamID == teamId)
                        .Select(p => p.ID)
                        .ToList();

                    foreach (var p in lineupPlayers)
                    {
                        scoreplayer = new ScorePlayer
                        {
                            GameLineUpID = p,
                        };
                        _context.ScorePlayers.Add(scoreplayer);
                        await _context.SaveChangesAsync();
                    }



                }


                return Json(new { Inplay = inplayData });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

        }

        //Method to count Balls
        [HttpPost]
        public async Task<IActionResult> CountBalls(int inplayId)
        {

            object inplayData = null;
            // get the inplay record
            var inplay = await _context.Inplays.FindAsync(inplayId);
            var inning = await _context.Innings.FindAsync(inplay.InningID);

            if (inplay == null)
            {
                return NotFound();
            }
            if (inplay.PlayerBattingID == null)
            {
                return Json(new
                {
                    success = false,
                    message = "There was a problem selecting the batter"
                });
            }

            var player = _context.Players
                     .Where(p => p.ID == inplay.PlayerBattingID)
                     .Select(p => p.FullName)
                     .FirstOrDefault();

            var gameId = _context.Innings
                .Where(i => i.ID == inplay.InningID)
                .Select(i => i.GameID)
                .FirstOrDefault();

            // Get the scores for each team
            var scores = _context.TeamGame
                .Where(tg => tg.GameID == gameId)
                .ToList();

            var team = _context.Players
                .Include(p => p.Team).ThenInclude(d => d.Division)
                .Where(p => p.ID == inplay.PlayerBattingID)
                .Select(p => p.Team)
                .FirstOrDefault();

            var inningNumber = _context.Innings
                      .Where(i => i.ID == inplay.InningID)
                      .Select(i => i.InningNumber)
                      .FirstOrDefault();

            if (team.Division.DivisionName == "9U")
            {
                var playeratBat = _context.Players
               .Where(p => p.ID == inplay.PlayerBattingID)
               .Select(p => p.FullName)
               .FirstOrDefault();



                inplayData = new
                {
                    ID = inplay.ID,
                    Runs = scores.FirstOrDefault(s => s.IsHomeTeam == true)?.score,
                    Runs2 = scores.FirstOrDefault(s => s.IsVisitorTeam == true)?.score,
                    Strikes = inplay.Strikes,
                    Outs = inplay.Outs,
                    Fouls = inplay.Fouls,
                    Balls = inplay.Balls,
                    InningNumber = inningNumber,
                    FirstPlayer = playeratBat,
                };
                return Json(new { Inplay = inplayData });
            }

            inplay.Balls++;
            //if the balls are less than 4, increment the balls
            if (inplay.Balls <= 3)
            {

                // save changes in the the database
                await _context.SaveChangesAsync();


                inningNumber = _context.Innings
                      .Where(i => i.ID == inplay.InningID)
                      .Select(i => i.InningNumber)
                      .FirstOrDefault();

                var nextplayer = _context.Players
                    .Where(p => p.ID == inplay.PlayerBattingID)
                    .Select(p => p.FullName)
                    .FirstOrDefault();

                inplayData = new
                {
                    ID = inplay.ID,
                    Runs = scores.FirstOrDefault(s => s.IsHomeTeam == true)?.score,
                    Runs2 = scores.FirstOrDefault(s => s.IsVisitorTeam == true)?.score,
                    Strikes = inplay.Strikes,
                    Outs = inplay.Outs,
                    Fouls = inplay.Fouls,
                    Balls = inplay.Balls,
                    InningNumber = inningNumber,
                    FirstPlayer = nextplayer,
                };

                return Json(new { Inplay = inplayData });
            }

            //if the balls are 4, increment the PA and BB in ScorePlayer
            else
            {


                var lineupID = _context.GameLineUps
                    .Where(g => g.PlayerID == inplay.PlayerBattingID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                    .Select(g => g.ID)
                    .FirstOrDefault();

                var scorePlayer = await _context.ScorePlayers
                     .Where(sp => sp.GameLineUpID == lineupID)
                     .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
                     .FirstOrDefaultAsync();


                if (scorePlayer == null)
                {
                    return NotFound();
                }

                //// Update fields PA y BB
                scorePlayer.PA++;
                scorePlayer.BB++;
                scorePlayer.Balls = inplay.Balls;
                inplay.Strikes = 0;
                inplay.Fouls = 0;

                inplay.NextPlayer = (int)inplay.PlayerBattingID;

                await _context.SaveChangesAsync();

                MovePlayer(inplay);


                GetNextPlayer(inplayId);

                var playeratBat = _context.Players
                  .Where(p => p.ID == inplay.PlayerBattingID)
                  .Select(p => p.FullName)
                  .FirstOrDefault();

                inplayData = new
                {
                    ID = inplay.ID,
                    Runs = scores.FirstOrDefault(s => s.IsHomeTeam == true)?.score,
                    Runs2 = scores.FirstOrDefault(s => s.IsVisitorTeam == true)?.score,
                    Strikes = inplay.Strikes,
                    Outs = inplay.Outs,
                    Fouls = inplay.Fouls,
                    Balls = inplay.Balls,
                    InningNumber = inning.InningNumber,
                    FirstPlayer = playeratBat,
                };

                var innings = await _context.Innings
                  .Where(i => i.GameID == inning.GameID && i.TeamID == inning.TeamID)
                  .OrderBy(i => i.InningNumber)
                  .ToListAsync();

                var defTeam = _context.TeamGame
                 .Where(tg => tg.GameID == inning.GameID && tg.TeamID == inning.TeamID)
                 .Select(tg => tg.IsHomeTeam)
                 .FirstOrDefault();
                if (defTeam)
                {
                    ViewBag.InningScoresTeam2 = innings.ToDictionary(i => i.InningNumber, i => i.ScorePerInning);
                    ViewBag.InningScoresTeam1 = innings.ToDictionary(i => i.InningNumber, i => i.ScorePerInningOpponent);

                }
                else
                {
                    ViewBag.InningScoresTeam1 = innings.ToDictionary(i => i.InningNumber, i => i.ScorePerInning);
                    ViewBag.InningScoresTeam2 = innings.ToDictionary(i => i.InningNumber, i => i.ScorePerInningOpponent);

                }



                return Json(new { Inplay = inplayData, InningScoresTeam1 = ViewBag.InningScoresTeam1, InningScoresTeam2 = ViewBag.InningScoresTeam2 });

            }

        }

        //Method to count Strikes
        [HttpPost]
        public async Task<IActionResult> CountStrikes(int inplayId)
        {
            object inplayData = null;
            // get the inplay record
            var inplay = await _context.Inplays.FindAsync(inplayId);
            var inning = await _context.Innings.FindAsync(inplay.InningID);
            if (inplay == null)
            {
                return NotFound();
            }
            if (inplay.PlayerBattingID == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Click on Next Player to continue."
                });
            }

            inplay.Strikes++;
            var player = _context.Players
                     .Where(p => p.ID == inplay.PlayerBattingID)
                     .Select(p => p.FullName)
                     .FirstOrDefault();



            var inningNumber = _context.Innings
                      .Where(i => i.ID == inplay.InningID)
                      .Select(i => i.InningNumber)
                      .FirstOrDefault();

            await _context.SaveChangesAsync();


            //Validation for 9U teams
            var divisionTeam = _context.Teams
                .Include(d => d.Division)
                .Where(t => t.ID == inplay.Inning.TeamID)
                .Select(d => d.Division.DivisionName)
                .FirstOrDefault();

            var gameId = _context.Innings
                .Where(i => i.ID == inplay.InningID)
                .Select(i => i.GameID)
                .FirstOrDefault();

            // Get the scores for each team
            var scores = _context.TeamGame
                .Where(tg => tg.GameID == gameId)
                .ToList();


            //if the division is 9U, then the strikes are 5

            if (divisionTeam == "9U")
            {
                if (inplay.Strikes < 5)
                {


                    inplayData = new
                    {
                        ID = inplay.ID,
                        Runs = scores.FirstOrDefault(s => s.IsHomeTeam == true)?.score,
                        Runs2 = scores.FirstOrDefault(s => s.IsVisitorTeam == true)?.score,
                        Strikes = inplay.Strikes,
                        Outs = inplay.Outs,
                        Fouls = inplay.Fouls,
                        Balls = inplay.Balls,
                        InningNumber = inningNumber,
                        FirstPlayer = player,
                    };
                }

                //if the Strikes are 3, increment the PA and Strike Out in ScorePlayer
                else if (inplay.Strikes >= 5)
                {
                    var lineupID = _context.GameLineUps
                        .Where(g => g.PlayerID == inplay.PlayerBattingID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                        .Select(g => g.ID)
                        .FirstOrDefault();

                    var scorePlayer = await _context.ScorePlayers
                        .Where(sp => sp.GameLineUpID == lineupID)
                        .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
                        .FirstOrDefaultAsync();
                    //var scorePlayer = await _context.ScorePlayers
                    //    .Where(sp => sp.GameLineUpID == lineupID)
                    //    .FirstOrDefaultAsync();

                    GetNextPlayer(inplayId);

                    var playeratBat = _context.Players
                      .Where(p => p.ID == inplay.PlayerBattingID)
                      .Select(p => p.FullName)
                      .FirstOrDefault();

                    if (scorePlayer == null)
                    {
                        return NotFound();
                    }

                    //// update fields PA ad StrikeOut
                    scorePlayer.PA++;
                    scorePlayer.StrikeOut++;
                    scorePlayer.AB++;

                    inplay.Balls = 0;
                    inplay.Strikes = 0;
                    inplay.Fouls = 0;
                    inplay.Outs++;
                    inplay.NextPlayer = (int)inplay.PlayerBattingID;



                    // save changes to the database
                    await _context.SaveChangesAsync();

                    if (inplay.Outs == 3)
                    {
                        NextInning(inning.TeamID, inning.GameID, inplayId);

                    }
                    var inplay2 = await _context.Inplays.FindAsync(inplayId);

                    var inningNumber2 = _context.Innings
                           .Where(i => i.ID == inplay2.InningID)
                           .Select(i => i.InningNumber)
                           .FirstOrDefault();
                    //return a response
                    inplayData = new
                    {
                        ID = inplay2.ID,
                        Runs = scores.FirstOrDefault(s => s.IsHomeTeam == true)?.score,
                        Runs2 = scores.FirstOrDefault(s => s.IsVisitorTeam == true)?.score,
                        Strikes = inplay2.Strikes,
                        Outs = inplay2.Outs,
                        Fouls = inplay2.Fouls,
                        Balls = inplay2.Balls,
                        InningNumber = inningNumber2,
                        FirstPlayer = playeratBat
                    };

                }

                return Json(new { Inplay = inplayData });

            }

            //if the division is diferent to 9U, then the strikes are 3
            else
            {
                //if the strikes are less than 3, increment the strikes
                if (inplay.Strikes < 3)
                {

                    inplayData = new
                    {
                        ID = inplay.ID,
                        Runs = scores.FirstOrDefault(s => s.IsHomeTeam == true)?.score,
                        Runs2 = scores.FirstOrDefault(s => s.IsVisitorTeam == true)?.score,
                        Strikes = inplay.Strikes,
                        Outs = inplay.Outs,
                        Fouls = inplay.Fouls,
                        Balls = inplay.Balls,
                        InningNumber = inningNumber,
                        FirstPlayer = player,
                    };
                    //save changes to the database
                    await _context.SaveChangesAsync();
                }

                //if the Strikes are 3, increment the PA and Strike Out in ScorePlayer
                else if (inplay.Strikes >= 3)
                {

                    var lineupID = _context.GameLineUps
                        .Where(g => g.PlayerID == inplay.PlayerBattingID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                        .Select(g => g.ID)
                        .FirstOrDefault();


                    var scorePlayer = await _context.ScorePlayers
                        .Where(sp => sp.GameLineUpID == lineupID)
                        .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
                        .FirstOrDefaultAsync();
                    //var scorePlayer = await _context.ScorePlayers
                    //    .Where(sp => sp.GameLineUpID == lineupID)
                    //    .FirstOrDefaultAsync();

                    if (scorePlayer == null)
                    {
                        return NotFound();
                    }

                    //// update fields PA ad StrikeOut
                    scorePlayer.PA++;
                    scorePlayer.StrikeOut++;
                    scorePlayer.AB++;



                    inplay.Balls = 0;
                    inplay.Strikes = 0;
                    inplay.Fouls = 0;
                    inplay.Outs++;
                    inplay.NextPlayer = (int)inplay.PlayerBattingID;


                    //save changes to the database
                    await _context.SaveChangesAsync();

                    GetNextPlayer(inplayId);

                    var playeratBat = _context.Players
                      .Where(p => p.ID == inplay.PlayerBattingID)
                      .Select(p => p.FullName)
                      .FirstOrDefault();

                    if (inplay.Outs == 3)
                    {
                        NextInning(inning.TeamID, inning.GameID, inplayId);

                    }
                    var inplay2 = await _context.Inplays.FindAsync(inplayId);

                    var inningNumber2 = _context.Innings
                           .Where(i => i.ID == inplay2.InningID)
                           .Select(i => i.InningNumber)
                           .FirstOrDefault();

                    // return a response
                    inplayData = new
                    {
                        ID = inplay.ID,
                        Runs = scores.FirstOrDefault(s => s.IsHomeTeam == true)?.score,
                        Runs2 = scores.FirstOrDefault(s => s.IsVisitorTeam == true)?.score,
                        Strikes = inplay.Strikes,
                        Outs = inplay.Outs,
                        Fouls = inplay.Fouls,
                        Balls = inplay.Balls,
                        InningNumber = inningNumber,
                        FirstPlayer = playeratBat
                    };

                }

                return Json(new { Inplay = inplayData });
            }


        }

        //Method to count Fouls
        [HttpPost]
        public async Task<IActionResult> CountFouls(int inplayId)
        {
            object inplayData = null;
            // get the inplay record
            var inplay = await _context.Inplays.FindAsync(inplayId);
            if (inplay == null)
            {
                return NotFound();
            }

            var gameId = _context.Innings
                .Where(i => i.ID == inplay.InningID)
                .Select(i => i.GameID)
                .FirstOrDefault();

            // Get the scores for each team
            var scores = _context.TeamGame
                .Where(tg => tg.GameID == gameId)
                .ToList();

            var player = _context.Players
                    .Where(p => p.ID == inplay.PlayerBattingID)
                    .Select(p => p.FullName)
                    .FirstOrDefault();

            var inningNumber = _context.Innings
                 .Where(i => i.ID == inplay.InningID)
                 .Select(i => i.InningNumber)
                 .FirstOrDefault();

            //For 9U teams
            var team = _context.Players
                .Include(p => p.Team).ThenInclude(d => d.Division)
                .Where(p => p.ID == inplay.PlayerBattingID)
                .Select(p => p.Team)
                .FirstOrDefault();

            if (team.Division.DivisionName == "9U")
            {
                // Si el equipo está en la división 9U y los strikes son menos de 4, incrementa los fouls y los strikes
                if (inplay.Strikes < 4)
                {
                    inplay.Strikes++;
                    inplay.Fouls++;
                    // Guarda los cambios en la base de datos
                    await _context.SaveChangesAsync();

                    inplayData = new
                    {
                        ID = inplay.ID,
                        Runs = scores.FirstOrDefault(s => s.IsHomeTeam == true)?.score,
                        Runs2 = scores.FirstOrDefault(s => s.IsVisitorTeam == true)?.score,
                        Strikes = inplay.Strikes,
                        Outs = inplay.Outs,
                        Fouls = inplay.Fouls,
                        Balls = inplay.Balls,
                        InningNumber = inningNumber,
                        FirstPlayer = player,
                    };
                }
                // Si los strikes son 4 o más, solo incrementa los fouls
                else
                {
                    inplay.Fouls++;
                    // Guarda los cambios en la base de datos
                    await _context.SaveChangesAsync();

                    inplayData = new
                    {
                        ID = inplay.ID,
                        Runs = scores.FirstOrDefault(s => s.IsHomeTeam == true)?.score,
                        Runs2 = scores.FirstOrDefault(s => s.IsVisitorTeam == true)?.score,
                        Strikes = inplay.Strikes,
                        Outs = inplay.Outs,
                        Fouls = inplay.Fouls,
                        Balls = inplay.Balls,
                        InningNumber = inningNumber,
                        FirstPlayer = player,
                    };
                    var inning = await _context.Innings.FindAsync(inplay.InningID);

                    var lineupID = _context.GameLineUps
                         .Where(g => g.PlayerID == inplay.PlayerBattingID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                         .Select(g => g.ID)
                         .FirstOrDefault();

                    var scorePlayer = await _context.ScorePlayers
                        .Where(sp => sp.GameLineUpID == lineupID)
                        .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
                        .FirstOrDefaultAsync();

                    scorePlayer.Fouls++;
                    // Guarda los cambios en la base de datos
                    await _context.SaveChangesAsync();

                }
            }
            else
            {

                //For the rest of the divisions
                //if the stikes are less than 2, increment the fouls
                if (inplay.Strikes < 2)
                {
                    inplay.Strikes++;
                    inplay.Fouls++;
                    // Guarda los cambios en la base de datos
                    await _context.SaveChangesAsync();

                    inplayData = new
                    {
                        ID = inplay.ID,
                        Runs = scores.FirstOrDefault(s => s.IsHomeTeam == true)?.score,
                        Runs2 = scores.FirstOrDefault(s => s.IsVisitorTeam == true)?.score,
                        Strikes = inplay.Strikes,
                        Outs = inplay.Outs,
                        Fouls = inplay.Fouls,
                        Balls = inplay.Balls,
                        InningNumber = inningNumber,
                        FirstPlayer = player,
                    };
                }

                //if the Strikes are more than 2, just increment the fouls
                else if (inplay.Strikes >= 2)
                {
                    inplay.Fouls++;
                    // Guarda los cambios en la base de datos
                    await _context.SaveChangesAsync();

                    //var inningNumber = _context.Innings
                    //      .Where(i => i.ID == inplay.InningID)
                    //      .Select(i => i.InningNumber)
                    //      .FirstOrDefault();
                    inplayData = new
                    {
                        ID = inplay.ID,
                        Runs = scores.FirstOrDefault(s => s.IsHomeTeam == true)?.score,
                        Runs2 = scores.FirstOrDefault(s => s.IsVisitorTeam == true)?.score,
                        Strikes = inplay.Strikes,
                        Outs = inplay.Outs,
                        Fouls = inplay.Fouls,
                        Balls = inplay.Balls,
                        InningNumber = inningNumber,
                        FirstPlayer = player,
                    };

                    var inning = await _context.Innings.FindAsync(inplay.InningID);

                    var lineupID = _context.GameLineUps
                         .Where(g => g.PlayerID == inplay.PlayerBattingID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                         .Select(g => g.ID)
                         .FirstOrDefault();

                    var scorePlayer = await _context.ScorePlayers
                        .Where(sp => sp.GameLineUpID == lineupID)
                        .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
                        .FirstOrDefaultAsync();

                    scorePlayer.Fouls++;
                    // Guarda los cambios en la base de datos
                    await _context.SaveChangesAsync();

                }

            }

            return Json(new { Inplay = inplayData });

        }

        //Method for recording Hit by Pitch
        [HttpPost]
        public async Task<IActionResult> HitByPitch(int inplayId)
        {
            object inplayData = null;
            // get the inplay record
            var inplay = await _context.Inplays.FindAsync(inplayId);

            if (inplay == null)
            {
                return NotFound();
            }
            if (inplay.PlayerBattingID == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Click on Next Player to continue."
                });
            }

            var player = _context.Players
                     .Where(p => p.ID == inplay.PlayerBattingID)
                     .Select(p => p.FullName)
                     .FirstOrDefault();

            var inning = await _context.Innings.FindAsync(inplay.InningID);

            // Get the scores for each team
            var scores = _context.TeamGame
                .Where(tg => tg.GameID == inning.GameID)
                .ToList();

            var lineupID = _context.GameLineUps
                 .Where(g => g.PlayerID == inplay.PlayerBattingID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                 .Select(g => g.ID)
                 .FirstOrDefault();

            var scorePlayer = await _context.ScorePlayers
                .Where(sp => sp.GameLineUpID == lineupID)
                .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
                .FirstOrDefaultAsync();

            if (scorePlayer == null)
            {
                return NotFound();
            }

            //// Actualiza los campos PA y BB
            scorePlayer.PA++;
            scorePlayer.HBP++;



            inplay.Balls = 0;
            inplay.Strikes = 0;
            inplay.Fouls = 0;

            var inningNumber = _context.Innings
                       .Where(i => i.ID == inplay.InningID)
                       .Select(i => i.InningNumber)
                       .FirstOrDefault();

            MovePlayer(inplay);


            GetNextPlayer(inplayId);

            var playeratBat = _context.Players
              .Where(p => p.ID == inplay.PlayerBattingID)
              .Select(p => p.FullName)
              .FirstOrDefault();

            // Devuelve una respuesta
            inplayData = new
            {
                ID = inplay.ID,
                Runs = scores.FirstOrDefault(s => s.IsHomeTeam == true)?.score,
                Runs2 = scores.FirstOrDefault(s => s.IsVisitorTeam == true)?.score,
                Strikes = inplay.Strikes,
                Outs = inplay.Outs,
                Fouls = inplay.Fouls,
                Balls = inplay.Balls,
                InningNumber = inningNumber,
                FirstPlayer = playeratBat
            };


            var innings = await _context.Innings
              .Where(i => i.GameID == inning.GameID && i.TeamID == inning.TeamID)
              .OrderBy(i => i.InningNumber)
              .ToListAsync();

            var defTeam = _context.TeamGame
                .Where(tg => tg.GameID == inning.GameID && tg.TeamID == inning.TeamID)
                .Select(tg => tg.IsHomeTeam)
                .FirstOrDefault();
            if (defTeam)
            {
                ViewBag.InningScoresTeam2 = innings.ToDictionary(i => i.InningNumber, i => i.ScorePerInning);
                ViewBag.InningScoresTeam1 = innings.ToDictionary(i => i.InningNumber, i => i.ScorePerInningOpponent);

            }
            else
            {
                ViewBag.InningScoresTeam1 = innings.ToDictionary(i => i.InningNumber, i => i.ScorePerInning);
                ViewBag.InningScoresTeam2 = innings.ToDictionary(i => i.InningNumber, i => i.ScorePerInningOpponent);

            }


            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();



            return Json(new { Inplay = inplayData, InningScoresTeam1 = ViewBag.InningScoresTeam1, InningScoresTeam2 = ViewBag.InningScoresTeam2 });

        }

        //Method to record an Out
        [HttpPost]
        public async Task<IActionResult> Out(int inplayId)
        {
            object inplayData = null;
            // get the inplay record
            var inplay = await _context.Inplays.FindAsync(inplayId);
            if (inplay == null)
            {
                return NotFound();
            }
            if (inplay.PlayerBattingID == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Click on Next Player to continue."
                });
            }

            var inning = await _context.Innings.FindAsync(inplay.InningID);

            // Get the scores for each team
            var scores = _context.TeamGame
                .Where(tg => tg.GameID == inning.GameID)
                .ToList();


            var lineupID = _context.GameLineUps
                .Where(g => g.PlayerID == inplay.PlayerBattingID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                .Select(g => g.ID)
                .FirstOrDefault();

            var scorePlayer = await _context.ScorePlayers
                 .Where(sp => sp.GameLineUpID == lineupID)
                 .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
                 .FirstOrDefaultAsync();

            if (scorePlayer == null)
            {
                return NotFound();
            }

            //// update fields PA ad StrikeOut
            scorePlayer.PA++;
            scorePlayer.Out++;
            scorePlayer.AB++;


            //inplay.NextPlayer = (int)inplay.PlayerBattingID;
            inplay.Balls = 0;
            inplay.Strikes = 0;
            inplay.Fouls = 0;
            inplay.Outs++;
            await _context.SaveChangesAsync();


            GetNextPlayer(inplayId);

            var playeratBat = _context.Players
              .Where(p => p.ID == inplay.PlayerBattingID)
              .Select(p => p.FullName)
              .FirstOrDefault();

            if (inplay.Outs == 3)
            {
                NextInning(inning.TeamID, inning.GameID, inplayId);

            }

            var inplay2 = await _context.Inplays.FindAsync(inplayId);

            var inningNumber = _context.Innings
                   .Where(i => i.ID == inplay2.InningID)
                   .Select(i => i.InningNumber)
                   .FirstOrDefault();
            // Devuelve una respuesta
            inplayData = new
            {
                ID = inplay2.ID,
                Runs = scores.FirstOrDefault(s => s.IsHomeTeam == true)?.score,
                Runs2 = scores.FirstOrDefault(s => s.IsVisitorTeam == true)?.score,
                Strikes = inplay2.Strikes,
                Outs = inplay2.Outs,
                Fouls = inplay2.Fouls,
                Balls = inplay2.Balls,
                InningNumber = inningNumber,
                FirstPlayer = playeratBat,
            };
            //inplay.Strikes = 0;
            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();


            return Json(new { Inplay = inplayData });

        }


        //This method is for the Next Batter button
        [HttpPost]
        public async Task<IActionResult> NextBatter(int inplayId)
        {
            object inplayData = null;
            // get the inplay record
            var inplay = await _context.Inplays.FindAsync(inplayId);
            if (inplay == null)
            {
                return NotFound();
            }

            var inning = await _context.Innings.FindAsync(inplay.InningID);

            // Get the scores for each team
            var scores = _context.TeamGame
                .Where(tg => tg.GameID == inning.GameID)
                .ToList();

            var lineupID = _context.GameLineUps
                .Where(g => g.PlayerID == inplay.NextPlayer && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                .Select(g => g.ID)
                .FirstOrDefault();

            var scorePlayer = await _context.ScorePlayers
                .Where(sp => sp.GameLineUpID == lineupID)
                .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
                .FirstOrDefaultAsync();

            if (scorePlayer == null)
            {
                return NotFound();
            }

            var nextBO = _context.GameLineUps
                .Where(g => g.GameID == inplay.Inning.GameID && g.TeamID == inplay.Inning.TeamID && g.PlayerID == inplay.NextPlayer)
                .Select(g => g.BattingOrder)
                .FirstOrDefault();

            var nextPlayer = _context.GameLineUps
                .Where(g => g.GameID == inplay.Inning.GameID && g.TeamID == inplay.Inning.TeamID && g.BattingOrder == nextBO + 1)
                .Select(g => g.Player)
                .FirstOrDefault();

            // if nextPlayer es null, then get the first player in the lineup
            if (nextPlayer == null)
            {
                nextPlayer = _context.GameLineUps
                    .Where(g => g.GameID == inplay.Inning.GameID && g.TeamID == inplay.Inning.TeamID)
                    .OrderBy(g => g.BattingOrder)
                    .Select(g => g.Player)
                    .FirstOrDefault();
            }

            inplay.PlayerBattingID = nextPlayer.ID;
            inplay.Balls = 0;
            inplay.Strikes = 0;
            inplay.Fouls = 0;

            var inningNumber = _context.Innings
                   .Where(i => i.GameID == inplay.Inning.GameID && i.TeamID == inplay.Inning.TeamID)
                   .Select(i => i.InningNumber)
                   .FirstOrDefault();
            // Devuelve una respuesta
            inplayData = new
            {
                ID = inplay.ID,
                Runs = scores.FirstOrDefault(s => s.IsHomeTeam == true)?.score,
                Runs2 = scores.FirstOrDefault(s => s.IsVisitorTeam == true)?.score,
                Strikes = inplay.Strikes,
                Outs = inplay.Outs,
                Fouls = inplay.Fouls,
                Balls = inplay.Balls,
                InningNumber = inningNumber,
                FirstPlayer = nextPlayer != null ? nextPlayer.FullName : "N/A"
            };

            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();


            return Json(new { Inplay = inplayData });

        }

        // GET: First screen of ScorePlayer
        public async Task<IActionResult> ScoreKeeping(int GameID, int TeamID)
        {
            var teams = await _context.GameLineUps
                .Include(t => t.Team)
                .Include(tg => tg.Game).ThenInclude(tm => tm.TeamGames)
                .Where(t => t.GameID == GameID)
                .Select(t => new { t.Team.ID, t.Team.Name })
                .Distinct()
                .ToListAsync();

            var teamDivision = _context.TeamGame
                .Include(tg => tg.Game)
                .Include(t => t.Team)
                .ThenInclude(t => t.Division)
                .Where(t => t.GameID == GameID)
                .ToList();

            var inPlay = await _context.Inplays
                .Include(i => i.Inning)
                .Where(s => s.Inning.GameID == GameID && s.Inning.TeamID == TeamID)
                .ToListAsync();

            // Get the scores for each team
            var scores = _context.TeamGame
                .Include(tg => tg.Team)
                .ThenInclude(t=>t.Division)
                .Where(tg => tg.GameID == GameID)
                .ToList();

            var TeamScoring = _context.Teams
                .Where(t => t.ID == TeamID)
                .Select(t => t.Name)
                .FirstOrDefault();

            var innings = await _context.Innings
                .Where(i => i.GameID == GameID)
                .OrderBy(i => i.InningNumber)
                .ToListAsync();

            var lineup = _context.GameLineUps
                .Include(gl => gl.Team)
                .ThenInclude(t => t.Players)
                .Include(gl => gl.Game)
                .Where(gl => gl.TeamID == TeamID && gl.GameID == GameID)
                .ToList();

            var inningScore = await _context.Innings
                .Where(i => i.GameID == GameID)
                .OrderBy(i => i.InningNumber)
                .ToListAsync();

            var teamInnings = await _context.Innings
                   .Where(i => i.GameID == GameID && i.TeamID == TeamID)
                   .OrderBy(i => i.InningNumber)
                   .ToListAsync();

            var defTeam = _context.TeamGame
            .Where(tg => tg.GameID == GameID && tg.TeamID == TeamID)
            .Select(tg => tg.IsHomeTeam)
            .FirstOrDefault();

            var playerCount = lineup.Sum(gl => gl.Team.Players.Count);
            // Store the scores in ViewBag
            ViewBag.Team1Score = scores.FirstOrDefault(s => s.IsHomeTeam == true)?.score;
            ViewBag.Team2Score = scores.FirstOrDefault(s => s.IsVisitorTeam == true)?.score;
            ViewBag.Team1 = scores.FirstOrDefault(s => s.IsHomeTeam == true)?.Team.Name;
            ViewBag.Team2 = scores.FirstOrDefault(s => s.IsVisitorTeam == true)?.Team.Name;
            ViewBag.Team1Division = teamDivision.FirstOrDefault(t => t.TeamID == TeamID)?.Team.Division.DivisionName;
            ViewBag.Team2Division = teamDivision.FirstOrDefault(t => t.TeamID != TeamID)?.Team.Division.DivisionName;

            if (defTeam)
            {
                ViewBag.InningScoresH = innings.ToDictionary(i => i.InningNumber, i => i.ScorePerInning);
                ViewBag.InningScoresA = innings.ToDictionary(i => i.InningNumber, i => i.ScorePerInningOpponent);
            }
            else
            {
                ViewBag.InningScoresA = innings.ToDictionary(i => i.InningNumber, i => i.ScorePerInning);
                ViewBag.InningScoresH = innings.ToDictionary(i => i.InningNumber, i => i.ScorePerInningOpponent);

            }
            //ViewBag.InningScores = innings.ToDictionary(i => i.InningNumber, i => i.ScorePerInning);
            ViewBag.PlayerCount = playerCount;

            ViewData["AllTeams"] = scores;
            ViewData["Teams"] = teams;
            ViewData["InPlay"] = inPlay;
            ViewBag.Inplay = new Inplay();
            ViewBag.TeamID = TeamID;
            ViewBag.TeamScoring = TeamScoring;
            ViewBag.GameID = GameID;

            return View();
        }


        // GET: Lineup
        public async Task<IActionResult> GetLineup(int gameId, int teamId)
        {
            var lineup = await _context.GameLineUps
                .Include(gl => gl.Player)
                .Where(gl => gl.GameID == gameId && gl.TeamID == teamId)
                .ToListAsync();

            return PartialView("_LineupView", lineup);
        }

        // Method to load the inplay partial view
        public async Task<IActionResult> LoadInplayPartial(int inplayId)
        {
            // get the inplay record
            var inplay = await _context.Inplays.FindAsync(inplayId);
            if (inplay == null)
            {
                return NotFound();
            }

            // get the players
            var playerBatting = await _context.Players.FindAsync(inplay.PlayerBattingID);
            var playerInBase1 = await _context.Players.FindAsync(inplay.PlayerInBase1ID);
            var playerInBase2 = await _context.Players.FindAsync(inplay.PlayerInBase2ID);
            var playerInBase3 = await _context.Players.FindAsync(inplay.PlayerInBase3ID);


            var inplayData = new InPlayMV
            {
                InplayID = inplayId,
                PlayerBattingId = inplay.PlayerBattingID,
                PlayerBattingName = playerBatting != null ? playerBatting.FullName : "N/A",
                PlayerInBase1Id = inplay.PlayerInBase1ID,
                PlayerInBase1Name = playerInBase1 != null ? playerInBase1.FullName : "N/A",
                PlayerInBase2Id = inplay.PlayerInBase2ID,
                PlayerInBase2Name = playerInBase2 != null ? playerInBase2.FullName : "N/A",
                PlayerInBase3Id = inplay.PlayerInBase3ID,
                PlayerInBase3Name = playerInBase3 != null ? playerInBase3.FullName : "N/A",
                PlayerInBase1Base = inplay.PlayerInBase1ID.HasValue ? 1 : (int?)null,
                PlayerInBase2Base = inplay.PlayerInBase2ID.HasValue ? 2 : (int?)null,
                PlayerInBase3Base = inplay.PlayerInBase3ID.HasValue ? 3 : (int?)null,

            };


            return PartialView("_InplayPartial", inplayData);
        }

        // This is the method for the Save button in the popup
        [HttpPost]
        public async Task<IActionResult> SaveInplayChanges([FromBody] JsonElement jsonElement)
        {
            try
            {
                var model = System.Text.Json.JsonSerializer.Deserialize<InPlayMV>(jsonElement.GetRawText());

                // Obtén el registro Inplay de la base de datos
                var inplay = await _context.Inplays.FindAsync(model.InplayID);
                var inning = await _context.Innings.FindAsync(inplay.InningID);

                //Information of the player batting
                var lineupID = _context.GameLineUps
                .Where(g => g.PlayerID == inplay.PlayerBattingID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                .Select(g => g.ID)
                .FirstOrDefault();

                var scorePlayer = await _context.ScorePlayers
                .Where(sp => sp.GameLineUpID == lineupID)
                .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
                .FirstOrDefaultAsync();

                int lineupIDP1;
                int lineupIDP2;
                int lineupIDP3;
                int lIDP1;
                int lIDP2;
                int lIDP3;
                ScorePlayer scorePlayerP1;
                ScorePlayer scorePlayerP2;
                ScorePlayer scorePlayerP3;
                ScorePlayer sppP1;
                ScorePlayer sppP2;
                ScorePlayer sppP3;
                int pb1 = (int)model.PlayerInBase1Id;
                int pb2 = (int)model.PlayerInBase2Id;
                int pb3 = (int)model.PlayerInBase3Id;

                int baseNum = 0;

                if (inplay == null)
                {
                    return NotFound();
                }

                var teamGameScore = await _context.TeamGame
                       .Where(tg => tg.GameID == inning.GameID && tg.TeamID == inning.TeamID)
                       .FirstOrDefaultAsync();

                //Saving information for Runs

                if (model.IsRunPlayer1)
                {
                    lineupIDP1 = _context.GameLineUps
                        .Where(g => g.PlayerID == inplay.PlayerInBase1ID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                        .Select(g => g.ID)
                        .FirstOrDefault();

                    scorePlayerP1 = await _context.ScorePlayers
                          .Where(sp => sp.GameLineUpID == lineupIDP1)
                          .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
                          .FirstOrDefaultAsync();


                    scorePlayerP1.Run++;
                    inplay.Runs++;
                    teamGameScore.score++;
                    inplay.PlayerInBase1ID = null;
                    inning.ScorePerInning++;
                }
                if (model.IsRunPlayer2)
                {
                    lineupIDP2 = _context.GameLineUps
                       .Where(g => g.PlayerID == inplay.PlayerInBase2ID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                       .Select(g => g.ID)
                       .FirstOrDefault();

                    scorePlayerP2 = await _context.ScorePlayers
                          .Where(sp => sp.GameLineUpID == lineupIDP2)
                          .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
                          .FirstOrDefaultAsync();

                    scorePlayerP2.Run++;
                    inplay.Runs++;
                    teamGameScore.score++;
                    inplay.PlayerInBase2ID = null;
                    inning.ScorePerInning++;
                }
                if (model.IsRunPlayer3)
                {
                    lineupIDP3 = _context.GameLineUps
                       .Where(g => g.PlayerID == inplay.PlayerInBase3ID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                       .Select(g => g.ID)
                       .FirstOrDefault();

                    scorePlayerP3 = await _context.ScorePlayers
                          .Where(sp => sp.GameLineUpID == lineupIDP3)
                          .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
                          .FirstOrDefaultAsync();

                    scorePlayerP3.Run++;
                    inplay.Runs++;
                    inning.ScorePerInning++;
                    teamGameScore.score++;
                    inplay.PlayerInBase3ID = null;
                }



                //Saving infromation for Outs

                if (model.IsOutPlayer1)
                {
                    lIDP1 = _context.GameLineUps
                        .Where(g => g.PlayerID == inplay.PlayerInBase1ID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                        .Select(g => g.ID)
                        .FirstOrDefault();

                    sppP1 = await _context.ScorePlayers
                          .Where(sp => sp.GameLineUpID == lIDP1)
                          .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
                          .FirstOrDefaultAsync();

                    sppP1.Out++;
                    inplay.Outs++;
                    inplay.PlayerInBase1ID = null;
                }
                if (model.IsOutPlayer2)
                {
                    lIDP2 = _context.GameLineUps
                        .Where(g => g.PlayerID == inplay.PlayerInBase2ID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                        .Select(g => g.ID)
                        .FirstOrDefault();

                    sppP2 = await _context.ScorePlayers
                          .Where(sp => sp.GameLineUpID == lIDP2)
                          .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
                          .FirstOrDefaultAsync();

                    sppP2.Out++;
                    inplay.Outs++;
                    inplay.PlayerInBase2ID = null;
                }
                if (model.IsOutPlayer3)
                {
                    lIDP3 = _context.GameLineUps
                        .Where(g => g.PlayerID == inplay.PlayerInBase3ID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                        .Select(g => g.ID)
                        .FirstOrDefault();

                    sppP3 = await _context.ScorePlayers
                          .Where(sp => sp.GameLineUpID == lIDP3)
                          .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
                          .FirstOrDefaultAsync();

                    sppP3.Out++;
                    inplay.Outs++;
                    inplay.PlayerInBase3ID = null;
                }


                //Saving options selected for players in base
                inplay.PlayerInBase1ID = null;
                inplay.PlayerInBase2ID = null;
                inplay.PlayerInBase3ID = null;

                if (model.PlayerInBase1Base == 1)
                {
                    inplay.PlayerInBase1ID = pb1;
                }
                else if (model.PlayerInBase1Base == 2)
                {
                    inplay.PlayerInBase2ID = pb1;
                }
                else if (model.PlayerInBase1Base == 3)
                {
                    inplay.PlayerInBase3ID = pb1;
                }




                if (model.PlayerInBase2Base == 1)
                {
                    inplay.PlayerInBase1ID = pb2;
                }
                else if (model.PlayerInBase2Base == 2)
                {
                    inplay.PlayerInBase2ID = pb2;
                }
                else if (model.PlayerInBase2Base == 3)
                {
                    inplay.PlayerInBase3ID = pb2;
                }



                if (model.PlayerInBase3Base == 1)
                {
                    inplay.PlayerInBase1ID = pb3;
                }
                else if (model.PlayerInBase3Base == 2)
                {
                    inplay.PlayerInBase2ID = pb3;
                }
                else if (model.PlayerInBase3Base == 3)
                {
                    inplay.PlayerInBase3ID = pb3;
                }

                //Saving option selected for player batting

                if (model.PlayerBattingBase == 1)
                {
                    inplay.PlayerInBase1ID = model.PlayerBattingId;
                    baseNum = 1;
                    inplay.NextPlayer = (int)model.PlayerBattingId;
                    inplay.PlayerBattingID = null;
                }
                else if (model.PlayerBattingBase == 2)
                {
                    inplay.PlayerInBase2ID = model.PlayerBattingId;
                    baseNum = 2;
                    inplay.NextPlayer = (int)model.PlayerBattingId;
                    inplay.PlayerBattingID = null;
                }
                else if (model.PlayerBattingBase == 3)
                {
                    inplay.PlayerInBase3ID = model.PlayerBattingId;
                    baseNum = 3;
                    inplay.NextPlayer = (int)model.PlayerBattingId;
                    inplay.PlayerBattingID = null;
                }

                //This is for saving the information abot hits

                if (model.IsHit)
                {
                    if (baseNum == 1)
                    {
                        scorePlayer.Singles++;
                    }
                    else if (baseNum == 2)
                    {
                        scorePlayer.Doubles++;
                    }
                    else if (baseNum == 3)
                    {
                        scorePlayer.Triples++;
                    }
                    scorePlayer.H++;
                    scorePlayer.PA++;
                    scorePlayer.AB++;

                }

                //This is for saving the information about Home Run

                if (model.IsHomerun)
                {
                    scorePlayer.HR++;
                    scorePlayer.PA++;
                    scorePlayer.AB++;
                    inplay.PlayerBattingID = null;

                }

                //This is for saving the RBI´s

                if (model.IsRBI > 0)
                {
                    scorePlayer.RBI += model.IsRBI;
                    //scorePlayer.PA++;
                    //scorePlayer.AB++;
                    inplay.PlayerBattingID = null;
                    inplay.Runs++;
                    inning.ScorePerInning++;

                }


                inplay.Balls = 0;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {


                    return Json(new { success = false });
                }

                //Obtiene los datos actualizados
                object inplayData = null;

                // Get the scores for each team
                var scores = _context.TeamGame
                    .Where(tg => tg.GameID == inning.GameID)
                    .ToList();

                var playeratBat = _context.Players
              .Where(p => p.ID == inplay.PlayerBattingID)
              .Select(p => p.FullName)
              .FirstOrDefault();

                //validate if there is more than 3 outs
                if (inplay.Outs == 3)
                {
                    NextInning(inning.TeamID, inning.GameID, inplay.ID);

                }
                var inplay2 = await _context.Inplays.FindAsync(inplay.ID);

                var inningNumber2 = _context.Innings
                       .Where(i => i.ID == inplay2.InningID)
                       .Select(i => i.InningNumber)
                       .FirstOrDefault();

                inplayData = new
                {
                    ID = inplay2.ID,
                    Runs = scores.FirstOrDefault(s => s.IsHomeTeam == true)?.score,
                    Runs2 = scores.FirstOrDefault(s => s.IsVisitorTeam == true)?.score,
                    Strikes = inplay2.Strikes,
                    Outs = inplay2.Outs,
                    Fouls = inplay2.Fouls,
                    Balls = inplay2.Balls,
                    InningNumber = inningNumber2,
                    FirstPlayer = playeratBat,
                };


                return Json(new { success = true, inplay = inplayData });

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }

        }

        //Mothod to Close popup and update InPlay   
        [HttpPost]
        public async Task<IActionResult> UpdateInplay(int inplayId)
        {
            object inplayData = null;
            // get the inplay record
            var inplay = await _context.Inplays.FindAsync(inplayId);
            if (inplay == null)
            {
                return NotFound();
            }

            var gameId = _context.Innings
                .Where(i => i.ID == inplay.InningID)
                .Select(i => i.GameID)
                .FirstOrDefault();

            // Get the scores for each team
            var scores = _context.TeamGame
                .Where(tg => tg.GameID == gameId)
                .ToList();


            inplay.Balls = 0;
            inplay.Strikes = 0;
            inplay.Fouls = 0;


            // Devuelve una respuesta
            inplayData = new
            {
                ID = inplay.ID,
                Runs = scores.FirstOrDefault(s => s.IsHomeTeam == true)?.score,
                Runs2 = scores.FirstOrDefault(s => s.IsVisitorTeam == true)?.score,
                Strikes = inplay.Strikes,
                Outs = inplay.Outs,
                Fouls = inplay.Fouls,
                Balls = inplay.Balls,
                FirstPlayer = inplay.PlayerBatting.FullName
            };

            return Json(new { Inplay = inplayData });

        }

        public async void GetNextPlayer(int inplayId)
        {

            // get the inplay record
            var inplay = await _context.Inplays.FindAsync(inplayId);
            if (inplay == null)
            {
                return;
            }
            var inning = await _context.Innings.FindAsync(inplay.InningID);

            var lineupID = _context.GameLineUps
                .Where(g => g.PlayerID == inplay.PlayerBattingID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                .Select(g => g.ID)
                .FirstOrDefault();


            var nextBO = _context.GameLineUps
                .Where(g => g.GameID == inplay.Inning.GameID && g.TeamID == inplay.Inning.TeamID && g.PlayerID == inplay.PlayerBattingID)
                .Select(g => g.BattingOrder)
                .FirstOrDefault();

            var nextPlayer = _context.GameLineUps
                .Where(g => g.GameID == inplay.Inning.GameID && g.TeamID == inplay.Inning.TeamID && g.BattingOrder == nextBO + 1)
                .Select(g => g.Player)
                .FirstOrDefault();

            // if nextPlayer is null, then get the first player in the lineup
            if (nextPlayer == null)
            {
                nextPlayer = _context.GameLineUps
                    .Where(g => g.GameID == inplay.Inning.GameID && g.TeamID == inplay.Inning.TeamID)
                    .OrderBy(g => g.BattingOrder)
                    .Select(g => g.Player)
                    .FirstOrDefault();
            }

            inplay.PlayerBattingID = nextPlayer.ID;
            inplay.Balls = 0;
            inplay.Strikes = 0;
            inplay.Fouls = 0;
            await _context.SaveChangesAsync();
        }

        public async void MovePlayer(Inplay inplay)
        {
            int? p1 = (int?)inplay.PlayerInBase1ID;
            int? p2 = (int?)inplay.PlayerInBase2ID;
            int? p3 = (int?)inplay.PlayerInBase3ID;
            int? b = (int?)inplay.PlayerBattingID;

            int? newP1 = null;
            int? newP2 = null;
            int? newP3 = null;

            var inning = await _context.Innings.FindAsync(inplay.InningID);

            //Information of the player at 3er base
            var lineupID = _context.GameLineUps
            .Where(g => g.PlayerID == inplay.PlayerBattingID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
            .Select(g => g.ID)
            .FirstOrDefault();

            var scorePlayer = await _context.ScorePlayers
            .Where(sp => sp.GameLineUpID == lineupID)
            .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
            .FirstOrDefaultAsync();


            var teamGameScore = await _context.TeamGame
                      .Where(tg => tg.GameID == inning.GameID && tg.TeamID == inning.TeamID)
                      .FirstOrDefaultAsync();


            if (b.HasValue)
            {
                newP1 = b;
            }

            if (p1.HasValue && newP1.HasValue)
            {
                newP2 = p1;
            }

            if (p2.HasValue && newP2.HasValue)
            {
                newP3 = p2;
                inplay.Runs++;
                teamGameScore.score++;
                //register runs
                scorePlayer.Run++;
                inning.ScorePerInning++;

            }
            else if (p3.HasValue)
            {
                newP3 = p3;
            }

            inplay.PlayerInBase1ID = newP1;
            inplay.PlayerInBase2ID = newP2;
            inplay.PlayerInBase3ID = newP3;


        }

        [HttpPost]
        public async Task<IActionResult> Exit(int id, string exitType)
        {
            var inplay = await _context.Inplays.FindAsync(id);

            var GameID = await _context.Innings
                .Where(i => i.ID == inplay.InningID)
                .Select(i => i.GameID)
                .FirstOrDefaultAsync();

            var Game = await _context.Games.FindAsync(GameID);

            if (inplay == null)
            {
                return NotFound();
            }

            if (exitType == "End Game")
            {
                //Mark the Inplay for deletion

                _context.Inplays.Remove(inplay);
                Game.Status = false;

                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            if (exitType == "Cancel Game")
            {
                var scoreplayers = await _context.ScorePlayers
                    .Where(sp => sp.GameLineUp.GameID == GameID)
                    .ToListAsync();

                var innings = await _context.Innings
                    .Where(i => i.GameID == GameID)
                    .ToListAsync();


                _context.Inplays.Remove(inplay);
                _context.ScorePlayers.RemoveRange(scoreplayers);
                _context.Innings.RemoveRange(innings);
                Game.Status = false;

                // Save changes to the database
                await _context.SaveChangesAsync();


            }

            return Json(new { redirectTo = Url.Action("Index", "ScorePlayer") });
        }

        [HttpPost]
        public async Task<IActionResult> Hit(int id, string hitType)
        {
            var inplay = await _context.Inplays.FindAsync(id);

            if (inplay == null)
            {
                return NotFound();
            }
            bool needUserDecision = false;
            string message = "";

            int? pb1 = (int?)inplay.PlayerInBase1ID;
            int? pb2 = (int?)inplay.PlayerInBase2ID;
            int? pb3 = (int?)inplay.PlayerInBase3ID;
            int? pb = (int?)inplay.PlayerBattingID;

            int? newP1 = null;
            int? newP2 = null;
            int? newP3 = null;
            int? newPb = null;
            int rbi = 0;

            var inning = await _context.Innings.FindAsync(inplay.InningID);

            var innings = await _context.Innings
             .Where(i => i.GameID == inning.GameID && i.TeamID == inning.TeamID)
             .OrderBy(i => i.InningNumber)
             .ToListAsync();


            var teamGameScore = await _context.TeamGame
                  .Where(tg => tg.GameID == inning.GameID && tg.TeamID == inning.TeamID)
                  .FirstOrDefaultAsync();

            if (hitType == "Single")
            {
                if (pb3.HasValue)
                {
                    var player = _context.Players
                     .Where(p => p.ID == pb3)
                     .Select(p => p.FullName)
                     .FirstOrDefault();

                    needUserDecision = true;
                    message = player + " was in base 3.What do you want to register?";

                }
                // Move the players
                newP3 = pb2;
                newP2 = pb1;
                newP1 = pb;
                newPb = null;

                var lineupID = _context.GameLineUps
                .Where(g => g.PlayerID == inplay.PlayerBattingID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                .Select(g => g.ID)
                .FirstOrDefault();

                var scorePlayer = await _context.ScorePlayers
               .Where(sp => sp.GameLineUpID == lineupID)
                .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
                .FirstOrDefaultAsync();

                scorePlayer.Singles++;
                scorePlayer.H++;
                scorePlayer.PA++;
                scorePlayer.AB++;


            }
            if (hitType == "Double")
            {
                // If there's a player on base 3, ask the user if they score a run or are out
                if (pb3.HasValue)
                {

                    var lineupID2 = _context.GameLineUps
                  .Where(g => g.PlayerID == pb3 && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                  .Select(g => g.ID)
                  .FirstOrDefault();

                    var scorePlayer2 = await _context.ScorePlayers
                   .Where(sp => sp.GameLineUpID == lineupID2)
                    .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
                    .FirstOrDefaultAsync();

                    scorePlayer2.Run++;
                    inplay.Runs++;
                    teamGameScore.score++;
                    inning.ScorePerInning++;
                    rbi++;



                }

                // If there's a player on base 2, they move to base 3
                if (pb2.HasValue)
                {
                    if (pb1.HasValue)
                    {
                        var lineupID2 = _context.GameLineUps
                         .Where(g => g.PlayerID == pb2 && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                         .Select(g => g.ID)
                         .FirstOrDefault();

                        var scorePlayer2 = await _context.ScorePlayers
                       .Where(sp => sp.GameLineUpID == lineupID2)
                       .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
                       .FirstOrDefaultAsync();

                        scorePlayer2.Run++;
                        inplay.Runs++;
                        teamGameScore.score++;
                        inning.ScorePerInning++;
                        rbi++;

                    }
                    else
                    {
                        newP3 = pb2;
                    }

                }

                // If there's a player on base 1, they move to base 3
                if (pb1.HasValue)
                {
                    newP3 = pb1;
                }
                // The batter moves to base 2
                newP2 = pb;

                var lineupID3 = _context.GameLineUps
                    .Where(g => g.PlayerID == inplay.PlayerBattingID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                    .Select(g => g.ID)
                    .FirstOrDefault();

                var scorePlayer3 = await _context.ScorePlayers
               .Where(sp => sp.GameLineUpID == lineupID3)
               .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
               .FirstOrDefaultAsync();

                scorePlayer3.Doubles++;
                scorePlayer3.H++;
                scorePlayer3.PA++;
                scorePlayer3.AB++;
                scorePlayer3.RBI += rbi;


            }

            if (hitType == "Triple")
            {
                // If there's a player on base 3, they score a run
                if (pb3.HasValue)
                {
                    var lineupID4 = _context.GameLineUps
                     .Where(g => g.PlayerID == pb3 && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                     .Select(g => g.ID)
                     .FirstOrDefault();

                    var scorePlayer4 = await _context.ScorePlayers
                   .Where(sp => sp.GameLineUpID == lineupID4)
                   .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
                   .FirstOrDefaultAsync();

                    teamGameScore.score++;
                    scorePlayer4.Run++;
                    inplay.Runs++;
                    inning.ScorePerInning++;
                    rbi++;

                }

                // If there's a player on base 2, they score a run
                if (pb2.HasValue)
                {
                    var lineupID4 = _context.GameLineUps
                    .Where(g => g.PlayerID == pb2 && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                    .Select(g => g.ID)
                    .FirstOrDefault();

                    var scorePlayer4 = await _context.ScorePlayers
                   .Where(sp => sp.GameLineUpID == lineupID4)
                   .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
                   .FirstOrDefaultAsync();

                    teamGameScore.score++;
                    scorePlayer4.Run++;
                    inplay.Runs++;
                    inning.ScorePerInning++;
                    rbi++;
                }

                // If there's a player on base 1, they move to base 3
                // You'll need to ask the user if they score a run or stay on base 3
                if (pb1.HasValue)
                {
                    var lineupID4 = _context.GameLineUps
                    .Where(g => g.PlayerID == pb1 && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                    .Select(g => g.ID)
                    .FirstOrDefault();

                    var scorePlayer4 = await _context.ScorePlayers
                   .Where(sp => sp.GameLineUpID == lineupID4)
                   .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
                    .FirstOrDefaultAsync();

                    teamGameScore.score++;
                    scorePlayer4.Run++;
                    inplay.Runs++;
                    inning.ScorePerInning++;
                    rbi++;
                }

                // The batter moves to base 3
                newP3 = pb;

                var lineupID3 = _context.GameLineUps
                      .Where(g => g.PlayerID == inplay.PlayerBattingID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                      .Select(g => g.ID)
                      .FirstOrDefault();

                var scorePlayer3 = await _context.ScorePlayers
               .Where(sp => sp.GameLineUpID == lineupID3)
               .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
                .FirstOrDefaultAsync();

                scorePlayer3.Triples++;
                scorePlayer3.H++;
                scorePlayer3.PA++;
                scorePlayer3.AB++;
                scorePlayer3.RBI += rbi;
            }

            if (hitType == "HomeRun")
            {
                // If there's a player on base 3, they score a run
                if (pb3.HasValue)
                {
                    var lineupID = _context.GameLineUps
                      .Where(g => g.PlayerID == pb3 && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                      .Select(g => g.ID)
                      .FirstOrDefault();

                    var scorePlayer = await _context.ScorePlayers
                   .Where(sp => sp.GameLineUpID == lineupID)
                   .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
                    .FirstOrDefaultAsync();

                    scorePlayer.Run++;
                    teamGameScore.score++;
                    inplay.Runs++;
                    inning.ScorePerInning++;
                    rbi++;
                }

                // If there's a player on base 2, they score a run
                if (pb2.HasValue)
                {
                    var lineupID = _context.GameLineUps
                      .Where(g => g.PlayerID == pb2 && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                      .Select(g => g.ID)
                      .FirstOrDefault();

                    var scorePlayer = await _context.ScorePlayers
                   .Where(sp => sp.GameLineUpID == lineupID)
                   .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
                    .FirstOrDefaultAsync();

                    scorePlayer.Run++;
                    teamGameScore.score++;
                    inplay.Runs++;
                    inning.ScorePerInning++;
                    rbi++;
                }

                // If there's a player on base 1, they score a run
                if (pb1.HasValue)
                {
                    var lineupID4 = _context.GameLineUps
                      .Where(g => g.PlayerID == pb1 && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                      .Select(g => g.ID)
                      .FirstOrDefault();

                    var scorePlayer4 = await _context.ScorePlayers
                   .Where(sp => sp.GameLineUpID == lineupID4)
                   .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
                   .FirstOrDefaultAsync();

                    scorePlayer4.Run++;
                    teamGameScore.score++;
                    inplay.Runs++;
                    inning.ScorePerInning++;
                    rbi++;
                }

                var lineupID3 = _context.GameLineUps
                           .Where(g => g.PlayerID == inplay.PlayerBattingID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                           .Select(g => g.ID)
                           .FirstOrDefault();

                var scorePlayer3 = await _context.ScorePlayers
               .Where(sp => sp.GameLineUpID == lineupID3)
               .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
                .FirstOrDefaultAsync();

                scorePlayer3.HR++;
                scorePlayer3.H++;
                scorePlayer3.PA++;
                scorePlayer3.AB++;
                scorePlayer3.Run++;
                inplay.Runs++;
                inning.ScorePerInning++;
                teamGameScore.score++;
                rbi++;
                scorePlayer3.RBI += rbi;



                // Clear the bases
                newP1 = null;
                newP2 = null;
                newP3 = null;
            }

            GetNextPlayer(id);

            inplay.PlayerInBase1ID = newP1;
            inplay.PlayerInBase2ID = newP2;
            inplay.PlayerInBase3ID = newP3;
            await _context.SaveChangesAsync();

            object inplayData = null;

            // Get the scores for each team
            var scores = _context.TeamGame
                .Where(tg => tg.GameID == inning.GameID)
                .ToList();

            var playeratBat = _context.Players
              .Where(p => p.ID == inplay.PlayerBattingID)
              .Select(p => p.FullName)
              .FirstOrDefault();

            var defTeam = _context.TeamGame
                .Where(tg => tg.GameID == inning.GameID && tg.TeamID == inning.TeamID)
                .Select(tg => tg.IsHomeTeam)
                .FirstOrDefault();

            if (defTeam)
            {
                ViewBag.InningScoresTeam2 = innings.ToDictionary(i => i.InningNumber, i => i.ScorePerInning);
                ViewBag.InningScoresTeam1 = innings.ToDictionary(i => i.InningNumber, i => i.ScorePerInningOpponent);

            }
            else
            {
                ViewBag.InningScoresTeam1 = innings.ToDictionary(i => i.InningNumber, i => i.ScorePerInning);
                ViewBag.InningScoresTeam2 = innings.ToDictionary(i => i.InningNumber, i => i.ScorePerInningOpponent);

            }

            inplayData = new
            {
                ID = inplay.ID,
                Runs = scores.FirstOrDefault(s => s.IsHomeTeam == true)?.score,
                Runs2 = scores.FirstOrDefault(s => s.IsVisitorTeam == true)?.score,
                Strikes = inplay.Strikes,
                Outs = inplay.Outs,
                Fouls = inplay.Fouls,
                Balls = inplay.Balls,
                InningNumber = inning.InningNumber,
                FirstPlayer = playeratBat,
            };

            return Json(new { needUserDecision = needUserDecision, message = message, playerInBase3 = pb3, playerBatting = pb, Inplay = inplayData, InningScoresTeam1 = ViewBag.InningScoresTeam1, InningScoresTeam2 = ViewBag.InningScoresTeam2 });
        }

        [HttpPost]
        public async Task<IActionResult> HandleUserDecision(int id, string decision, int batter, int player)
        {

            var inplay = await _context.Inplays.FindAsync(id);
            if (inplay == null)
            {
                return NotFound();
            }

            var inning = await _context.Innings.FindAsync(inplay.InningID);


            var lineupID = _context.GameLineUps
            .Where(g => g.PlayerID == player && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
            .Select(g => g.ID)
            .FirstOrDefault();

            var lineupIDB = _context.GameLineUps
            .Where(g => g.PlayerID == batter && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
            .Select(g => g.ID)
            .FirstOrDefault();

            var scorePlayer = await _context.ScorePlayers
                .Where(sp => sp.GameLineUpID == lineupID)
                .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
                .FirstOrDefaultAsync();

            var scorePlayer2 = await _context.ScorePlayers
               .Where(sp => sp.GameLineUpID == lineupIDB)
               .OrderByDescending(sp => sp.ID) // Ordena los resultados por ID en orden descendente
               .FirstOrDefaultAsync();

            var teamGameScore = await _context.TeamGame
                      .Where(tg => tg.GameID == inning.GameID && tg.TeamID == inning.TeamID)
                      .FirstOrDefaultAsync();

            if (decision == "Out")
            {
                inplay.Outs++;
                scorePlayer.Out++;


            }
            else if (decision == "Run")
            {
                inplay.Runs++;
                scorePlayer.Run++;
                scorePlayer2.RBI++;
                teamGameScore.score++;
                inning.ScorePerInning++;


            }
            object inplayData = null;

            // Get the scores for each team
            var scores = _context.TeamGame
                .Where(tg => tg.GameID == inning.GameID)
                .ToList();
            var playeratBat = _context.Players
             .Where(p => p.ID == inplay.PlayerBattingID)
             .Select(p => p.FullName)
             .FirstOrDefault();

            var innings = await _context.Innings
           .Where(i => i.GameID == inning.GameID && i.TeamID == inning.TeamID)
           .OrderBy(i => i.InningNumber)
           .ToListAsync();

            var defTeam = _context.TeamGame
                .Where(tg => tg.GameID == inning.GameID && tg.TeamID == inning.TeamID)
                .Select(tg => tg.IsHomeTeam)
                .FirstOrDefault();

            if (defTeam)
            {
                ViewBag.InningScoresTeam2 = innings.ToDictionary(i => i.InningNumber, i => i.ScorePerInning);
                ViewBag.InningScoresTeam1 = innings.ToDictionary(i => i.InningNumber, i => i.ScorePerInningOpponent);

            }
            else
            {
                ViewBag.InningScoresTeam1 = innings.ToDictionary(i => i.InningNumber, i => i.ScorePerInning);
                ViewBag.InningScoresTeam2 = innings.ToDictionary(i => i.InningNumber, i => i.ScorePerInningOpponent);

            }

            inplayData = new
            {
                ID = inplay.ID,
                Runs = scores.FirstOrDefault(s => s.IsHomeTeam == true)?.score,
                Runs2 = scores.FirstOrDefault(s => s.IsVisitorTeam == true)?.score,
                Strikes = inplay.Strikes,
                Outs = inplay.Outs,
                Fouls = inplay.Fouls,
                Balls = inplay.Balls,
                InningNumber = inning.InningNumber,
                FirstPlayer = playeratBat,
            };

            await _context.SaveChangesAsync();

            return Json(new { Inplay = inplayData, InningScoresTeam1 = ViewBag.InningScoresTeam1, InningScoresTeam2 = ViewBag.InningScoresTeam2 });


        }

        [HttpPost]
        public async Task<IActionResult> RemovePlayer(int playerId)
        {
            var gameLineUp = await _context.GameLineUps
                .Include(gl => gl.Game)
                .Where(gl => gl.PlayerID == playerId)
                .FirstOrDefaultAsync();

            if (gameLineUp == null)
            {
                return NotFound();
            }

            try
            {

                _context.GameLineUps.Remove(gameLineUp);
                await _context.SaveChangesAsync();


                var remainingPlayers = await _context.GameLineUps
                    .Where(gl => gl.GameID == gameLineUp.GameID && gl.TeamID == gameLineUp.TeamID && gl.BattingOrder > gameLineUp.BattingOrder)
                    .ToListAsync();

                foreach (var player in remainingPlayers)
                {
                    player.BattingOrder -= 1;
                }

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "An error occurred while removing the player.");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetPlayersForDivision(int gameId)
        {

            var game = await _context.Games
                                     .Include(g => g.TeamGames)
                                         .ThenInclude(tg => tg.Team)
                                     .FirstOrDefaultAsync(g => g.ID == gameId);

            if (game == null)
            {
                return NotFound();
            }


            var teamIds = game.TeamGames.Select(tg => tg.TeamID).ToList();


            var divisionIds = await _context.Teams
                                             .Where(t => teamIds.Contains(t.ID))
                                             .Select(t => t.DivisionID)
                                             .ToListAsync();


            var players = await _context.Players
                                         .Where(p => !teamIds.Contains(p.TeamID) && (p.Team.DivisionID == divisionIds[0] || p.Team.DivisionID == divisionIds[0] - 1))
                                         .ToListAsync();


            var playersDto = players.Select(p => new { id = p.ID, fullName = p.FullName });

            return Json(playersDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddPlayerToLineup(int playerId, int gameId, int teamId)
        {

            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
            {
                return NotFound("Player not found.");
            }


            var game = await _context.Games.FindAsync(gameId);
            if (game == null)
            {
                return NotFound("Game not found.");
            }


            var team = await _context.Teams.FindAsync(teamId);
            if (team == null)
            {
                return NotFound("Team not found.");
            }


            var existingLineup = await _context.GameLineUps
                .FirstOrDefaultAsync(gl => gl.PlayerID == playerId && gl.GameID == gameId && gl.TeamID == teamId);

            if (existingLineup != null)
            {
                return BadRequest("Player is already in the lineup.");
            }


            var maxBattingOrder = await _context.GameLineUps
                .Where(gl => gl.GameID == gameId && gl.TeamID == teamId)
                .Select(gl => (int?)gl.BattingOrder)
                .MaxAsync();


            var newBattingOrder = maxBattingOrder.GetValueOrDefault() + 1;


            var newLineup = new GameLineUp
            {
                PlayerID = playerId,
                GameID = gameId,
                TeamID = teamId,
                BattingOrder = newBattingOrder
            };


            player.TeamID = teamId;

            _context.GameLineUps.Add(newLineup);
            await _context.SaveChangesAsync();


            return Ok(new
            {
                battingOrder = newLineup.BattingOrder,
                fullName = player.FullName,
                playerId = newLineup.PlayerID
            });
        }

        private bool ScorePlayerExists(int id)
        {
            return _context.ScorePlayers.Any(e => e.ID == id);
        }
        private async Task<List<string>> GetConvenorDivisionsAsync(string convenorEmail)
        {
            var user = await _userManager.FindByEmailAsync(convenorEmail);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);

                switch (roles.FirstOrDefault())
                {
                    case "RookieConvenor":
                        return new List<string> { "9U" };
                    case "IntermediateConvenor":
                        return new List<string> { "11U", "13U" };
                    case "SeniorConvenor":
                        return new List<string> { "15U", "18U" };
                    default:
                        return new List<string> { "9U", "11U", "13U", "15U", "18U" };
                }
            }
            return new List<string>();
        }

        private async Task<List<int>> GetCoachTeamsAsync(string coachEmail)
        {
            var user = await _userManager.FindByEmailAsync(coachEmail);
            if (user != null)
            {

                var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Email == coachEmail);
                if (staff != null)
                {

                    var teamIds = await _context.TeamStaff
                        .Where(ts => ts.StaffID == staff.ID)
                        .Select(ts => ts.TeamID)
                        .ToListAsync();

                    return teamIds;
                }
            }
            return new List<int>();
        }

        private async Task<List<int>> GetScorekeeperTeamsAsync(string coachEmail)
        {
            var user = await _userManager.FindByEmailAsync(coachEmail);
            if (user != null)
            {

                var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Email == coachEmail);
                if (staff != null)
                {

                    var teamIds = await _context.TeamStaff
                        .Where(ts => ts.StaffID == staff.ID)
                        .Select(ts => ts.TeamID)
                        .ToListAsync();

                    return teamIds;
                }
            }
            return new List<int>();
        }
    }
}
