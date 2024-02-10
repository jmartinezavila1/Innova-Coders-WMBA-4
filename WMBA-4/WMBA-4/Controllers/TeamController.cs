using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using WMBA_4.CustomControllers;
using WMBA_4.Data;
using WMBA_4.Models;
using WMBA_4.Utilities;
using WMBA_4.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using String = System.String;

namespace WMBA_4.Controllers
{
    public class TeamController : ElephantController
    {
        private readonly WMBA_4_Context _context;

        public TeamController(WMBA_4_Context context)
        {
            _context = context;
        }

        // GET: Team
        public async Task<IActionResult> Index(string SearchString, int? DivisionID, int? CoachID, int? page, int? pageSizeID,
            string actionButton, string sortDirection = "asc", string sortField = "Team")
        {
            PopulateDropDownLists();

            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;
            //Then in each "test" for filtering, add to the count of Filters applied

            var teams = from t in  _context.Teams
                .Include(t => t.Division)
                .Where(s => s.Status == true)
                .OrderBy(t => t.Division)
                .AsNoTracking()select t;
            //sorting sortoption array
            string[] sortOptions = new[] { "Team", "Division", "Coach" };

            //Filter
            if (DivisionID.HasValue)
            {
                teams = teams.Where(p => p.DivisionID == DivisionID);
                numberFilters++;
            }           
            if (!String.IsNullOrEmpty(SearchString))
            {
                teams = teams.Where(p => p.Name.ToUpper().Contains(SearchString.ToUpper()));
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
                if (sortField == "Team")
                {
                    if (sortDirection == "asc")
                    {
                        teams = teams
                            .OrderBy(p => p.Name);
                            
                    }
                    else
                    {
                        teams = teams
                            .OrderByDescending(p => p.Name);
                    }
                }
                else if (sortField == "Division")
                {
                    if (sortDirection == "asc")
                    {
                        teams = teams
                            .OrderBy(p => p.Division);
                    }
                    else
                    {
                        teams = teams
                            .OrderByDescending(p => p.Division);
                    }
                }
                else
                {
                    if (sortDirection == "asc")
                    {
                        teams = teams
                            .OrderBy(p => p.Coach_Name);
                    }
                    else
                    {
                        teams = teams
                            .OrderByDescending(p => p.Coach_Name);
                    }
                }
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            ViewData["TeamID"] = new SelectList(_context.Teams, "ID", "Name");

            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID);
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Team>.CreateAsync(teams.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData); ;
        }

        // GET: Team/Details/5
        public IActionResult Details(int id)
        {
            var team = _context.Teams
                .Include(t => t.Division)
                .Include(t => t.TeamGames)
                    .ThenInclude(tg => tg.Game)
                        .ThenInclude(g => g.TeamGames)
                            .ThenInclude(tg => tg.Team)
                .FirstOrDefault(t => t.ID == id);

            var players = from p in _context.Players
            .Include(p => p.Team)
            .Where(s => s.Status == true && s.TeamID == id)
            .OrderBy(p => p.LastName)
            .Where(p=>p.Status==true)
            .AsNoTracking()
             select p;

            if (team == null)
            {
                return NotFound();
            }


            var opponentTeams = new Dictionary<int, string>();

            foreach (var teamGame in team.TeamGames)
            {
                if (teamGame.IsHomeTeam)
                {
                    var opponentTeam = teamGame.Game.TeamGames
                        .FirstOrDefault(tg => tg.IsVisitorTeam)?.Team.Name;

                    opponentTeams[teamGame.GameID] = opponentTeam ?? "Unknown Team";
                }
                else if (teamGame.IsVisitorTeam)
                {
                    var opponentTeam = teamGame.Game.TeamGames
                        .FirstOrDefault(tg => tg.IsHomeTeam)?.Team.Name;

                    opponentTeams[teamGame.GameID] = opponentTeam ?? "Unknown Team";
                }
            }

            ViewBag.OpponentTeams = opponentTeams;
            ViewData["Players"] = players.ToList();
            return View(team);
        }


        // GET: Team/Create
        public IActionResult Create()
        {
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName");
            return View();
        }

        // POST: Team/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Coach_Name,DivisionID")] Team team)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(team);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { team.ID });
                }

            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                {
                    ModelState.AddModelError("Team name", "Unable to save changes. Remember, you cannot have duplicate Team Names.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }

            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", team.DivisionID);
            return View(team);
        }

        // GET: Team/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Teams == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", team.DivisionID);
            return View(team);
        }

