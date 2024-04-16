using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using WMBA_4.CustomControllers;
using WMBA_4.Data;
using WMBA_4.Models;
using WMBA_4.Utilities;
using WMBA_4.ViewModels;


namespace WMBA_4.Controllers
{
    public class ImportGameController : CognizantController
    {
        private readonly WMBA_4_Context _context;
        public ImportGameController(WMBA_4_Context context)
        {
            _context = context;
        }

        public ActionResult GoToImportGames()
        {
            return View("ImportGame");
        }
        [Authorize(Roles = "Admin,RookieConvenor, IntermediateConvenor, SeniorConvenor")]
        [HttpPost]
        public async Task<IActionResult> ImportGame(IFormFile theExcel)
        {
            string feedBack = string.Empty;
            string errorInvalid = "Invalid Excel file. Please save your CSV document as Excel file and try again.";
            string errorCSV = "Error: That file is not an Excel spreadsheet or CSV file";
            string errorFile = "Error: Problem with the file";
            string errorNofile = "Error: No file uploaded";

            if (theExcel != null)
            {
                string mimeType = theExcel.ContentType;
                long fileLength = theExcel.Length;
                if (!(mimeType == "" || fileLength == 0))//Looks like we have a file!!!
                {
                    if (mimeType.Contains("excel") || mimeType.Contains("spreadsheet"))
                    {
                        ExcelPackage excel;
                        try
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await theExcel.CopyToAsync(memoryStream);
                                excel = new ExcelPackage(memoryStream);
                            }
                            var workSheet = excel.Workbook.Worksheets[0];

                            // Call data processing method
                            feedBack = await ProcessImportedData(workSheet);

                        }
                        catch
                        {

                            feedBack = "<span class=\"text-danger\">" + errorInvalid + "</span>";

                        }

                    }
                    else if (mimeType.Contains("csv"))
                    {
                        var format = new ExcelTextFormat();
                        format.Delimiter = ',';
                        bool firstRowIsHeader = true;

                        using var reader = new System.IO.StreamReader(theExcel.OpenReadStream());

                        using ExcelPackage package = new ExcelPackage();
                        var result = reader.ReadToEnd();
                        ExcelWorksheet workSheet = package.Workbook.Worksheets.Add("Imported Report Data");

                        workSheet.Cells["A1"].LoadFromText(result, format, TableStyles.None, firstRowIsHeader);

                        // Call data processing method
                        feedBack = await ProcessImportedData(workSheet);
                    }
                    else
                    {
                        feedBack = "<span class=\"text-danger\">" + errorCSV + "</span>";
                    }
                }
                else
                {
                    feedBack = "<span class=\"text-danger\">" + errorFile + "</span>";
                }
            }
            else
            {
                feedBack = "<span class=\"text-danger\">" + errorNofile + "</span>";
            }

            TempData["Feedback"] = feedBack;


