using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using WMBA_4.CustomControllers;
using WMBA_4.Data;
using WMBA_4.Models;
using WMBA_4.Utilities;
using WMBA_4.ViewModels;

namespace WMBA_4.Controllers
{
    public class ReportController : CognizantController
    {
        private readonly WMBA_4_Context _context;

        public ReportController(WMBA_4_Context context)
        {
            _context = context;
        }

        #region PlayerStats
        //Method to display the PlayerStats
        public async Task<IActionResult> PlayerStats(string SearchString, int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "Player", List<int> selectedPlayers = null, Dictionary<int, string> playerRankings = null, bool validateRankings = false, bool comparePlayers = false)
        {
            IQueryable<PlayerStatsVM> playerStats = _context.PlayerStats;

            if (playerStats == null)
            {
                return NotFound();
            }

            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;

            //sorting sortoption array
            string[] sortOptions = new[] { "Player", "G", "H", "RBI", "Singles", "Doubles", "Triples", "HR", "BB", "PA", "AB", "Run", "HBP", "SO", "Out", "AVG", "OBP", "SLG", "OPS" };

            //filter

            if (!String.IsNullOrEmpty(SearchString))
            {
                playerStats = playerStats.Where(p => p.Player != null && p.Player.ToUpper().Contains(SearchString.ToUpper()));
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
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1;//Reset page to start
                         //sorting
                if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
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
            }
            if (sortField == "Player")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats.OrderBy(p => p.Player);
                }
                else
                {
                    playerStats = playerStats.OrderByDescending(p => p.Player);
                }
            }
            else if (sortField == "G")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats.OrderBy(p => p.G);
                }
                else
                {
                    playerStats = playerStats.OrderByDescending(p => p.G);
                }
            }
            else if (sortField == "H")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats.OrderBy(p => p.H);
                }
                else
                {
                    playerStats = playerStats.OrderByDescending(p => p.H);
                }
            }
            else if (sortField == "RBI")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats.OrderBy(p => p.RBI);
                }
                else
                {
                    playerStats = playerStats.OrderByDescending(p => p.RBI);
                }
            }
            else if (sortField == "Singles")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats.OrderBy(p => p.Singles);
                }
                else
                {
                    playerStats = playerStats.OrderByDescending(p => p.Singles);
                }
            }
            else if (sortField == "Doubles")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats.OrderBy(p => p.Doubles);
                }
                else
                {
                    playerStats = playerStats.OrderByDescending(p => p.Doubles);
                }
            }
            else if (sortField == "Triples")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats.OrderBy(p => p.Triples);
                }
                else
                {
                    playerStats = playerStats.OrderByDescending(p => p.Triples);
                }
            }
            else if (sortField == "HR")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats.OrderBy(p => p.HR);
                }
                else
                {
                    playerStats = playerStats.OrderByDescending(p => p.HR);
                }
            }
            else if (sortField == "BB")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats.OrderBy(p => p.BB);
                }
                else
                {
                    playerStats = playerStats.OrderByDescending(p => p.BB);
                }
            }
            else if (sortField == "PA")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats.OrderBy(p => p.PA);
                }
                else
                {
                    playerStats = playerStats.OrderByDescending(p => p.PA);
                }
            }
            else if (sortField == "AB")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats.OrderBy(p => p.AB);
                }
                else
                {
                    playerStats = playerStats.OrderByDescending(p => p.AB);
                }
            }
            else if (sortField == "Run")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats.OrderBy(p => p.Run);
                }
                else
                {
                    playerStats = playerStats.OrderByDescending(p => p.Run);
                }
            }
            else if (sortField == "HBP")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats.OrderBy(p => p.HBP);
                }
                else
                {
                    playerStats = playerStats.OrderByDescending(p => p.HBP);
                }
            }
            else if (sortField == "SO")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats.OrderBy(p => p.SO);
                }
                else
                {
                    playerStats = playerStats.OrderByDescending(p => p.SO);
                }
            }
            else if (sortField == "Out")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats.OrderBy(p => p.Out);
                }
                else
                {
                    playerStats = playerStats.OrderByDescending(p => p.Out);
                }
            }
            else if (sortField == "AVG")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats.OrderBy(p => p.AVG);
                }
                else
                {
                    playerStats = playerStats.OrderByDescending(p => p.AVG);
                }
            }
            else if (sortField == "OBP")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats.OrderBy(p => p.OBP);
                }
                else
                {
                    playerStats = playerStats.OrderByDescending(p => p.OBP);
                }
            }
            else if (sortField == "SLG")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats.OrderBy(p => p.SLG);
                }
                else
                {
                    playerStats = playerStats.OrderByDescending(p => p.SLG);
                }
            }
            else if (sortField == "OPS")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats.OrderBy(p => p.OPS);
                }
                else
                {
                    playerStats = playerStats.OrderByDescending(p => p.OPS);
                }
            }

            if (comparePlayers)
            {
                //Filter by selected players for comparation

                if (selectedPlayers != null && selectedPlayers.Count > 0)
                {
                    playerStats = playerStats.Where(p => selectedPlayers.Contains(p.ID));
                }
            }


            //For adding player rankings
            playerRankings = playerRankings ?? new Dictionary<int, string>();
            List<string> errorMessages = new List<string>();

            if (validateRankings)
            {
                foreach (var playerRanking in playerRankings)
                {
                    if (string.IsNullOrEmpty(playerRanking.Value))
                    {
                        continue;
                    }

                    var player = _context.Players.Find(playerRanking.Key);
                    if (player != null)
                    {
                        if (int.TryParse(playerRanking.Value, out int ranking) && ranking != 0)
                        {
                            var existingPlayerWithSameRanking = _context.Players.FirstOrDefault(p => p.Ranking == ranking);
                            if (existingPlayerWithSameRanking != null)
                            {
                                errorMessages.Add($"Ranking {ranking} is already in use for player {existingPlayerWithSameRanking.FullName}. Please choose a different ranking.");
                            }
                            else
                            {
                                player.Ranking = ranking;
                                _context.Update(player);
                            }
                        }
                    }
                }

                if (errorMessages.Any())
                {
                    ViewBag.ErrorMessages = errorMessages;
                }
                else
                {
                    await _context.SaveChangesAsync();
                }
            }


            var playerStatsList = await playerStats.ToListAsync();

            var playerRank = _context.Players
            .Where(p => p.Ranking != 0)
            .ToDictionary(p => p.ID, p => p.Ranking);
            ViewBag.PlayerRankings = playerRank;

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID);
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<PlayerStatsVM>.CreateAsync(playerStats.AsQueryable(), page ?? 1, pageSize);

            return View(pagedData);
        }
        #endregion

        #region TeamStats

        //Method to display the TeamStats
        public async Task<IActionResult> TeamStats(string SearchString, int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "Team", List<int> selectedTeams = null, Dictionary<int, string> teamRankings = null, bool validateRankings = false, bool compareTeams = false)
        {
            IQueryable<TeamStatsVM> teamStats = _context.TeamStats;

            if (teamStats == null)
            {
                return NotFound();
            }

            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;

            //sorting sortoption array
            string[] sortOptions = new[] { "Team", "G", "H", "RBI", "Singles", "Doubles", "Triples", "HR", "BB", "PA", "AB", "Run", "HBP", "SO", "Out", "AVG", "OBP", "SLG", "OPS" };

            //filter

            if (!String.IsNullOrEmpty(SearchString))
            {
                teamStats = teamStats.Where(p => p.Team != null && p.Team.ToUpper().Contains(SearchString.ToUpper()));
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
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1;//Reset page to start
                         //sorting
                if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
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
            }
            if (sortField == "Team")
            {
                if (sortDirection == "asc")
                {
                    teamStats = teamStats.OrderBy(p => p.Team);
                }
                else
                {
                    teamStats = teamStats.OrderByDescending(p => p.Team);
                }
            }
            else if (sortField == "G")
            {
                if (sortDirection == "asc")
                {
                    teamStats = teamStats.OrderBy(p => p.G);
                }
                else
                {
                    teamStats = teamStats.OrderByDescending(p => p.G);
                }
            }
            else if (sortField == "H")
            {
                if (sortDirection == "asc")
                {
                    teamStats = teamStats.OrderBy(p => p.H);
                }
                else
                {
                    teamStats = teamStats.OrderByDescending(p => p.H);
                }
            }
            else if (sortField == "RBI")
            {
                if (sortDirection == "asc")
                {
                    teamStats = teamStats.OrderBy(p => p.RBI);
                }
                else
                {
                    teamStats = teamStats.OrderByDescending(p => p.RBI);
                }
            }
            else if (sortField == "Singles")
            {
                if (sortDirection == "asc")
                {
                    teamStats = teamStats.OrderBy(p => p.Singles);
                }
                else
                {
                    teamStats = teamStats.OrderByDescending(p => p.Singles);
                }
            }
            else if (sortField == "Doubles")
            {
                if (sortDirection == "asc")
                {
                    teamStats = teamStats.OrderBy(p => p.Doubles);
                }
                else
                {
                    teamStats = teamStats.OrderByDescending(p => p.Doubles);
                }
            }
            else if (sortField == "Triples")
            {
                if (sortDirection == "asc")
                {
                    teamStats = teamStats.OrderBy(p => p.Triples);
                }
                else
                {
                    teamStats = teamStats.OrderByDescending(p => p.Triples);
                }
            }
            else if (sortField == "HR")
            {
                if (sortDirection == "asc")
                {
                    teamStats = teamStats.OrderBy(p => p.HR);
                }
                else
                {
                    teamStats = teamStats.OrderByDescending(p => p.HR);
                }
            }
            else if (sortField == "BB")
            {
                if (sortDirection == "asc")
                {
                    teamStats = teamStats.OrderBy(p => p.BB);
                }
                else
                {
                    teamStats = teamStats.OrderByDescending(p => p.BB);
                }
            }
            else if (sortField == "PA")
            {
                if (sortDirection == "asc")
                {
                    teamStats = teamStats.OrderBy(p => p.PA);
                }
                else
                {
                    teamStats = teamStats.OrderByDescending(p => p.PA);
                }
            }
            else if (sortField == "AB")
            {
                if (sortDirection == "asc")
                {
                    teamStats = teamStats.OrderBy(p => p.AB);
                }
                else
                {
                    teamStats = teamStats.OrderByDescending(p => p.AB);
                }
            }
            else if (sortField == "Run")
            {
                if (sortDirection == "asc")
                {
                    teamStats = teamStats.OrderBy(p => p.Run);
                }
                else
                {
                    teamStats = teamStats.OrderByDescending(p => p.Run);
                }
            }
            else if (sortField == "HBP")
            {
                if (sortDirection == "asc")
                {
                    teamStats = teamStats.OrderBy(p => p.HBP);
                }
                else
                {
                    teamStats = teamStats.OrderByDescending(p => p.HBP);
                }
            }
            else if (sortField == "SO")
            {
                if (sortDirection == "asc")
                {
                    teamStats = teamStats.OrderBy(p => p.SO);
                }
                else
                {
                    teamStats = teamStats.OrderByDescending(p => p.SO);
                }
            }
            else if (sortField == "Out")
            {
                if (sortDirection == "asc")
                {
                    teamStats = teamStats.OrderBy(p => p.Out);
                }
                else
                {
                    teamStats = teamStats.OrderByDescending(p => p.Out);
                }
            }
            else if (sortField == "AVG")
            {
                if (sortDirection == "asc")
                {
                    teamStats = teamStats.OrderBy(p => p.AVG);
                }
                else
                {
                    teamStats = teamStats.OrderByDescending(p => p.AVG);
                }
            }
            else if (sortField == "OBP")
            {
                if (sortDirection == "asc")
                {
                    teamStats = teamStats.OrderBy(p => p.OBP);
                }
                else
                {
                    teamStats = teamStats.OrderByDescending(p => p.OBP);
                }
            }
            else if (sortField == "SLG")
            {
                if (sortDirection == "asc")
                {
                    teamStats = teamStats.OrderBy(p => p.SLG);
                }
                else
                {
                    teamStats = teamStats.OrderByDescending(p => p.SLG);
                }
            }
            else if (sortField == "OPS")
            {
                if (sortDirection == "asc")
                {
                    teamStats = teamStats.OrderBy(p => p.OPS);
                }
                else
                {
                    teamStats = teamStats.OrderByDescending(p => p.OPS);
                }
            }

            if (compareTeams)
            {
                //Filter by selected players for comparation

                if (selectedTeams != null && selectedTeams.Count > 0)
                {
                    teamStats = teamStats.Where(p => selectedTeams.Contains(p.ID));
                }
            }


            //For adding player rankings
            teamRankings = teamRankings ?? new Dictionary<int, string>();
            List<string> errorMessages = new List<string>();

            if (validateRankings)
            {
                foreach (var teamRanking in teamRankings)
                {
                    if (string.IsNullOrEmpty(teamRanking.Value))
                    {
                        continue;
                    }

                    var team = _context.Players.Find(teamRanking.Key);
                    if (team != null)
                    {
                        if (int.TryParse(teamRanking.Value, out int ranking) && ranking != 0)
                        {
                            var existingTeamWithSameRanking = _context.Players.FirstOrDefault(p => p.Ranking == ranking);
                            if (existingTeamWithSameRanking != null)
                            {
                                errorMessages.Add($"Ranking {ranking} is already in use for Team {existingTeamWithSameRanking.Team}. Please choose a different ranking.");
                            }
                            else
                            {
                                team.Ranking = ranking;
                                _context.Update(team);
                            }
                        }
                    }
                }

                if (errorMessages.Any())
                {
                    ViewBag.ErrorMessages = errorMessages;
                }
                else
                {
                    await _context.SaveChangesAsync();
                }
            }


            var teamStatsList = await teamStats.ToListAsync();

            var teamRank = _context.Teams
            .Where(p => p.Ranking != 0)
            .ToDictionary(p => p.ID, p => p.Ranking);
            ViewBag.TeamRankings = teamRank;

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID);
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<TeamStatsVM>.CreateAsync(teamStats.AsQueryable(), page ?? 1, pageSize);

            return View(pagedData);
        }


        #endregion

        #region PlayerStatsDownload
        public async Task<IActionResult> DowloadPlayerStats()
        {
            //For Score
            var scoreRep = _context.PlayerStats
                .OrderBy(p => p.ID)
                .Select(s => new
                {
                    Player_Name = s.Player,
                    Jersey_Number = s.JerseyNumber,
                    Team = s.Team,
                    G = s.G,
                    H = s.H,
                    RBI = s.RBI,
                    Singles = s.Singles,
                    Doubles = s.Doubles,
                    Triples = s.Triples,
                    HR = s.HR,
                    BB = s.BB,
                    PA = s.PA,
                    AB = s.AB,
                    Run = s.Run,
                    HBP = s.HBP,
                    SO = s.SO,
                    Out = s.Out,
                    AVG = s.AVG,
                    OBP = s.OBP,
                    SLG = s.SLG,
                    OPS = s.OPS,

                })
               .AsNoTracking();

            var seasonRep = await _context.Seasons
                .Select(s => s.SeasonCode)
                .FirstOrDefaultAsync();

            int numRowsSC = scoreRep.Count();

            //Create a new spreadsheet from scratch.
            using (ExcelPackage excel = new ExcelPackage())
            {
                if (numRowsSC > 0) //We have data
                {
                    var workSheetD = excel.Workbook.Worksheets.Add("Score_by_Player");


                    //Note: Cells[row, column]
                    workSheetD.Cells[3, 1].LoadFromCollection(scoreRep, true);

                    // Change the header names
                    workSheetD.Cells["G3"].Value = "1B";
                    workSheetD.Cells["H3"].Value = "2B";
                    workSheetD.Cells["I3"].Value = "3B";

                    //Format for decimal numbers    
                    int numRows = scoreRep.Count();
                    workSheetD.Cells["R4:R" + numRows].Style.Numberformat.Format = "#.000";
                    workSheetD.Cells["S4:S" + numRows].Style.Numberformat.Format = "#.000";
                    workSheetD.Cells["T4:T" + numRows].Style.Numberformat.Format = "#.000";
                    workSheetD.Cells["U4:U" + numRows].Style.Numberformat.Format = "#.000";

                    //Set Style and backgound colour of headings
                    using (ExcelRange headings = workSheetD.Cells[3, 1, 3, 21])
                    {
                        headings.Style.Font.Bold = true;
                        var fill = headings.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#5EC22E"));
                    }

                    //Autofit columns
                    workSheetD.Cells.AutoFitColumns();
                    //Note: You can manually set width of columns as well
                    //workSheet.Column(7).Width = 10;

                    //Add a title and timestamp at the top of the report
                    workSheetD.Cells[1, 1].Value = "Score by Player";

                    using (ExcelRange Rng = workSheetD.Cells[1, 1, 1, 21])
                    {
                        Rng.Merge = true; //Merge columns start and end range
                        Rng.Style.Font.Bold = true; //Font should be bold
                        Rng.Style.Font.Size = 18;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    //Since the time zone where the server is running can be different, adjust to 
                    //Local for us.
                    DateTime utcDate = DateTime.UtcNow;
                    TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                    using (ExcelRange Rng = workSheetD.Cells[2, 3])
                    {
                        Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " +
                            localDate.ToShortDateString();
                        Rng.Style.Font.Bold = true; //Font should be bold
                        Rng.Style.Font.Size = 12;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    try
                    {
                        Byte[] theData = excel.GetAsByteArray();
                        string filename = "Season " + seasonRep + " Report.xlsx";
                        string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        return File(theData, mimeType, filename);
                    }
                    catch (Exception)
                    {
                        return BadRequest("Could not download the report.");
                    }


                }
            }
            return BadRequest("No data to download.");

        }

        #endregion

        #region TeamStatsDownload

        public async Task<IActionResult> DowloadTeamStats()
        {
            //For Score
            var scoreRep = _context.TeamStats
                .OrderBy(p => p.ID)
                .Select(s => new
                {
                    Team = s.Team,
                    G = s.G,
                    H = s.H,
                    RBI = s.RBI,
                    Singles = s.Singles,
                    Doubles = s.Doubles,
                    Triples = s.Triples,
                    HR = s.HR,
                    BB = s.BB,
                    PA = s.PA,
                    AB = s.AB,
                    Run = s.Run,
                    HBP = s.HBP,
                    SO = s.SO,
                    Out = s.Out,
                    AVG = s.AVG,
                    OBP = s.OBP,
                    SLG = s.SLG,
                    OPS = s.OPS,

                })
               .AsNoTracking();

            var seasonRep = await _context.Seasons
                .Select(s => s.SeasonCode)
                .FirstOrDefaultAsync();

            int numRowsSC = scoreRep.Count();

            //Create a new spreadsheet from scratch.
            using (ExcelPackage excel = new ExcelPackage())
            {
                if (numRowsSC > 0) //We have data
                {
                    var workSheetD = excel.Workbook.Worksheets.Add("Score_by_Team");


                    //Note: Cells[row, column]
                    workSheetD.Cells[3, 1].LoadFromCollection(scoreRep, true);

                    // Change the header names
                    workSheetD.Cells["G3"].Value = "1B";
                    workSheetD.Cells["H3"].Value = "2B";
                    workSheetD.Cells["I3"].Value = "3B";

                    //Format for decimal numbers    
                    int numRows = scoreRep.Count();
                    workSheetD.Cells["R4:R" + numRows].Style.Numberformat.Format = "#.000";
                    workSheetD.Cells["S4:S" + numRows].Style.Numberformat.Format = "#.000";
                    workSheetD.Cells["T4:T" + numRows].Style.Numberformat.Format = "#.000";
                    workSheetD.Cells["U4:U" + numRows].Style.Numberformat.Format = "#.000";

                    //Set Style and backgound colour of headings
                    using (ExcelRange headings = workSheetD.Cells[3, 1, 3, 19])
                    {
                        headings.Style.Font.Bold = true;
                        var fill = headings.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#5EC22E"));
                    }

                    //Autofit columns
                    workSheetD.Cells.AutoFitColumns();
                    //Note: You can manually set width of columns as well
                    //workSheet.Column(7).Width = 10;

                    //Add a title and timestamp at the top of the report
                    workSheetD.Cells[1, 1].Value = "Score by Team";

                    using (ExcelRange Rng = workSheetD.Cells[1, 1, 1, 19])
                    {
                        Rng.Merge = true; //Merge columns start and end range
                        Rng.Style.Font.Bold = true; //Font should be bold
                        Rng.Style.Font.Size = 18;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    //Since the time zone where the server is running can be different, adjust to 
                    //Local for us.
                    DateTime utcDate = DateTime.UtcNow;
                    TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                    using (ExcelRange Rng = workSheetD.Cells[2, 3])
                    {
                        Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " +
                            localDate.ToShortDateString();
                        Rng.Style.Font.Bold = true; //Font should be bold
                        Rng.Style.Font.Size = 12;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    try
                    {
                        Byte[] theData = excel.GetAsByteArray();
                        string filename = "Season " + seasonRep + " Report.xlsx";
                        string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        return File(theData, mimeType, filename);
                    }
                    catch (Exception)
                    {
                        return BadRequest("Could not download the report.");
                    }


                }
            }
            return BadRequest("No data to download.");

        }
        #endregion
    }
}
