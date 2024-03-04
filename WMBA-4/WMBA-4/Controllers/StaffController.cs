using System;
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
    public class StaffController : ElephantController
    {
        private readonly WMBA_4_Context _context;

        public StaffController(WMBA_4_Context context)
        {
            _context = context;
        }

        // GET: Staff
        public async Task<IActionResult> Index(string SearchString, int? RoleID, int? CoachID, bool isActive, bool isInactive, int? page, int? pageSizeID,
            string actionButton, string sortDirection = "asc", string sortField = "Staff")
        {
            var wMBA_4_Context = _context.Staff.Include(s => s.Roles);
            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;

            var staff = from r in _context.Staff
                                    .Include(r => r.Roles)
                                    .Where(s => s.Status == true)
                                    .AsNoTracking()
                        select r;

            //sorting sortoption array
            string[] sortOptions = new[] { "Staff", "Role" };

            //filter
            if (RoleID.HasValue)
            {
                staff = staff.Where(r => r.RoleId == RoleID);
                numberFilters++;
            }
            if (!String.IsNullOrEmpty(SearchString))
            {
                staff = staff.Where(r => r.LastName.ToUpper().Contains(SearchString.ToUpper())
                                       || r.FirstName.ToUpper().Contains(SearchString.ToUpper()));
                numberFilters++;
            }
            if (isActive == true)
            {
                staff = staff.Where(r => r.Status == true);
                numberFilters++;
            }
            if (isInactive == true)
            {
                staff = staff.Where(r => r.Status == false);
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

            staff = staff
                .OrderByDescending(r => r.Status) // send all false status staff to the back in the list 
                .ThenBy(r => r.FirstName)
                .ThenBy(r => r.LastName)
                .ThenBy(r => r.RoleId);

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
                if (sortField == "Staff")
                {
                    if (sortDirection == "asc")
                    {
                        staff = staff
                            .OrderBy(r => r.FirstName)
                            .ThenBy(r => r.LastName);
                    }
                    else
                    {
                        staff = staff
                            .OrderByDescending(r => r.FirstName)
                            .ThenByDescending(r => r.LastName);
                    }
                }
                else if (sortField == "Role")
                {
                    if (sortDirection == "asc")
                    {
                        staff = staff.OrderBy(r => r.RoleId);
                    }
                    else
                    {
                        staff = staff.OrderByDescending(r => r.RoleId);
                    }
                }
            }
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            ViewData["RoleID"] = new SelectList(_context.Roles, "ID", "Name");

            // Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID);
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Staff>.CreateAsync(staff.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: Staff/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Staff == null)
            {
                return NotFound();
            }

            var staff = await _context.Staff
                .Include(s => s.Roles)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        // GET: Staff/Activate/5
        public async Task<IActionResult> Activate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staff.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            // Set the staff's status to active
            staff.Status = true;
            _context.Staff.Update(staff);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Staff/Create
        public IActionResult Create()
        {
            ViewData["returnURL"] = Url.Action("Index");
            ViewData["ControllerName"] = "Staff";
            ViewData["Roles"] = new SelectList(_context.Roles, "ID", "Description");
            return View();
        }

        // POST: Staff/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,Email,Status,RoleId")] Staff staff)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(staff);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["RoleId"] = new SelectList(_context.Roles, "ID", "Description", staff.RoleId);

                return View(staff);
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "An error occurred while processing your request.");
                return View(staff);
            }
        }

        // GET: Staff/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Staff == null)
            {
                return NotFound();
            }

            var staff = await _context.Staff.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }
            ViewBag.Roles = await _context.Roles.ToListAsync();

            ViewData["RoleId"] = new SelectList(_context.Roles, "ID", "ID", staff.RoleId);
            return View(staff);
        }

        // POST: Staff/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FirstName,LastName,Email,Status,RoleId")] Staff staff)
        {
            if (id != staff.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(staff);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaffExists(staff.ID))
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
            ViewData["RoleId"] = new SelectList(_context.Roles, "ID", "ID", staff.RoleId);
            return View(staff);
        }

        // GET: Staff/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Staff == null)
            {
                return NotFound();
            }

            var staff = await _context.Staff
                .Include(s => s.Roles)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        // POST: Staff/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Staff == null)
            {
                return Problem("Entity set 'WMBA_4_Context.Staff'  is null.");
            }
            var staff = await _context.Staff.FindAsync(id);

            try
            {

                if (staff != null)
                {
                    staff.Status = false; // Set status to inactive
                    _context.Staff.Update(staff);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists see your system administrator.");
            }
            return RedirectToAction(nameof(Index));
        }

        private bool StaffExists(int id)
        {
          return _context.Staff.Any(e => e.ID == id);
        }
    }
}