        // POST: Team/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Coach_Name,DivisionID")] Team team)
        {
            if (id != team.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(team);
                    await _context.SaveChangesAsync();
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists(team.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException dex)
                {
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                    {
                        ModelState.AddModelError("Name", "Unable to save changes. Remember, you cannot have duplicate Team Names.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", team.DivisionID);
            return View("Index", new List<WMBA_4.Models.Team> { team });
        }

        // GET: Team/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Teams == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .Include(t => t.Division)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: Team/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Teams == null)
            {
                return Problem("Entity set 'WMBA_4_Context.Teams'  is null.");
            }
            var team = await _context.Teams.FindAsync(id);



            if (team != null)
            {
                // Verify if the team has games scheduled
                bool hasScheduledGames = _context.TeamGame
                    .Any(tg => tg.TeamID == id && tg.Game.Date >= DateTime.Today);

                // Verify if the team has Players assigned
                bool hasplayers = _context.Players
                    .Any(tg => tg.TeamID == id);

                if (hasScheduledGames)
                {
                    //Display an error message indicating that the team has games scheduled
                    ModelState.AddModelError(string.Empty, "You cannot delete the team because it has scheduled games for today or later.");
                    return View(nameof(Delete), team);
                }
                else if (hasplayers)
                {
                    //Display an error message indicating that the team has players assigned
                    ModelState.AddModelError(string.Empty, "You cannot delete the team because it has players assigned.");
                    return View(nameof(Delete), team);

                }
                else {
                    team.Status = false;
                    _context.Teams.Update(team);
                }

            }

            await _context.SaveChangesAsync();
            return Redirect(ViewData["returnURL"].ToString());
        }

        public ActionResult GoToImportPlayers()
        {
            return View("ImportTeam");
        }

        [HttpPost]
        public async Task<IActionResult> ImportTeam(IFormFile theExcel)
        {
            string feedBack = string.Empty;
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
                            var start = workSheet.Dimension.Start;
                            var end = workSheet.Dimension.End;

                            if (workSheet.Cells[1, 2].Text == "First Name" &&
                                workSheet.Cells[1, 3].Text == "Last Name" &&
                                workSheet.Cells[1, 4].Text == "Member ID" &&
                                workSheet.Cells[1, 6].Text == "Division" &&
                                workSheet.Cells[1, 7].Text == "Club" &&
                                workSheet.Cells[1, 8].Text == "Team")

                            {
                                int successCount = 0;
                                int errorCount = 0;

                                using (var transaction = _context.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        for (int row = start.Row + 1; row <= end.Row; row++)

                                        {
                                            Player p = new Player();
                                            try
                                            {

                                                // Row by row...
                                                p.FirstName = workSheet.Cells[row, 2].Text;
                                                p.LastName = workSheet.Cells[row, 3].Text;
                                                p.MemberID = workSheet.Cells[row, 4].Text;
                                                Team t = new Team();

                                                //For Divisions
                                                string DivisonName = workSheet.Cells[row, 6].Text;
                                                Division existingDiv = _context.Divisions.FirstOrDefault(t => t.DivisionName == DivisonName);
                                                if (existingDiv == null)
                                                {

                                                    Division newDivision = newDivision = new Division { DivisionName = DivisonName };
                                                    _context.Divisions.Add(newDivision);
                                                    _context.SaveChanges();


                                                    t.DivisionID = newDivision.ID;
                                                }
                                                else
                                                {


                                                    t.DivisionID = existingDiv.ID;
                                                }


                                                //For Teams

                                                //string teamNameFirst = workSheet.Cells[row, 8].Text;
                                                //string teamName = teamNameFirst.Substring(5 - 1);
                                                //Team existingTeam = _context.Teams.FirstOrDefault(t => t.Name == teamName);

                                                string teamNameFirst = workSheet.Cells[row, 8].Text;
                                                string teamName = Regex.Replace(teamNameFirst, @"^\d+[A-Za-z]*\s*", "");
                                                Team existingTeam = _context.Teams.FirstOrDefault(t => t.Name == teamName);
                                                if (existingTeam == null)
                                                {

                                                    Team newTeam = newTeam = new Team { Name = teamName, DivisionID = t.DivisionID };
                                                    _context.Teams.Add(newTeam);
                                                    _context.SaveChanges();


                                                    p.TeamID = newTeam.ID;
                                                }
                                                else
                                                {

                                                    p.TeamID = existingTeam.ID;
                                                }

                                                _context.Players.Add(p);
                                                _context.SaveChanges();
                                                successCount++;

                                            }
                                            catch (DbUpdateException dex)
                                            {
                                                errorCount++;
                                                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                                                {
                                                    feedBack += "Error: Record " + p.FirstName + " " + p.LastName + " was rejected as a duplicate."
                                                            + "<br />";
                                                }
                                                else
                                                {
                                                    feedBack += "Error: Record " + p.FirstName + " " + p.LastName + " caused an error."
                                                            + "<br />";
                                                }


                                            }
                                            catch (Exception ex)
                                            {
                                                errorCount++;
                                                if (ex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                                                {
                                                    feedBack += "Error: Record " + p.FirstName + p.LastName + " was rejected because the Team name is duplicated."
                                                            + "<br />";
                                                }
                                                else
                                                {
                                                    feedBack += "Error: Record " + p.FirstName + p.LastName + " caused and error."
                                                            + "<br />";
                                                }

                                            }
                                        }
                                        foreach (var entry in _context.ChangeTracker.Entries<Player>().Where(e => e.State == EntityState.Added))
                                        {
                                            entry.State = EntityState.Detached;
                                        }
                                        transaction.Commit();
                                        feedBack += "Finished Importing " + (successCount + errorCount).ToString() +
                                            " Records with " + successCount.ToString() + " inserted and " +
                                            errorCount.ToString() + " rejected";
                                    }
                                    catch (Exception)
                                    {
                                        transaction.Rollback();
                                        throw;
                                    }
                                }
                            }
                            else
                            {
                                feedBack = "Error: You may have selected the wrong file to upload.<br /> Remember, you must have the headings 'First Name','Last Name','Member ID','Season','Division' and 'Team' in the first two cells of the first row.";
                            }
                        }
                        catch
                        {

                            feedBack = "Invalid Excel file. Please save your CSV document as Excel file and try again.";

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


            return View();
        }

        private SelectList DivisionList(int? selectedId)
        {
            return new SelectList(_context
                .Divisions
                .OrderBy(m => m.DivisionName), "ID", "DivisionName", selectedId);
        }
        
        private void PopulateDropDownLists(Team team = null)
        {
            ViewData["DivisionID"] = DivisionList(team?.DivisionID);
            
        }
        private bool TeamExists(int id)
        {
            return _context.Teams.Any(e => e.ID == id);
        }

    }

}
