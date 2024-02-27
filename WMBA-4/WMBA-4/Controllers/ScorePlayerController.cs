using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        // GET: For selecting the team
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
                    ID = inplay2.ID,
                    Runs = inplay2.Runs,
                    Strikes = inplay2.Strikes,
                    Outs = inplay2.Outs,
                    Fouls = inplay2.Fouls,
                    Balls = inplay2.Balls,
                    InningNumber = inplay2.Inning.InningNumber,
                    FirstPlayer = firstPlayer != null ? firstPlayer.FullName : "N/A"
                };

                var lineupPlayers = _context.GameLineUps
                        .Include(p => p.Player)
                        .Where(g => g.GameID == gameId && g.TeamID == teamId)
                        .Select(p => p.Player)
                        .ToList();
                foreach (var player in lineupPlayers)
                {
                    scoreplayer = new ScorePlayer
                    {
                        GameLineUpID = player.ID,
                    };
                    _context.ScorePlayers.Add(scoreplayer);
                }

                //scoreplayer = new ScorePlayer
                //{
                //    GameLineUpID = firstPlayer.ID,
                //};
                _context.ScorePlayers.Add(scoreplayer);
                await _context.SaveChangesAsync();



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
            if (inplay == null)
            {
                return NotFound();
            }

            //if the balls are less than 4, increment the balls
            if (inplay.Balls < 3)
            {
                inplay.Balls++;
                // Guarda los cambios en la base de datos
                await _context.SaveChangesAsync();

                var player = _context.Players
                      .Where(p => p.ID == inplay.PlayerBattingID)
                      .Select(p => p.FullName)
                      .FirstOrDefault();

                var inningNumber = _context.Innings
                      .Where(i => i.ID == inplay.InningID)
                      .Select(i => i.InningNumber)
                      .FirstOrDefault();
                inplayData = new
                {
                    ID = inplay.ID,
                    Runs = inplay.Runs,
                    Strikes = inplay.Strikes,
                    Outs = inplay.Outs,
                    Fouls = inplay.Fouls,
                    Balls = inplay.Balls,
                    InningNumber = inningNumber,
                    FirstPlayer = player,
                };
            }

            //if the balls are 4, increment the PA and BB in ScorePlayer
            else if (inplay.Balls >= 3)
            {
                var inning = await _context.Innings.FindAsync(inplay.InningID);

                var lineupID = _context.GameLineUps
                    .Where(g => g.PlayerID == inplay.PlayerBattingID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
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
                scorePlayer.PA++;
                scorePlayer.BB++;

                var nextPlayer = _context.GameLineUps
                    .Where(g => g.GameID == inplay.Inning.GameID && g.TeamID == inplay.Inning.TeamID && g.PlayerID == inplay.PlayerBattingID + 1)
                    .Select(g => g.Player)
                    .FirstOrDefault();

                inplay.PlayerInBase1ID = inplay.PlayerBattingID;
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
                    Runs = inplay.Runs,
                    Strikes = inplay.Strikes,
                    Outs = inplay.Outs,
                    Fouls = inplay.Fouls,
                    Balls = inplay.Balls,
                    InningNumber = inningNumber,
                    FirstPlayer = nextPlayer != null ? nextPlayer.FullName : "N/A"
                };

                // Guarda los cambios en la base de datos
                await _context.SaveChangesAsync();

            }


            return Json(new { Inplay = inplayData });

        }



        //Method to count Strikes

        [HttpPost]
        public async Task<IActionResult> CountStrikes(int inplayId)
        {
            object inplayData = null;
            // get the inplay record
            var inplay = await _context.Inplays.FindAsync(inplayId);
            if (inplay == null)
            {
                return NotFound();
            }

            //if the balls are less than 4, increment the balls
            if (inplay.Strikes < 2)
            {
                inplay.Strikes++;
                // Guarda los cambios en la base de datos
                await _context.SaveChangesAsync();

                var player = _context.Players
                      .Where(p => p.ID == inplay.PlayerBattingID)
                      .Select(p => p.FullName)
                      .FirstOrDefault();

                var inningNumber = _context.Innings
                      .Where(i => i.ID == inplay.InningID)
                      .Select(i => i.InningNumber)
                      .FirstOrDefault();
                inplayData = new
                {
                    ID = inplay.ID,
                    Runs = inplay.Runs,
                    Strikes = inplay.Strikes,
                    Outs = inplay.Outs,
                    Fouls = inplay.Fouls,
                    Balls = inplay.Balls,
                    InningNumber = inningNumber,
                    FirstPlayer = player,
                };
            }

            //if the Strikes are 3, increment the PA and Strike Out in ScorePlayer
            else if (inplay.Strikes >= 2)
            {
                var inning = await _context.Innings.FindAsync(inplay.InningID);

                var lineupID = _context.GameLineUps
                    .Where(g => g.PlayerID == inplay.PlayerBattingID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                    .Select(g => g.ID)
                    .FirstOrDefault();

                var scorePlayer = await _context.ScorePlayers
                    .Where(sp => sp.GameLineUpID == lineupID)
                    .FirstOrDefaultAsync();

                if (scorePlayer == null)
                {
                    return NotFound();
                }

                //// update fields PA ad StrikeOut
                scorePlayer.PA++;
                scorePlayer.StrikeOut++;

                var nextPlayer = _context.GameLineUps
                    .Where(g => g.GameID == inplay.Inning.GameID && g.TeamID == inplay.Inning.TeamID && g.PlayerID == inplay.PlayerBattingID + 1)
                    .Select(g => g.Player)
                    .FirstOrDefault();


                inplay.PlayerBattingID = nextPlayer.ID;
                inplay.Balls = 0;
                inplay.Strikes = 0;
                inplay.Fouls = 0;
                inplay.Outs++;

                var inningNumber = _context.Innings
                       .Where(i => i.GameID == inplay.Inning.GameID && i.TeamID == inplay.Inning.TeamID)
                       .Select(i => i.InningNumber)
                       .FirstOrDefault();
                // Devuelve una respuesta
                inplayData = new
                {
                    ID = inplay.ID,
                    Runs = inplay.Runs,
                    Strikes = inplay.Strikes,
                    Outs = inplay.Outs,
                    Fouls = inplay.Fouls,
                    Balls = inplay.Balls,
                    InningNumber = inningNumber,
                    FirstPlayer = nextPlayer != null ? nextPlayer.FullName : "N/A"
                };
                inplay.Strikes = 0;
                // Guarda los cambios en la base de datos
                await _context.SaveChangesAsync();

            }


            return Json(new { Inplay = inplayData });

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

            //if the stikes are less than 2, increment the fouls
            if (inplay.Strikes < 2)
            {
                inplay.Strikes++;
                inplay.Fouls++;
                // Guarda los cambios en la base de datos
                await _context.SaveChangesAsync();

                var player = _context.Players
                      .Where(p => p.ID == inplay.PlayerBattingID)
                      .Select(p => p.FullName)
                      .FirstOrDefault();

                var inningNumber = _context.Innings
                      .Where(i => i.ID == inplay.InningID)
                      .Select(i => i.InningNumber)
                      .FirstOrDefault();
                inplayData = new
                {
                    ID = inplay.ID,
                    Runs = inplay.Runs,
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

                var player = _context.Players
                      .Where(p => p.ID == inplay.PlayerBattingID)
                      .Select(p => p.FullName)
                      .FirstOrDefault();

                var inningNumber = _context.Innings
                      .Where(i => i.ID == inplay.InningID)
                      .Select(i => i.InningNumber)
                      .FirstOrDefault();
                inplayData = new
                {
                    ID = inplay.ID,
                    Runs = inplay.Runs,
                    Strikes = inplay.Strikes,
                    Outs = inplay.Outs,
                    Fouls = inplay.Fouls,
                    Balls = inplay.Balls,
                    InningNumber = inningNumber,
                    FirstPlayer = player,
                };
                // Guarda los cambios en la base de datos
                await _context.SaveChangesAsync();

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

            var inning = await _context.Innings.FindAsync(inplay.InningID);

            var lineupID = _context.GameLineUps
                .Where(g => g.PlayerID == inplay.PlayerBattingID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
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
            scorePlayer.PA++;
            scorePlayer.HBP++;

            var nextPlayer = _context.GameLineUps
                .Where(g => g.GameID == inplay.Inning.GameID && g.TeamID == inplay.Inning.TeamID && g.PlayerID == inplay.PlayerBattingID + 1)
                .Select(g => g.Player)
                .FirstOrDefault();

            inplay.PlayerInBase1ID = inplay.PlayerBattingID;
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
                Runs = inplay.Runs,
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

            var inning = await _context.Innings.FindAsync(inplay.InningID);

            var lineupID = _context.GameLineUps
                .Where(g => g.PlayerID == inplay.PlayerBattingID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                .Select(g => g.ID)
                .FirstOrDefault();

            var scorePlayer = await _context.ScorePlayers
                .Where(sp => sp.GameLineUpID == lineupID)
                .FirstOrDefaultAsync();

            if (scorePlayer == null)
            {
                return NotFound();
            }

            //// update fields PA ad StrikeOut
            scorePlayer.PA++;
            scorePlayer.Out++;

            var nextPlayer = _context.GameLineUps
                .Where(g => g.GameID == inplay.Inning.GameID && g.TeamID == inplay.Inning.TeamID && g.PlayerID == inplay.PlayerBattingID + 1)
                .Select(g => g.Player)
                .FirstOrDefault();


            inplay.PlayerBattingID = nextPlayer.ID;
            inplay.Balls = 0;
            inplay.Strikes = 0;
            inplay.Fouls = 0;
            inplay.Outs++;

            var inningNumber = _context.Innings
                   .Where(i => i.GameID == inplay.Inning.GameID && i.TeamID == inplay.Inning.TeamID)
                   .Select(i => i.InningNumber)
                   .FirstOrDefault();
            // Devuelve una respuesta
            inplayData = new
            {
                ID = inplay.ID,
                Runs = inplay.Runs,
                Strikes = inplay.Strikes,
                Outs = inplay.Outs,
                Fouls = inplay.Fouls,
                Balls = inplay.Balls,
                InningNumber = inningNumber,
                FirstPlayer = nextPlayer != null ? nextPlayer.FullName : "N/A"
            };
            inplay.Strikes = 0;
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


        // GET: Lineup
        public async Task<IActionResult> GetLineup(int gameId, int teamId)
        {
            var lineup = await _context.GameLineUps
                .Include(gl => gl.Player)
                .Where(gl => gl.GameID == gameId && gl.TeamID == teamId)
                .ToListAsync();

            return PartialView("_LineupView", lineup);
        }


        private bool ScorePlayerExists(int id)
        {
            return _context.ScorePlayers.Any(e => e.ID == id);
        }
    }
}
