using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMBA_4.Data;
using WMBA_4.Models;
using WMBA_4.Utilities;

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

        // GET: ScorePlayer
        public async Task<IActionResult> SelectTeam(int? id)
        {
            var teams = await _context.GameLineUps
                        .Include(t => t.Team)
                        .Where(t => t.GameID == id)
                        .Select(t => new { t.Team.ID, t.Team.Name })
                        .Distinct()
                        .ToListAsync();
            if (teams.Count == 0)
            {
                TempData["Message"] = "No hay alineación para estos equipos.";
                return RedirectToAction("Index"); // redirige a la vista que quieras
            }
            ViewData["TeamID"] = new SelectList(teams, "ID", "Name");
            ViewData["GameID"] = id;

            var model = new GameLineUp();


            return View(model);
        }

        // This method is for the Start Button

        public async Task<IActionResult> Start(int teamId, int gameId)
        {
            try
            {
                // Creating a new inning
                
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
                var inplay = new Inplay();
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


                var inplayData = new
                {
                    ID= inplay2.ID,
                    Runs = inplay2.Runs,
                    Strikes = inplay2.Strikes,
                    Outs = inplay2.Outs,
                    Fouls = inplay2.Fouls,
                    Balls = inplay2.Balls,
                    InningNumber = inplay2.Inning.InningNumber,
                    FirstPlayer = firstPlayer != null ? firstPlayer.FullName : "N/A"
                };

                scoreplayer = new ScorePlayer
                {
                    GameLineUpID = firstPlayer.ID,
                };
                _context.ScorePlayers.Add(scoreplayer);
                await _context.SaveChangesAsync();



                return Json(new { Inplay = inplayData });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

        }


        //Meotodo para el conteo de bolas

        [HttpPost]
        public async Task<IActionResult> CountBalls(int inplayId)
        {
            // get the inplay record
            var inplay = await _context.Inplays.FindAsync(inplayId);
            if (inplay == null)
            {
                return NotFound();
            }

            //if the balls are less than 4, increment the balls
            if (inplay.Balls < 4)
            {
                inplay.Balls++;
            }

            //if the balls are 4, increment the PA and BB in ScorePlayer
            if (inplay.Balls == 4)
            {
           

                var lineupID = _context.GameLineUps
                    .Where(g => g.PlayerID == inplay.PlayerBattingID)
                    .Select(g => g.ID)
                    .FirstOrDefault();

                var scorePlayer = await _context.ScorePlayers
                    .Where(sp => sp.GameLineUpID == lineupID)
                    .FirstOrDefaultAsync();

                if (scorePlayer == null)
                {
                    return NotFound();
                }

                //// Actualiza los campos PA y BB
                scorePlayer.PA = 1;
                scorePlayer.BB = 1;
            }

            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();

            // Devuelve una respuesta
            return Json(new { Inplay = inplay });
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

            var inPlay = await _context.Inplays
                .Include(i => i.Inning)
                .Where(s => s.Inning.GameID == GameID && s.Inning.TeamID == TeamID)
                .ToListAsync();

            ViewData["Teams"] = teams;
            ViewData["InPlay"] = inPlay;
            ViewBag.Inplay = new Inplay();
            ViewBag.TeamID = TeamID;
            ViewBag.GameID = GameID;

            return View();
        }






        private bool ScorePlayerExists(int id)
        {
            return _context.ScorePlayers.Any(e => e.ID == id);
        }
    }
}
