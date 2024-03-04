using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMBA_4.Data;
using WMBA_4.Models;
using WMBA_4.Utilities;
using WMBA_4.ViewModels;

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

        // This method is for creating a new inning
        public async Task<IActionResult> Inning(int teamId, int gameId, int inplayId)
        {
            var inplay = _context.Inplays.Include(i => i.Inning).FirstOrDefault(i => i.ID == inplayId);

            var inning = new Inning
            {
                InningNumber = inplay.InningID+1,
                TeamID = teamId,
                GameID = gameId
            };

            // Add the inning to the context and save the changes
            _context.Innings.Add(inning);
            await _context.SaveChangesAsync();

            // load the lineup data
            var nextBO = _context.GameLineUps
                 .Where(g => g.GameID == inplay.Inning.GameID && g.TeamID == inplay.Inning.TeamID && g.PlayerID == inplay.NextPlayer)
                 .Select(g => g.BattingOrder)
                 .FirstOrDefault();

            var nextPlayer = _context.GameLineUps
                .Where(g => g.GameID == inplay.Inning.GameID && g.TeamID == inplay.Inning.TeamID && g.BattingOrder == nextBO + 1)
                .Select(g => g.Player)
                .FirstOrDefault();

            //Refresh Inplay data           
            inplay.Runs = 0;
            inplay.Strikes = 0;
            inplay.Outs = 0;
            inplay.Fouls = 0;
            inplay.Balls = 0;
            inplay.InningID = inning.ID;
            inplay.NextPlayer = 0;
            inplay.PlayerBattingID = nextPlayer.ID;
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

            var inplayData = new
            {
                ID = inplay2.ID,
                Runs = inplay2.Runs,
                Strikes = inplay2.Strikes,
                Outs = inplay2.Outs,
                Fouls = inplay2.Fouls,
                Balls = inplay2.Balls,
                InningNumber = inplay2.Inning.InningNumber,
                FirstPlayer = nextPlayer != null ? nextPlayer.FullName : "N/A"
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

            return Json(new { Inplay = inplayData});
           

        }


        // This method is for the Start Button

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


                    if (inplay != null)
                    {


                        // Actualizar la información de la vista y retornar
                        inplayData = new
                        {
                            ID = inplay.ID,
                            Runs = inplay.Runs,
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


                    inplayData = new
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


            inplay.Balls++;
            //if the balls are less than 4, increment the balls
            if (inplay.Balls <= 3)
            {

                // Guarda los cambios en la base de datos
                await _context.SaveChangesAsync();



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

                return Json(new { Inplay = inplayData });
            }

            //if the balls are 4, increment the PA and BB in ScorePlayer
            else
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



                inplay.Strikes = 0;
                inplay.Fouls = 0;


                inplay.NextPlayer = (int)inplay.PlayerBattingID;

                await _context.SaveChangesAsync();

                inplayData = new
                {
                    ID = inplay.ID,
                    Runs = inplay.Runs,
                    Strikes = inplay.Strikes,
                    Outs = inplay.Outs,
                    Fouls = inplay.Fouls,
                    Balls = inplay.Balls,
                    InningNumber = inning.InningNumber,
                    FirstPlayer = player,
                };


                return Json(new { Inplay = inplayData });

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

            //var inningNumber = _context.Innings
            //          .Where(i => i.GameID == inplay.Inning.GameID && i.TeamID == inplay.Inning.TeamID)
            //          .Select(i => i.InningNumber)
            //          .FirstOrDefault();

            var inningNumber = _context.Innings
                      .Where(i => i.ID == inplay.InningID)
                      .Select(i => i.InningNumber)
                      .FirstOrDefault();

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


            //if the strikes are less than 3, increment the strikes
            if (inplay.Strikes < 3)
            {
                // Guarda los cambios en la base de datos
                await _context.SaveChangesAsync();



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
            else if (inplay.Strikes >= 3)
            {

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



                inplay.Balls = 0;
                inplay.Strikes = 0;
                inplay.Fouls = 0;
                inplay.Outs++;
                inplay.NextPlayer = (int)inplay.PlayerBattingID;
                inplay.PlayerBattingID = null;



                // Guarda los cambios en la base de datos
                await _context.SaveChangesAsync();


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
                    FirstPlayer = "No player"
                };

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
            if (inplay.PlayerBattingID == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Click on Next Player to continue."
                });
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

            //var nextPlayer = _context.GameLineUps
            //    .Where(g => g.GameID == inplay.Inning.GameID && g.TeamID == inplay.Inning.TeamID && g.PlayerID == inplay.PlayerBattingID + 1)
            //    .Select(g => g.Player)
            //    .FirstOrDefault();

            //inplay.PlayerInBase1ID = inplay.PlayerBattingID;
            //inplay.PlayerBattingID = player;
            inplay.NextPlayer = (int)inplay.PlayerBattingID;
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
                FirstPlayer = player
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
            if (inplay.PlayerBattingID == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Click on Next Player to continue."
                });
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

            inplay.NextPlayer = (int)inplay.PlayerBattingID;
            inplay.PlayerBattingID = null;
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
                FirstPlayer = "No player"
            };
            inplay.Strikes = 0;
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

            var lineupID = _context.GameLineUps
                .Where(g => g.PlayerID == inplay.NextPlayer && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                .Select(g => g.ID)
                .FirstOrDefault();

            var scorePlayer = await _context.ScorePlayers
                .Where(sp => sp.GameLineUpID == lineupID)
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

                //Saving infromation for Runs

                if (model.IsRunPlayer1)
                {
                    lineupIDP1 = _context.GameLineUps
                        .Where(g => g.PlayerID == inplay.PlayerInBase1ID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                        .Select(g => g.ID)
                        .FirstOrDefault();

                    scorePlayerP1 = await _context.ScorePlayers
                          .Where(sp => sp.GameLineUpID == lineupIDP1)
                          .FirstOrDefaultAsync();

                    scorePlayerP1.Run++;
                    inplay.Runs++;
                    inplay.PlayerInBase1ID = null;
                }
                if (model.IsRunPlayer2)
                {
                    lineupIDP2 = _context.GameLineUps
                       .Where(g => g.PlayerID == inplay.PlayerInBase2ID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                       .Select(g => g.ID)
                       .FirstOrDefault();

                    scorePlayerP2 = await _context.ScorePlayers
                          .Where(sp => sp.GameLineUpID == lineupIDP2)
                          .FirstOrDefaultAsync();

                    scorePlayerP2.Run++;
                    inplay.Runs++;
                    inplay.PlayerInBase2ID = null;
                }
                if (model.IsRunPlayer3)
                {
                    lineupIDP3 = _context.GameLineUps
                       .Where(g => g.PlayerID == inplay.PlayerInBase3ID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                       .Select(g => g.ID)
                       .FirstOrDefault();

                    scorePlayerP3 = await _context.ScorePlayers
                          .Where(sp => sp.GameLineUpID == lineupIDP3)
                          .FirstOrDefaultAsync();

                    scorePlayerP3.Run++;
                    inplay.Runs++;
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
                          .FirstOrDefaultAsync();

                    sppP1.Out++;
                    inplay.Outs++;
                    inplay.PlayerInBase1ID = null;
                }
                if (model.IsOutPlayer2)
                {
                    lIDP2 = _context.GameLineUps
                        .Where(g => g.PlayerID == inplay.PlayerInBase1ID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                        .Select(g => g.ID)
                        .FirstOrDefault();

                    sppP2 = await _context.ScorePlayers
                          .Where(sp => sp.GameLineUpID == lIDP2)
                          .FirstOrDefaultAsync();

                    sppP2.Out++;
                    inplay.Outs++;
                    inplay.PlayerInBase2ID = null;
                }
                if (model.IsOutPlayer3)
                {
                    lIDP3 = _context.GameLineUps
                        .Where(g => g.PlayerID == inplay.PlayerInBase1ID && g.GameID == inning.GameID && g.TeamID == inning.TeamID)
                        .Select(g => g.ID)
                        .FirstOrDefault();

                    sppP3 = await _context.ScorePlayers
                          .Where(sp => sp.GameLineUpID == lIDP3)
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
                    scorePlayer.H++;                   
                    scorePlayer.PA++;
                    scorePlayer.AB++;
                    inplay.PlayerBattingID = null;
                    inplay.Runs++;

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
                inplayData = new
                {
                    ID = inplay.ID,
                    Runs = inplay.Runs,
                    Strikes = inplay.Strikes,
                    Outs = inplay.Outs,
                    Fouls = inplay.Fouls,
                    Balls = inplay.Balls,
                    InningNumber = inning.InningNumber,
                    FirstPlayer = "No Player",
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


            inplay.Balls = 0;
            inplay.Strikes = 0;
            inplay.Fouls = 0;


            // Devuelve una respuesta
            inplayData = new
            {
                ID = inplay.ID,
                Runs = inplay.Runs,
                Strikes = inplay.Strikes,
                Outs = inplay.Outs,
                Fouls = inplay.Fouls,
                Balls = inplay.Balls,
                FirstPlayer = inplay.PlayerBatting.FullName
            };

            return Json(new { Inplay = inplayData });

        }



        private bool ScorePlayerExists(int id)
        {
            return _context.ScorePlayers.Any(e => e.ID == id);
        }
    }
}
