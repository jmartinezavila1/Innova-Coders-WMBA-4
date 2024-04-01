using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WMBA_4.CustomControllers;
using WMBA_4.Data;
using WMBA_4.Models;
using WMBA_4.Utilities;
using WMBA_4.ViewModels;

namespace WMBA_4.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StaffController : ElephantController
    {
        private readonly WMBA_4_Context _context;
        private readonly ApplicationDbContext _identityContext;
        private IMyEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;


        public StaffController(WMBA_4_Context context,
           ApplicationDbContext identityContext, IMyEmailSender emailSender,
           UserManager<IdentityUser> userManager)
        {
            _context = context;
            _identityContext = identityContext;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        // GET: Staff
        public async Task<IActionResult> Index(string SearchString, int? RoleId, int? CoachID, bool isActive, bool isInactive, int? page, int? pageSizeID,
            string actionButton, string sortDirection = "asc", string sortField = "Staff")
        {
     

            var staffs = await _context.Staff
                 .Select(s => new StaffAdminVM
                 {
                     Email = s.Email,
                     Status = s.Status,

                     ID = s.ID,
                     FirstName = s.FirstName,
                     LastName = s.LastName
                 }).ToListAsync();

            
            foreach (var e in staffs)
            {
                var user = await _userManager.FindByEmailAsync(e.Email);
                if (user != null)
                {
                    e.Roles = (List<string>)await _userManager.GetRolesAsync(user);
                }
            };
            return View(staffs);
        }

        // GET: Employee/Create
        public IActionResult Create()
        {
            StaffAdminVM staff = new StaffAdminVM();
            PopulateAssignedRoleData(staff);
            return View(staff);
        }

        // POST: Employee/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email")] Staff staff, string[] selectedRoles)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(staff);
                    await _context.SaveChangesAsync();

                    var roleId = await _context.Roles
                       .Where(r => selectedRoles.Contains(r.Description))
                       .Select(r => r.ID)
                       .FirstOrDefaultAsync();

                    staff.RoleId = roleId;
                    _context.Update(staff);
                    await _context.SaveChangesAsync();

                    InsertIdentityUser(staff.Email, selectedRoles);

                    //Send Email to new Employee - commented out till email configured
                    //await InviteUserToResetPassword(employee, null);

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                {
                    ModelState.AddModelError("Email", "Unable to save changes. Remember, you cannot have duplicate Email addresses.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            //We are here because something went wrong and need to redisplay
            StaffAdminVM staffAdminVM = new StaffAdminVM
            {
                Email = staff.Email,
                Status = staff.Status,
                ID = staff.ID,
                FirstName = staff.FirstName,
                LastName = staff.LastName,
  
            };
            foreach (var role in selectedRoles)
            {
                staffAdminVM.Roles.Add(role);
            }
            PopulateAssignedRoleData(staffAdminVM);
            return View(staffAdminVM);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staff
                .Where(e => e.ID == id)
                .Select(e => new StaffAdminVM
                {
                    Email = e.Email,
                    Status = e.Status,
                    ID = e.ID,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                
                }).FirstOrDefaultAsync();

            if (staff == null)
            {
                return NotFound();
            }

            //Get the user from the Identity system
            var user = await _userManager.FindByEmailAsync(staff.Email);
            if (user != null)
            {
                //Add the current roles
                var r = await _userManager.GetRolesAsync(user);
                staff.Roles = (List<string>)r;
            }
            PopulateAssignedRoleData(staff);

            return View(staff);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, bool Active, string[] selectedRoles)
        {
            var staffToUpdate = await _context.Staff
                .FirstOrDefaultAsync(m => m.ID == id);
            if (staffToUpdate == null)
            {
                return NotFound();
            }

            //Note the current Email and Active Status
            bool ActiveStatus = staffToUpdate.Status;
            string databaseEmail = staffToUpdate.Email;


            if (await TryUpdateModelAsync<Staff>(staffToUpdate, "",
                e => e.FirstName, e => e.LastName, e => e.Email, e => e.Status))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    //Save successful so go on to related changes

                    //Check for changes in the Active state
                    if (staffToUpdate.Status == false && ActiveStatus == true)
                    {
                        //Deactivating them so delete the IdentityUser
                        //This deletes the user's login from the security system
                        await DeleteIdentityUser(staffToUpdate.Email);

                    }
                    else if (staffToUpdate.Status == true && ActiveStatus == false)
                    {
                        //You reactivating the user, create them and
                        //give them the selected roles
                        InsertIdentityUser(staffToUpdate.Email, selectedRoles);
                    }
                    else if (staffToUpdate.Status == true && ActiveStatus == true)
                    {
                        //No change to Active status so check for a change in Email
                        //If you Changed the email, Delete the old login and create a new one
                        //with the selected roles
                        if (staffToUpdate.Email != databaseEmail)
                        {
                            //Add the new login with the selected roles
                            InsertIdentityUser(staffToUpdate.Email, selectedRoles);

                            //This deletes the user's old login from the security system
                            await DeleteIdentityUser(databaseEmail);
                        }
                        else
                        {
                            //Finially, Still Active and no change to Email so just Update
                            await UpdateUserRoles(selectedRoles, staffToUpdate.Email);
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaffExists(staffToUpdate.ID))
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
                        ModelState.AddModelError("Email", "Unable to save changes. Remember, you cannot have duplicate Email addresses.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    }
                }
            }
            //We are here because something went wrong and need to redisplay
            StaffAdminVM staffAdminVM = new StaffAdminVM
            {
                Email = staffToUpdate.Email,
                ID = staffToUpdate.ID,
                FirstName = staffToUpdate.FirstName,
                LastName = staffToUpdate.LastName,
                Status = staffToUpdate.Status
            };
            foreach (var role in selectedRoles)
            {
                staffAdminVM.Roles.Add(role);
            }
            PopulateAssignedRoleData(staffAdminVM);
            return View(staffAdminVM);
        }

        private void PopulateAssignedRoleData(StaffAdminVM staff)
        {//Prepare checkboxes for all Roles
            var allRoles = _identityContext.Roles;
            var currentRoles = staff.Roles;
            var viewModel = new List<RoleVM>();
            foreach (var r in allRoles)
            {
                viewModel.Add(new RoleVM
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    Assigned = currentRoles.Contains(r.Name)
                });
            }
            ViewBag.Roles = viewModel;
        }

        private async Task UpdateUserRoles(string[] selectedRoles, string Email)
        {
            var _user = await _userManager.FindByEmailAsync(Email);//IdentityUser
            if (_user != null)
            {
                var UserRoles = (List<string>)await _userManager.GetRolesAsync(_user);//Current roles user is in

                if (selectedRoles == null)
                {
                    //No roles selected so just remove any currently assigned
                    foreach (var r in UserRoles)
                    {
                        await _userManager.RemoveFromRoleAsync(_user, r);
                    }
                }
                else
                {
                    //At least one role checked so loop through all the roles
                    //and add or remove as required

                    //We need to do this next line because foreach loops don't always work well
                    //for data returned by EF when working async.  Pulling it into an IList<>
                    //first means we can safely loop over the colleciton making async calls and avoid
                    //the error 'New transaction is not allowed because there are other threads running in the session'
                    IList<IdentityRole> allRoles = _identityContext.Roles.ToList<IdentityRole>();

                    foreach (var r in allRoles)
                    {
                        if (selectedRoles.Contains(r.Name))
                        {
                            if (!UserRoles.Contains(r.Name))
                            {
                                await _userManager.AddToRoleAsync(_user, r.Name);
                            }
                        }
                        else
                        {
                            if (UserRoles.Contains(r.Name))
                            {
                                await _userManager.RemoveFromRoleAsync(_user, r.Name);
                            }
                        }
                    }
                }
            }
        }

        private void InsertIdentityUser(string Email, string[] selectedRoles)
        {
            //Create the IdentityUser in the IdentitySystem
            //Note: this is similar to what we did in ApplicationSeedData
            if (_userManager.FindByEmailAsync(Email).Result == null)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = Email,
                    Email = Email,
                    EmailConfirmed = true //since we are creating it!
                };
                //Create a random password with a default 8 characters
                //string password = MakePassword.Generate();
                IdentityResult result = _userManager.CreateAsync(user).Result;

                if (result.Succeeded)
                {
                    foreach (string role in selectedRoles)
                    {
                        _userManager.AddToRoleAsync(user, role).Wait();
                    }
                }
            }
            else
            {
                TempData["message"] = "The Login Account for " + Email + " was already in the system.";
            }
        }

        private async Task DeleteIdentityUser(string Email)
        {
            var userToDelete = await _identityContext.Users.Where(u => u.Email == Email).FirstOrDefaultAsync();
            if (userToDelete != null)
            {
                _identityContext.Users.Remove(userToDelete);
                await _identityContext.SaveChangesAsync();
            }
        }

        private async Task InviteUserToResetPassword(Staff staff, string message)
        {
            message ??= "Hello " + staff.FirstName + "<br /><p>Please navigate to:<br />" +
                        "<a href='https://theapp.azurewebsites.net/' title='https://theapp.azurewebsites.net/' target='_blank' rel='noopener'>" +
                        "https://theapp.azurewebsites.net</a><br />" +
                        " and create a new password for " + staff.Email + " using Forgot Password.</p>";
            try
            {
                await _emailSender.SendOneAsync(staff.FullName, staff.Email,
                "Account Registration", message);
                TempData["message"] = "Invitation email sent to " + staff.FullName + " at " + staff.Email;
            }
            catch (Exception)
            {
                TempData["message"] = "Could not send Invitation email to " + staff.FullName + " at " + staff.Email;
            }


        }


        private bool StaffExists(int id)
        {
            return _context.Staff.Any(e => e.ID == id);
        }
    }
}
