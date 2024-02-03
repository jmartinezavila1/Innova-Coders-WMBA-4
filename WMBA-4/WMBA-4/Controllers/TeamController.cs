using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using WMBA_4.Data;
using WMBA_4.Models;

namespace WMBA_4.Controllers
{
    public class TeamController : Controller
    {
        private readonly WMBA_4_Context _context;

        public TeamController(WMBA_4_Context context)
        {
            _context = context;
        }

        // GET: Team
        public async Task<IActionResult> Index()
        {
            var wMBA_4_Context = _context.Teams
                .Include(t => t.Division)
                .Where(s => s.Status == true);
            return View(await wMBA_4_Context.ToListAsync());
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

            return View(team);
        }

        //Cristina added this for listing the Players in a Team
        // GET: Team/Details/5
        public async Task<IActionResult> TeamPlayer(int teamId)
        {
            Team team = await _context.Teams.FindAsync(teamId);

            if (team == null)
            {
                return NotFound();
            }

            ViewBag.TeamName = team.Name;
            ViewBag.TeamID = team.ID;

            IQueryable<Player> player = _context.Players
            .Where(m => m.TeamID == teamId);

            var playerList = await player.ToListAsync();

            return View(playerList);
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
                    return RedirectToAction(nameof(Index));
                }

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
                return RedirectToAction(nameof(Index));
            }
            ViewData["DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", team.DivisionID);
            return View(team);
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

                if (hasScheduledGames)
                {
                    //Display an error message indicating that the team has games scheduled
                    ModelState.AddModelError(string.Empty, "You cannot delete the team because it has scheduled games for today or later.");
                    return View(nameof(Delete), team);
                }
                team.Status = false;
                _context.Teams.Update(team);

            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
                    if (mimeType.Contains("excel") || mimeType.Contains("spreadsheet") || mimeType.Contains("csv"))
                    {
                        ExcelPackage excel;
                        try {
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
                                                
                                                string teamNameFirst = workSheet.Cells[row, 8].Text;
                                                string teamName = teamNameFirst.Substring(5 - 1);
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
                                                    feedBack += "Error: Record " + p.FirstName + p.LastName + " was rejected as a duplicate."
                                                            + "<br />";
                                                }
                                                else
                                                {
                                                    feedBack += "Error: Record " + p.FirstName + p.LastName + " caused an error."
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

        private bool TeamExists(int id)
        {
            return _context.Teams.Any(e => e.ID == id);
        }

    }

}
