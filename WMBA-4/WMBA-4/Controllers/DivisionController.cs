using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WMBA_4.CustomControllers;
using WMBA_4.Data;
using WMBA_4.Models;
using WMBA_4.Utilities;

namespace WMBA_4.Controllers
{
    public class DivisionController : ElephantController
    {
        private readonly WMBA_4_Context _context;

        public DivisionController(WMBA_4_Context context)
        {
            _context = context;
        }

        // GET: Division
        public async Task<IActionResult> Index(string SearchString, int? ClubID, bool isActive, bool isInactive, int? page, int? pageSizeID,
            string actionButton, string sortDirection = "asc", string sortField = "Division")
        {
            var wMBA_4_Context = _context.Divisions.Include(d => d.Club);
            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;

            var divisions = from d in _context.Divisions
                                      .Include(d => d.Club)
                                      .OrderBy(s => s.Status == true)
                                      .AsNoTracking()
                            select d;

            //sorting sortoption array
            string[] sortOptions = new[] { "Division", "Club" };

            //filter
            if (ClubID.HasValue)
            {
                divisions = divisions.Where(d => d.ClubID == ClubID);
                numberFilters++;
            }
            if (!String.IsNullOrEmpty(SearchString))
            {
                divisions = divisions.Where(d => d.DivisionName.ToUpper().Contains(SearchString.ToUpper()));
                numberFilters++;
            }
            if (isActive == true)
            {
                divisions = divisions.Where(r => r.Status == true);
                numberFilters++;
            }
            if (isInactive == true)
            {
                divisions = divisions.Where(r => r.Status == false);
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

            divisions = divisions
                .OrderByDescending(r => r.Status) // send all false status staff to the back in the list 
                .ThenBy(r => r.DivisionName)
                .ThenBy(r => r.Club.ClubName);

            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                if (sortOptions.Contains(actionButton))//Change of sort is requested
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

                    if (sortField == "Division")
                    {
                        if (sortDirection == "asc")
                        {
                            divisions = divisions.OrderBy(d => d.DivisionName);
                        }
                        else
                        {
                            divisions = divisions
                            .OrderByDescending(d => d.DivisionName);
                        }
                    }
                    else if (sortField == "Club")
                    {
                        if (sortDirection == "asc")
                        {
                            divisions = divisions.OrderBy(d => d.Club.ClubName);
                        }
                        else
                        {
                            divisions = divisions.OrderByDescending(d => d.Club.ClubName);
                        }
                    }
                }
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            ViewData["ClubID"] = new SelectList(_context.Clubs, "ID", "Name");

            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID);
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Division>.CreateAsync(divisions.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: Division/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Divisions == null)
            {
                return NotFound();
            }

            var division = await _context.Divisions
                .Include(d => d.Club)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (division == null)
            {
                return NotFound();
            }

            return View(division);
        }
        // GET: Division/Activate/5
        public async Task<IActionResult> Activate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var divisions = await _context.Divisions.FindAsync(id);
            if (divisions == null)
            {
                return NotFound();
            }

            // Set the Division's status to active
            divisions.Status = true;
            _context.Divisions.Update(divisions);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Division/Create
        public IActionResult Create()
        {
            ViewData["returnURL"] = Url.Action("Index");
            ViewData["ControllerName"] = "Division";
            ViewData["Club"] = new SelectList(_context.Clubs, "ID", "ClubName");
            return View();
        }

        // POST: Division/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,DivisionName,ClubID")] Division division)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(division);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["ClubID"] = new SelectList(_context.Clubs, "ID", "ClubName", division.ClubID);

                return View(division);
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Division Name"))
                {
                    ModelState.AddModelError("Division Name", "The entered Division Name is already in use. Please provide a different Division Name.");
                }
                else
                {
                    ModelState.AddModelError("", "An error occurred while saving. Retry a few times, and if the issue persists, seek assistance from your system administrator.");
                }     
            }
            ViewData["ClubID"] = new SelectList(_context.Clubs,"ID", "ClubName", division.ClubID);
            return View(division);
        }

        // GET: Division/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Divisions == null)
            {
                return NotFound();
            }

            var division = await _context.Divisions.FindAsync(id);
            if (division == null)
            {
                return NotFound();
            }
            ViewData["ClubID"] = new SelectList(_context.Clubs, "ID", "ClubName", division.ClubID);
            return View(division);
        }

        // POST: Division/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,DivisionName,ClubID")] Division division)
        {
            if (id != division.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(division);
                    await _context.SaveChangesAsync();
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DivisionExists(division.ID))
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
                return RedirectToAction("Details", new { division.ID });
            }
            ViewData["ClubID"] = new SelectList(_context.Clubs, "ID", "ClubName", division.ClubID);
            return View(division);
        }

        // GET: Division/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Divisions == null)
            {
                return NotFound();
            }

            var division = await _context.Divisions
                .Include(d => d.Club)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (division == null)
            {
                return NotFound();
            }

            return View(division);
        }

        // POST: Division/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Divisions == null)
            {
                return Problem("Entity set 'WMBA_4_Context.Divisions'  is null.");
            }
            var division = await _context.Divisions.FindAsync(id);

            try
            {

                if (division != null)
                {
                    division.Status = false;
                    _context.Divisions.Update(division);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists see your system administrator.");
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DivisionExists(int id)
        {
          return _context.Divisions.Any(e => e.ID == id);
        }
    }
}
