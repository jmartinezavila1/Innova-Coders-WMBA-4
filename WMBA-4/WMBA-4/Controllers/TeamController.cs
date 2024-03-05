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
        public async Task<IActionResult> Index(string SearchString, int? DivisionID, int? CoachID, bool isActive, bool isInactive,int? page, int? pageSizeID,
            string actionButton, string sortDirection = "asc", string sortField = "Team")
        {
            PopulateDropDownLists();

            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;
            //Then in each "test" for filtering, add to the count of Filters applied

            var teams = from t in _context.Teams
                .Include(t => t.Division)
                .Include(t => t.TeamStaff).ThenInclude(ts => ts.Staff)
                .OrderBy(t => t.Division)
                .AsNoTracking()
                        select t;
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
            if (isActive == true)
            {
                teams = teams.Where(p => p.Status == true);
                numberFilters++;
            }
            if (isInactive == true)
            {
                teams = teams.Where(p => p.Status == false);

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

            teams = teams.OrderByDescending(p => p.Status) // Active players first
                    .ThenBy(p => p.Name)     
                    .ThenBy(p => p.Division.DivisionName); // Order by last name


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

        // GET: Player/Activate/5
        public async Task<IActionResult> Activate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }

            // Set the player's status to active
            team.Status = true;
            _context.Teams.Update(team);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Team/Details/5
        public IActionResult Details(int id)
        {
            var team = _context.Teams
                .Include(t => t.Division)
                 .Include(t => t.TeamStaff).ThenInclude(ts => ts.Staff).ThenInclude(s => s.Roles)
                .Include(t => t.TeamGames)
                    .ThenInclude(tg => tg.Game)
                        .ThenInclude(g => g.TeamGames)
                            .ThenInclude(tg => tg.Team)
                .FirstOrDefault(t => t.ID == id);

            var players = from p in _context.Players
            .Include(p => p.Team)
            .Where(s => s.Status == true && s.TeamID == id)
            .OrderBy(p => p.LastName)
            .Where(p => p.Status == true)
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
            // Get the coach of the team
            var coach = team.TeamStaff.FirstOrDefault(ts => ts.Staff.Roles.Description == "Coach")?.Staff;

            // Pass the coach to the view 
            ViewBag.Coach = coach;

            ViewBag.OpponentTeams = opponentTeams;
            ViewData["Players"] = players.ToList();
           
            return View(team);
        }


        // GET: Team/Create
        public IActionResult Create()
        {
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName");


            var staffMembers = _context.Staff.Include(s => s.Roles).ToList();
            var staffSelectItems = staffMembers.Select(s => new SelectListItem
            {
                Value = s.ID.ToString(),
                Text = $"{s.FirstName} {s.LastName} - {s.Roles.Description}"
            });

            ViewData["StaffId"] = new MultiSelectList(staffSelectItems, "Value", "Text");

            return View();
        }

        // POST: Team/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,DivisionID")] Team team, List<int> TeamStaff)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(team);
                    await _context.SaveChangesAsync();

                    foreach (var id in TeamStaff)
                    {
                        var teamStaff = new TeamStaff { TeamID = team.ID, StaffID = id };
                        _context.Add(teamStaff);
                    }

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

            var team = await _context.Teams
                .Include(t => t.TeamStaff).ThenInclude(ts => ts.Staff)
                .FirstOrDefaultAsync(t => t.ID == id);

            //var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", team.DivisionID);

            if (!team.Status) // Check if team is inactive
            {
                TempData["ErrorMessage"] = "The team is inactive, you cannot edit this team.";
                return RedirectToAction("Index");
            }


            var staffMembers = _context.Staff.Include(s => s.Roles).ToList();
            var staffSelectItems = staffMembers.Select(s => new SelectListItem
            {
                Value = s.ID.ToString(),
                Text = $"{s.FirstName} {s.LastName} - {s.Roles.Description}"
            });


            var selectedStaffIds = team.TeamStaff.Select(ts => ts.StaffID.ToString()).ToList();

            ViewData["StaffId"] = new MultiSelectList(staffSelectItems, "Value", "Text", selectedStaffIds);


            ViewBag.SelectedStaffIds = selectedStaffIds;

            return View(team);
        }

        // POST: Team/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,DivisionID")] Team team, List<int> SelectedStaffIds)
        {
            if (id != team.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    var existingStaffMembers = _context.TeamStaff.Where(ts => ts.TeamID == id);
                    _context.TeamStaff.RemoveRange(existingStaffMembers);


                    foreach (var staffId in SelectedStaffIds)
                    {
                        var teamStaff = new TeamStaff { TeamID = id, StaffID = staffId };
                        _context.Add(teamStaff);
                    }
                    _context.Update(team);
                    await _context.SaveChangesAsync();
                }
                catch (RetryLimitExceededException)
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
                return RedirectToAction("Details", new { team });
            }
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", team.DivisionID);
            return View("Details", new List<WMBA_4.Models.Team> { team });
        }
        [HttpPost]
        public async Task<IActionResult> AddCoach(string firstName, string lastName, string email)
        {
            try
            {

                var coachRole = await _context.Roles.FirstOrDefaultAsync(r => r.Description == "Coach");
                if (coachRole == null)
                {
                    ModelState.AddModelError("", "The Coach role does not exist.");
                    return BadRequest(ModelState);
                }


                var coach = new Staff
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Status = true,
                    RoleId = coachRole.ID
                };


                _context.Staff.Add(coach);
                await _context.SaveChangesAsync();

                return Ok(new { id = coach.ID, name = $"{coach.FirstName} {coach.LastName}" });
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while adding the coach.");
                return BadRequest(ModelState);
            }
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
                else
                {
                    team.Status = false;
                    _context.Teams.Update(team);
                    await _context.SaveChangesAsync();
                }

            }

            return Redirect(ViewData["returnURL"].ToString());
        }

        /// <summary>
        /// This is for importing Teams
        /// </summary>
        /// <returns></returns>
        public ActionResult GoToImportPlayers()
        {
            return View("ImportTeam");
        }

        [HttpPost]
        public async Task<IActionResult> ImportTeam(IFormFile theExcel)
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

            if (workSheet.Cells[1, 2].Text == "First Name" &&
                workSheet.Cells[1, 3].Text == "Last Name" &&
                workSheet.Cells[1, 4].Text == "Member ID" &&
                workSheet.Cells[1, 6].Text == "Division" &&
                workSheet.Cells[1, 7].Text == "Club" &&
                workSheet.Cells[1, 8].Text == "Team")
            {
                int successCount = 0;
                int errorCount = 0;

                for (int row = start.Row + 1; row <= end.Row; row++)
                {
                    using (var transaction = _context.Database.BeginTransaction())
                    {
                        Player pl = new Player();
                        try
                        {
                            if (errorCount > 0)
                            {
                                feedBack += "There was a problem with the data you tried to import. The errors listed below." + "<br/>";
                            }

                            pl.FirstName = workSheet.Cells[row, 2].Text;
                            pl.LastName = workSheet.Cells[row, 3].Text;
                            pl.MemberID = workSheet.Cells[row, 4].Text;

                            // Validación para la columna "Season"
                            string season = workSheet.Cells[row, 5].Text;
                            int currentYear = DateTime.Now.Year;
                            if (season != currentYear.ToString())
                            {
                                transaction.Rollback();
                                errorCount++;
                                feedBack += "<span class=\"text-danger\">"+ "Error: Record " + pl.FirstName + " " + pl.LastName + " was rejected because the Season value is not the current year."
                                        +"</span>"+"<br />";
                                continue; // Salta al siguiente registro
                            }
                            Player existingPlayer = _context.Players.FirstOrDefault(p => p.MemberID == pl.MemberID);
                            if (existingPlayer == null)
                            {
                                Team t = new Team();
                                //For Divisions
                                string DivisonName = workSheet.Cells[row, 6].Text;
                                Division existingDiv = _context.Divisions.FirstOrDefault(t => t.DivisionName == DivisonName);
                                if (existingDiv == null)
                                {
                                    Division newDivision = newDivision = new Division { DivisionName = DivisonName };
                                    _context.Divisions.Add(newDivision);
                                    t.DivisionID = newDivision.ID;
                                }
                                else
                                {
                                    t.DivisionID = existingDiv.ID;
                                }
                                //For Teams
                                string teamNameFirst = workSheet.Cells[row, 8].Text;
                                string teamName = Regex.Replace(teamNameFirst, @"^\d+[A-Za-z]*\s*", "");
                                Team existingTeam = _context.Teams.FirstOrDefault(t => t.Name == teamName);
                                if (existingTeam == null)
                                {
                                    Team newTeam = newTeam = new Team { Name = teamName, DivisionID = t.DivisionID };
                                    _context.Teams.Add(newTeam);
                                    pl.TeamID = newTeam.ID;
                                }
                                else
                                {
                                    pl.TeamID = existingTeam.ID;
                                }
                                _context.Players.Add(pl);
                                _context.SaveChanges();
                                successCount++;
                                transaction.Commit();
                            }
                            else
                            {
                                // El jugador ya existe, por lo que no lo agregamos
                                transaction.Rollback();
                                errorCount++;
                                feedBack += "<span class=\"text-danger\">"+ "Error: Record " + pl.FirstName + " " + pl.LastName + " was rejected as a duplicate."
                                        +"</span>"+"<br />";
                            }
                        }
                        catch (DbUpdateException dex)
                        {
                            transaction.Rollback();
                            errorCount++;
                            if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                            {
                                feedBack += "<span class=\"text-danger\">"+ "Error: Record " + pl.FirstName + " " + pl.LastName + " was rejected as a duplicate."
                                        +"</span>"+"<br />";
                            }
                            else
                            {
                                feedBack += "<span class=\"text-danger\">"+ "Error: Record " + pl.FirstName + " " + pl.LastName + " caused an error."
                                        + "</span>" + "<br />";
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            errorCount++;
                            if (ex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                            {
                                feedBack += "<span class=\"text-danger\">"+ "<span class=\"text-danger\">"+ "Error: Record " + pl.FirstName + pl.LastName + " was rejected because the Team name is duplicated."
                                        +"</span>"+ "<br />";
                            }
                            else
                            {
                                feedBack += "<span class=\"text-danger\">"+ "Error: Record " + pl.FirstName + pl.LastName + " caused and error."
                                        + "</span>" + "<br />";
                            }
                        }
                    }
                }
                foreach (var entry in _context.ChangeTracker.Entries<Player>().Where(e => e.State == EntityState.Added))
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