            return View();
        }

        private async Task<string> ProcessImportedData(ExcelWorksheet workSheet)
        {
            string feedBack = string.Empty;
            var start = workSheet.Dimension.Start;
            var end = workSheet.Dimension.End;

            if (workSheet.Cells[1, 4].Text == "GameType" &&
                workSheet.Cells[1, 6].Text == "HomeTeam" &&
                workSheet.Cells[1, 7].Text == "AwayTeam" &&
                workSheet.Cells[1, 8].Text == "Location" &&
                workSheet.Cells[1, 9].Text == "HomeDivision" &&
                workSheet.Cells[1, 10].Text == "AwayDivision" &&
                workSheet.Cells[1, 11].Text == "sDate")
            {
                int successCount = 0;
                int errorCount = 0;

                for (int row = start.Row + 1; row <= end.Row; row++)
                {
                    using (var transaction = _context.Database.BeginTransaction())
                    {
                        Game g = new Game();
                        try
                        {
                            string dateString = workSheet.Cells[row, 11].Text;
                            DateTime dateValue;

                            string format = "yyyy-MM-dd";
                            if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
                            {
                                g.Date = dateValue;
                                g.Status = true;
                                g.SeasonID = _context.Seasons.FirstOrDefault().ID;

                                //GameType
                                GameType gameType= new GameType();
                             
                                string gtype = workSheet.Cells[row, 4].Text;
                                GameType existingGt= _context.GameTypes.FirstOrDefault(t => t.Description == gtype);
                                if (existingGt == null)
                                {
                                    GameType gt = new GameType { Description = gtype };
                                    _context.GameTypes.Add(gt);
                                    await _context.SaveChangesAsync();
                                    g.GameTypeID = gt.ID;
                                }
                                else
                                {
                                    g.GameTypeID = existingGt.ID;
                                }

                                //Location
                                Location location = new Location();

                                string stringloc = workSheet.Cells[row, 8].Text;
                                Location existingLoc = _context.Locations.FirstOrDefault(t => t.LocationName == stringloc);
                                if (existingLoc == null)
                                {
                                    Location newLoc = new Location { LocationName = stringloc,CityID=1 };
                                    _context.Locations.Add(newLoc);
                                    await _context.SaveChangesAsync();
                                    g.LocationID = newLoc.ID;
                                }
                                else
                                {
                                    g.LocationID = existingLoc.ID;
                                }


                                _context.Games.Add(g);

                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                
                                throw new Exception("Invalid date format at row " + row);
                            }

                            // Validación para la columna "Season"
                            string season = workSheet.Cells[row, 2].Text;
                            int seasonValue;

                            var currentSeason = _context.Seasons.FirstOrDefault();
                            int currentYear = int.Parse(currentSeason.SeasonCode);

                            if (int.TryParse(season, out seasonValue))
                            {
                                //int currentYear = DateTime.Now.Year;
                                if (season != currentYear.ToString())
                            {
                                transaction.Rollback();
                                errorCount++;
                                feedBack += "<span class=\"text-danger\">" + "Error: Record  was rejected because the Season value is not the current year."
                                        + "</span>" + "<br />";
                                continue; // Salta al siguiente registro
                            }
                            }
                            else
                            {
                                transaction.Rollback();
                                errorCount++;
                                feedBack += "<span class=\"text-danger\">" + "Error: Record was rejected because the Season value is not a valid number."
                                        + "</span>" + "<br />";
                                continue; // Salta al siguiente registro
                            }

                            Team t = new Team();
                                //For Divisions
                                string DivisonName = workSheet.Cells[row, 9].Text;
                                Division existingDiv = _context.Divisions.FirstOrDefault(t => t.DivisionName == DivisonName);
                                if (existingDiv == null)
                                {
                                    Division newDivision = new Division { DivisionName = DivisonName, Status = true, ClubID = 1 };
                                    _context.Divisions.Add(newDivision);
                                    await _context.SaveChangesAsync();
                                    t.DivisionID = newDivision.ID;
                                }
                                else
                                {
                                    t.DivisionID = existingDiv.ID;
                                }
                                //For Teams

                                TeamGame tgHome = new TeamGame();
                                TeamGame tgAway = new TeamGame();

                            //HomeTeams
                                string teamNameFirst = workSheet.Cells[row, 6].Text;
                             
                                string teamName = Regex.Replace(teamNameFirst, @"^\d+[A-Za-z]*\s*", "");
                                Team existingTeam = _context.Teams.FirstOrDefault(t => t.Name == teamName);
                                if (existingTeam == null)
                                {
                                    Team newTeam = newTeam = new Team { Name = teamName, DivisionID = t.DivisionID };
                                    _context.Teams.Add(newTeam);

                                    await _context.SaveChangesAsync();
                                    tgHome.TeamID = newTeam.ID;
                                    tgHome.GameID= g.ID;
                                    tgHome.IsHomeTeam =true;
                                    tgHome.IsVisitorTeam = false;


                            }
                                else
                                {
                                    tgHome.TeamID = existingTeam.ID;
                                    tgHome.GameID = g.ID;
                                    tgHome.IsHomeTeam = true;
                                    tgHome.IsVisitorTeam = false;
                            }

                                //AwayTeams
                            string teamNameSecond = workSheet.Cells[row, 7].Text;
                            string teamNameAway = Regex.Replace(teamNameSecond, @"^\d+[A-Za-z]*\s*", "");
                            Team existingTeamAway = _context.Teams.FirstOrDefault(t => t.Name == teamNameAway);
                            if (existingTeamAway == null)
                            {
                                Team newTeam2 = newTeam2 = new Team { Name = teamNameAway, DivisionID = t.DivisionID };
                                _context.Teams.Add(newTeam2);

                                await _context.SaveChangesAsync();
                                tgAway.TeamID = newTeam2.ID;
                                tgAway.GameID = g.ID;
                                tgAway.IsVisitorTeam = true;
                                tgAway.IsHomeTeam = false;
                            }
                            else
                            {
                                tgAway.TeamID = existingTeamAway.ID;
                                tgAway.GameID = g.ID;
                                tgAway.IsVisitorTeam = true;
                                tgAway.IsHomeTeam = false;
                            }


                            _context.TeamGame.Add(tgHome);
                            _context.TeamGame.Add(tgAway);
                           
                                await _context.SaveChangesAsync();
                                successCount++;
                                transaction.Commit();
                        
                        }
                        catch (DbUpdateException dex)
                        {
                            transaction.Rollback();
                            errorCount++;
                           
                                feedBack += "<span class=\"text-danger\">" + "Error: Record  caused an error."
                                        + "</span>" + "<br />";
                            
                        }
                        catch (Exception ex)
                        {

                            feedBack += "<span class=\"text-danger\">" + ex + "Error: Record caused and error."
                                    + "</span>" + "<br />";

                        }
                    }
                }
                foreach (var entry in _context.ChangeTracker.Entries<Game>().Where(e => e.State == EntityState.Added))
                {
                    entry.State = EntityState.Detached;
                }
                if (successCount > 0)
                {
                    feedBack += "Your file has been successfully imported and saved." + "<br/>";
                    feedBack += "Result: " + "<span class=\"text-bold\">" + (successCount + errorCount).ToString() + "</span>" +
                " Records with " + "<span class=\"text-bold text-primary\">" + successCount.ToString() + "</span>" + " inserted and " +
                "<span class=\"text-bold text-danger\">" + errorCount.ToString() + "</span>" + " rejected";
                }
                else
                {
                    feedBack += "Result: " + "<span class=\"text-bold mt-2\">" + (successCount + errorCount).ToString() + "</span>" +
                " Records with " + "<span class=\"text-bold text-primary\">" + successCount.ToString() + "</span>" + " inserted and " +
                "<span class=\"text-bold text-danger\">" + errorCount.ToString() + "</span>" + " rejected";
                }
            }
            else
            {
                feedBack = "Error: You may have selected the wrong file to upload.<br /> Remember, you must have the headings 'First Name','Last Name','Member ID','Season','Division' and 'Team' in the first two cells of the first row.";
            }
            return feedBack;
        }

    }
}
