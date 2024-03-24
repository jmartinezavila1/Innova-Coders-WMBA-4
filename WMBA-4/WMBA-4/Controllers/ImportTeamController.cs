using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WMBA_4.Data;
using WMBA_4.Controllers;
using static System.Net.Mime.MediaTypeNames;
using OfficeOpenXml;
using WMBA_4.Models;
using System.Numerics;
using Microsoft.EntityFrameworkCore;


namespace WMBA_4.Controllers
{
    [Authorize(Roles = "Admin,RookieConvenor, IntermediateConvenor, SeniorConvenor")]
    public class ImportTeamController : Controller
    {
        private readonly WMBA_4_Context _context;
        public ImportTeamController(WMBA_4_Context context)
        {
            _context = context;
        }


        [HttpPost]
        public async Task<IActionResult> InsertFromExcel(IFormFile theExcel)
        {
            string feedBack = string.Empty;
            if (theExcel != null)
            {
                string mimeType = theExcel.ContentType;
                long fileLength = theExcel.Length;
                if (!(mimeType == "" || fileLength == 0))//Looks like we have a file!!!
                {
                    if (mimeType.Contains("excel") || mimeType.Contains("spreadsheet") || mimeType.Contains("csv"))
                    {
                        ExcelPackage excel;
                        using (var memoryStream = new MemoryStream())
                        {
                            await theExcel.CopyToAsync(memoryStream);
                            excel = new ExcelPackage(memoryStream);
                        }
                        var workSheet = excel.Workbook.Worksheets[0];
                        var start = workSheet.Dimension.Start;
                        var end = workSheet.Dimension.End;

                        if (workSheet.Cells[1, 2].Text == "First Name" &&
                            workSheet.Cells[1, 3].Text == "Last Name" &&
                            workSheet.Cells[1, 4].Text == "Member ID" &&
                            workSheet.Cells[1, 8].Text == "Team" &&
                            workSheet.Cells[1, 9].Text == "DivisionID" &&
                            workSheet.Cells[1, 10].Text == "TeamName" &&
                            workSheet.Cells[1, 11].Text == "SeasonName")

                        {
                            int successCount = 0;
                            int errorCount = 0;
                            for (int row = start.Row + 1; row <= end.Row; row++)

                            {
                                Player p = new Player();
                                try
                                {
                                    // Insertar Season si no existe
                                    string seasonCode = workSheet.Cells[row, 11].Text;
                                    Season season = _context.Seasons.FirstOrDefault(s => s.SeasonCode == seasonCode);
                                    if (season == null)
                                    {
                                        season = new Season { SeasonCode = seasonCode,SeasonName= "Summer "+seasonCode};
                                        _context.Seasons.Add(season);
                                        _context.SaveChanges();
                                    }


                                    // Insertar Team asociado a la Season
                                    string teamName = workSheet.Cells[row, 10].Text;
                                    int division = workSheet.Cells[row, 9].GetValue<int>(); ;
                                    Team team = new Team
                                    {
                                        Name = teamName,
                                        DivisionID = division  // Asociar al ID de la División
                                    };
                                    _context.Teams.Add(team);
                                    _context.SaveChanges();

                                    // Insertar Player con ID de Team correspondiente
                                    

                                    // Row by row...
                                    p.FirstName = workSheet.Cells[row, 2].Text;
                                    p.LastName = workSheet.Cells[row, 3].Text;
                                    p.MemberID = workSheet.Cells[row, 4].Text;
                                    p.TeamID = team.ID;  // Asociar al ID del Team
                                    _context.Players.Add(p);
                                    _context.SaveChanges();
                                    successCount++;
                                }
                                catch (DbUpdateException dex)
                                {
                                    errorCount++;
                                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                                    {
                                        feedBack += "Error: Record " + p.FirstName + p.LastName + " was rejected as a duplicate."
                                                + "<br />";
                                    }
                                    else
                                    {
                                        feedBack += "Error: Record " + p.FirstName + p.LastName + " caused an error."
                                                + "<br />";
                                    }
                                    //Here is the trick to using SaveChanges in a loop.  You must remove the 
                                    //offending object from the cue or it will keep raising the same error.
                                    _context.Remove(p);
                                }
                                catch (Exception ex)
                                {
                                    errorCount++;
                                    if (ex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                                    {
                                        feedBack += "Error: Record " + p.FirstName + p.LastName +  " was rejected because the Team name is duplicated."
                                                + "<br />";
                                    }
                                    else
                                    {
                                        feedBack += "Error: Record " + p.FirstName + p.LastName + " caused and error."
                                                + "<br />";
                                    }
                                }
                            }
                            feedBack += "Finished Importing " + (successCount + errorCount).ToString() +
                                " Records with " + successCount.ToString() + " inserted and " +
                                errorCount.ToString() + " rejected";
                        }
                        else
                        {
                            feedBack = "Error: You may have selected the wrong file to upload.<br /> Remember, you must have the headings 'First Name','Last Name','Member ID','Season','Division' and 'Team' in the first two cells of the first row.";
                        }
                    }
                    else
                    {
                        feedBack = "Error: That file is not an Excel spreadsheet.";
                    }
                }
                else
                {
                    feedBack = "Error:  file appears to be empty";
                }
            }
            else
            {
                feedBack = "Error: No file uploaded";
            }

            TempData["Feedback"] = feedBack + "<br /><br />";

            //Note that we are assuming that you are using the Preferred Approach to Lookup Values
            //And the custom LookupsController

            return View();
        }
    }

}
