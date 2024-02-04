﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMBA_4.CustomControllers;
using WMBA_4.Data;
using WMBA_4.Models;

namespace WMBA_4.Controllers
{
    public class PlayerController : ElephantController
    {
        private readonly WMBA_4_Context _context;

        public PlayerController(WMBA_4_Context context)
        {
            _context = context;
        }

        // GET: Player
        public async Task<IActionResult> Index(string SearchString, int? TeamID,
            string actionButton, string sortDirection = "asc", string sortField = "Player")
        {
            var wMBA_4_Context = _context.Players.Include(p => p.Team);

            var players = from p in _context.Players
                                    .Include(p => p.Team).ThenInclude(d => d.Division)
                                    .Where(s => s.Status == true)
                                    .AsNoTracking()
                          select p;

            //sorting sortoption array
            string[] sortOptions = new[] { "Player", "Division", "Team" };

            //filter
            if (TeamID.HasValue)
            {
                players = players.Where(p => p.TeamID == TeamID);
            }
            if (!String.IsNullOrEmpty(SearchString))
            {
                players = players.Where(p => p.LastName.ToUpper().Contains(SearchString.ToUpper())
                                       || p.FirstName.ToUpper().Contains(SearchString.ToUpper()));
            }

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
            if (sortField == "Player")
            {
                if (sortDirection == "asc")
                {
                    players = players
                        .OrderBy(p => p.FirstName)
                        .ThenBy(p => p.LastName);
                }
                else
                {
                    players = players
                        .OrderByDescending(p => p.FirstName)
                        .ThenByDescending(p => p.LastName);
                }
            }
            else if (sortField == "Division")
            {
                if (sortDirection == "asc")
                {
                    players = players
                        .OrderBy(p => p.Team.Division);
                }
                else
                {
                    players = players
                        .OrderByDescending(p => p.Team.Division);
                }
            }
            else
            {
                if (sortDirection == "asc")
                {
                    players = players
                        .OrderBy(p => p.Team);
                }
                else
                {
                    players = players
                        .OrderByDescending(p => p.Team);
                }
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            ViewData["TeamID"] = new SelectList(_context.Teams, "ID", "Name");

            return View(await players.ToListAsync());
        }


        // GET: Player/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .Include(p => p.Team)
                .ThenInclude(d => d.Division)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // GET: Player/Create
        public IActionResult Create()
        {
            ViewData["TeamID"] = new SelectList(_context.Teams, "ID", "Name");
            return View();
        }

        // POST: Player/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,MemberID,FirstName,LastName,JerseyNumber,Status,TeamID")] Player player)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(player);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Players.MemberID"))
                {
                    ModelState.AddModelError("MemberID", "The entered member ID is already in use. Please provide a different member ID.");
                }
                else
                {
                    ModelState.AddModelError("", "An error occurred while saving. Retry a few times, and if the issue persists, seek assistance from your system administrator.");
                }
            }

            ViewData["TeamID"] = new SelectList(_context.Teams, "ID", "Name", player.TeamID);

            return View(player);
        }

        // GET: Player/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .Include(p => p.Team)
                .ThenInclude(d => d.Division)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (player == null)
            {
                return NotFound();
            }

            ViewData["TeamID"] = new SelectList(_context.Teams, "ID", "Name", player.TeamID);
            ViewData["Team.DivisionID"] = new SelectList(_context.Divisions, "ID", "DivisionName", player.Team.DivisionID);
            
            return View(player);
        }

        // POST: Player/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var playerToUpdate = await _context.Players.FirstOrDefaultAsync(p => p.ID == id);

            if (playerToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Player>(playerToUpdate,"", p=>p.MemberID, p=>p.FirstName, p => p.LastName,
                p => p.JerseyNumber, p => p.TeamID))
            {
                try
                {
                    if (IsJerseyNumberDuplicate(playerToUpdate))
                    {
                        ModelState.AddModelError("JerseyNumber", "The jersey number should be unique within the team. Please choose a different jersey number.");
                    }
                    else
                    {
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlayerExists(playerToUpdate.ID))
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
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Players.MemberID"))
                    {
                        ModelState.AddModelError("MemberID", "The entered member ID is already in use. Please provide a different member ID.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "An error occurred while saving. Retry a few times, and if the issue persists, seek assistance from your system administrator.");
                    }
                }
            }

            ViewData["TeamID"] = new SelectList(_context.Teams, "ID", "Name", playerToUpdate.TeamID);
            return View(playerToUpdate);
        }

        // GET: Player/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .Include(p => p.Team)
                .ThenInclude(d => d.Division)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // POST: Player/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Players == null)
            {
                return Problem("The player data could not be retrieved. It may have been deleted or does not exist. If the issue persists, please contact support.");
            }
            var player = await _context.Players.FindAsync(id);

            try
            {
                if (player != null)
                {
                    player.Status = false;
                    player.Team = null;
                    _context.Players.Update(player);
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists see your system administrator.");
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private string GetLatestMemberId()
        {
            // 데이터베이스에서 최신 MemberId 가져오기
            // 예: SELECT TOP 1 MemberId FROM Players ORDER BY MemberId DESC
            // 데이터베이스 컨텍스트를 사용하여 해당 로직을 구현
            var latestMember = _context.Players.OrderByDescending(p => p.MemberID).FirstOrDefault();

            // 최신 MemberId가 존재하면 해당 값을 반환, 없으면 기본값 반환
            return latestMember?.MemberID ?? "M000";
        }

        private bool IsJerseyNumberDuplicate(Player player)
        {
            bool isDuplicated = false;
            if(_context.Players.Any(p => p.TeamID == player.TeamID && p.ID != player.ID && p.JerseyNumber == player.JerseyNumber))
                isDuplicated = true;
            return isDuplicated;
        }

        private bool PlayerExists(int id)
        {
            return _context.Players.Any(e => e.ID == id);
        }
    }
}
