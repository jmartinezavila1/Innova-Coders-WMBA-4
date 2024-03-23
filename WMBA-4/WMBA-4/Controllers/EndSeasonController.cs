using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Drawing;
using WMBA_4.CustomControllers;
using WMBA_4.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace WMBA_4.Controllers
{
    public class EndSeasonController : CognizantController
    {
        private readonly WMBA_4_Context _context;
        public EndSeasonController(WMBA_4_Context context)
        {
            _context = context;
        }

        public ActionResult index()
        {
            return View("index");
        }
        public async Task<IActionResult> EndSeason()
        {
              //For players
                var playerRep = _context.PlayersEndSeason
                .OrderBy(a => a.ID)
                .Select(p => new
                {
                    ID_Player = p.ID,
                    First_Name = p.FirstName,
                    Last_Name = p.LastName,
                    MamberID = p.MemberID,
                    Jerse_Number = p.JerseyNumber,
                    Season = p.Season,
                    Division = p.DivisionName,
                    Club = p.ClubName,
                    Team = p.Team
                })
                .AsNoTracking();

                //For games
                var gameRep = _context.GamesEndSeason
               .OrderBy(a => a.ID)
               .Select(g => new
               {
                   ID_Game = g.ID,
                   Date = g.Date,
                   Location = g.LocationName,
                   Division = g.DivisionName,
                   Team_Home = g.TeamHome,
                   Score_Home = g.scoreHome,
                   Team_Visitor = g.TeamVisitor,
                   Score_Visitor = g.scoreVisitor,
                   Status = g.Status ? "Active" : "Inactive"
               })
               .AsNoTracking();

                //For Teams
                var teamRep = _context.Teams
                    .Include(t => t.Division)
                    .OrderBy(a => a.ID)
                    .Select(t => new
                    {
                        ID_Team = t.ID,
                        Name = t.Name,
                        Division = t.Division.DivisionName,
                        Status = t.Status ? "Active" : "Inactive"
                    })
                   .AsNoTracking();

                //For Staff
                var staffRep = _context.Staff
                    .Include(t => t.TeamStaff).ThenInclude(ts => ts.Team)
                    .Include(r => r.Roles)
                    .OrderBy(a => a.ID)
                    .Select(t => new
                    {
                        ID_Staff = t.ID,
                        First_Name = t.FirstName,
                        Last_Name = t.LastName,
                        Email = t.Email,
                        Role = t.Roles.Description,
                        Status = t.Status ? "Active" : "Inactive"
                    })
                   .AsNoTracking();

                //For TeamStaff
                var teamStaffRep = _context.TeamStaff
                    .Include(t => t.Staff)
                    .Include(t => t.Team)
                    .OrderBy(a => a.TeamID)
                    .Select(t => new
                    {
                        ID_Staff = t.StaffID,
                        First_Name = t.Staff.FirstName,
                        Last_Name = t.Staff.LastName,
                        Email = t.Staff.Email,
                        Role = t.Staff.Roles.Description,
                        Team = t.Team.Name,
                        Status = t.Staff.Status ? "Active" : "Inactive"
                    })
                   .AsNoTracking();

                //For Divison
                var divisionRep = _context.Divisions
                    .OrderBy(a => a.ID)
                    .Select(d => new
                    {
                        ID_Division = d.ID,
                        Name = d.DivisionName,
                        Status = d.Status ? "Active" : "Inactive"
                    })
                   .AsNoTracking();

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

                //How many rows?
                int numRowsP = playerRep.Count();
                int numRowsG = gameRep.Count();
                int numRowsT = teamRep.Count();
                int numRowsS = staffRep.Count();
                int numRowsTS = teamStaffRep.Count();
                int numRowsD = divisionRep.Count();
                int numRowsSC = scoreRep.Count();

                //Create a new spreadsheet from scratch.
                using (ExcelPackage excel = new ExcelPackage())
                {

                    if (numRowsP > 0) //We have data
                    {

                        var workSheet = excel.Workbook.Worksheets.Add("Players");

                        //Note: Cells[row, column]
                        workSheet.Cells[3, 1].LoadFromCollection(playerRep, true);

                        //Set Style and backgound colour of headings
                        using (ExcelRange headings = workSheet.Cells[3, 1, 3, 9])
                        {
                            headings.Style.Font.Bold = true;
                            var fill = headings.Style.Fill;
                            fill.PatternType = ExcelFillStyle.Solid;
                            fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#5EC22E"));
                        }

                        //Autofit columns
                        workSheet.Cells.AutoFitColumns();
                        //Note: You can manually set width of columns as well
                        //workSheet.Column(7).Width = 10;

                        //Add a title and timestamp at the top of the report
                        workSheet.Cells[1, 1].Value = "Players end of Season " + seasonRep;

                        using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 9])
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
                        using (ExcelRange Rng = workSheet.Cells[2, 7])
                        {
                            Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " +
                                localDate.ToShortDateString();
                            Rng.Style.Font.Bold = true; //Font should be bold
                            Rng.Style.Font.Size = 12;
                            Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }


                    }

                    if (numRowsG > 0) //We have data
                    {
                        var workSheetG = excel.Workbook.Worksheets.Add("Games");

                        //Note: Cells[row, column]
                        workSheetG.Cells[3, 1].LoadFromCollection(gameRep, true);

                        //Set Style and backgound colour of headings
                        using (ExcelRange headings = workSheetG.Cells[3, 1, 3, 9])
                        {
                            headings.Style.Font.Bold = true;
                            var fill = headings.Style.Fill;
                            fill.PatternType = ExcelFillStyle.Solid;
                            fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#5EC22E"));
                        }

                        //Autofit columns
                        workSheetG.Cells.AutoFitColumns();
                        //Note: You can manually set width of columns as well
                        //workSheet.Column(7).Width = 10;

                        //Add a title and timestamp at the top of the report
                        workSheetG.Cells[1, 1].Value = "Games end of Season " + seasonRep;

                        using (ExcelRange Rng = workSheetG.Cells[1, 1, 1, 9])
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
                        using (ExcelRange Rng = workSheetG.Cells[2, 7])
                        {
                            Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " +
                                localDate.ToShortDateString();
                            Rng.Style.Font.Bold = true; //Font should be bold
                            Rng.Style.Font.Size = 12;
                            Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }


                    }

                    if (numRowsT > 0) //We have data
                    {
                        var workSheetT = excel.Workbook.Worksheets.Add("Teams");

                        //Note: Cells[row, column]
                        workSheetT.Cells[3, 1].LoadFromCollection(teamRep, true);

                        //Set Style and backgound colour of headings
                        using (ExcelRange headings = workSheetT.Cells[3, 1, 3, 4])
                        {
                            headings.Style.Font.Bold = true;
                            var fill = headings.Style.Fill;
                            fill.PatternType = ExcelFillStyle.Solid;
                            fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#5EC22E"));
                        }

                        //Autofit columns
                        workSheetT.Cells.AutoFitColumns();
                        //Note: You can manually set width of columns as well
                        //workSheet.Column(7).Width = 10;

                        //Add a title and timestamp at the top of the report
                        workSheetT.Cells[1, 1].Value = "Teams end of Season " + seasonRep;

                        using (ExcelRange Rng = workSheetT.Cells[1, 1, 1, 4])
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
                        using (ExcelRange Rng = workSheetT.Cells[2, 4])
                        {
                            Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " +
                                localDate.ToShortDateString();
                            Rng.Style.Font.Bold = true; //Font should be bold
                            Rng.Style.Font.Size = 12;
                            Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }


                    }

                    if (numRowsS > 0) //We have data
                    {
                        var workSheetS = excel.Workbook.Worksheets.Add("Staff Members");

                        //Note: Cells[row, column]
                        workSheetS.Cells[3, 1].LoadFromCollection(staffRep, true);

                        //Set Style and backgound colour of headings
                        using (ExcelRange headings = workSheetS.Cells[3, 1, 3, 6])
                        {
                            headings.Style.Font.Bold = true;
                            var fill = headings.Style.Fill;
                            fill.PatternType = ExcelFillStyle.Solid;
                            fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#5EC22E"));
                        }

                        //Autofit columns
                        workSheetS.Cells.AutoFitColumns();
                        //Note: You can manually set width of columns as well
                        //workSheet.Column(7).Width = 10;

                        //Add a title and timestamp at the top of the report
                        workSheetS.Cells[1, 1].Value = "Staff end of Season " + seasonRep;

                        using (ExcelRange Rng = workSheetS.Cells[1, 1, 1, 6])
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
                        using (ExcelRange Rng = workSheetS.Cells[2, 4])
                        {
                            Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " +
                                localDate.ToShortDateString();
                            Rng.Style.Font.Bold = true; //Font should be bold
                            Rng.Style.Font.Size = 12;
                            Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }


                    }

                    if (numRowsTS > 0) //We have data
                    {
                        var workSheetTS = excel.Workbook.Worksheets.Add("Staff Members by Team");

                        //Note: Cells[row, column]
                        workSheetTS.Cells[3, 1].LoadFromCollection(teamStaffRep, true);

                        //Set Style and backgound colour of headings
                        using (ExcelRange headings = workSheetTS.Cells[3, 1, 3, 7])
                        {
                            headings.Style.Font.Bold = true;
                            var fill = headings.Style.Fill;
                            fill.PatternType = ExcelFillStyle.Solid;
                            fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#5EC22E"));
                        }

                        //Autofit columns
                        workSheetTS.Cells.AutoFitColumns();
                        //Note: You can manually set width of columns as well
                        //workSheet.Column(7).Width = 10;

                        //Add a title and timestamp at the top of the report
                        workSheetTS.Cells[1, 1].Value = "Divisions of Season " + seasonRep;

                        using (ExcelRange Rng = workSheetTS.Cells[1, 1, 1, 7])
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
                        using (ExcelRange Rng = workSheetTS.Cells[2, 7])
                        {
                            Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " +
                                localDate.ToShortDateString();
                            Rng.Style.Font.Bold = true; //Font should be bold
                            Rng.Style.Font.Size = 12;
                            Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }


                    }

                    if (numRowsD > 0) //We have data
                    {
                        var workSheetD = excel.Workbook.Worksheets.Add("Divisions");

                        //Note: Cells[row, column]
                        workSheetD.Cells[3, 1].LoadFromCollection(divisionRep, true);


                        //Set Style and backgound colour of headings
                        using (ExcelRange headings = workSheetD.Cells[3, 1, 3, 3])
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
                        workSheetD.Cells[1, 1].Value = "Divisions end of Season " + seasonRep;

                        using (ExcelRange Rng = workSheetD.Cells[1, 1, 1, 3])
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


                    }
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
                        workSheetD.Cells[1, 1].Value = "Score of Season " + seasonRep;

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


                    }

                    //Ok, time to download the Excel

                    try
                    {
                        Byte[] theData = excel.GetAsByteArray();
                        string filename = "Season " + seasonRep + " Report.xlsx";
                        string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";


                    _context.Database.ExecuteSqlRaw("DELETE FROM Inplays");
                    _context.Database.ExecuteSqlRaw("DELETE FROM ScorePlayers");
                    _context.Database.ExecuteSqlRaw("DELETE FROM GameLineUps");
                    _context.Database.ExecuteSqlRaw("DELETE FROM Players");
                    _context.Database.ExecuteSqlRaw("DELETE FROM Innings");
                    _context.Database.ExecuteSqlRaw("DELETE FROM TeamGame");
                    _context.Database.ExecuteSqlRaw("DELETE FROM Games");
                    _context.Database.ExecuteSqlRaw("DELETE FROM Locations");
                    _context.Database.ExecuteSqlRaw("DELETE FROM Teams");
                    _context.Database.ExecuteSqlRaw("DELETE FROM Divisions");
       

                    // Reset autoincrement counters
                    _context.Database.ExecuteSqlRaw("VACUUM");
                    // save changes
                    await _context.SaveChangesAsync();


                    return File(theData, mimeType, filename);


                }
                    catch (Exception)
                    {
                        return BadRequest("Could not end Season.");
                    }


                }

                
          
            

          
        }

    }
}
