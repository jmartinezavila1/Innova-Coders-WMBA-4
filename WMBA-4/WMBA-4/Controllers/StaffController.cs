using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
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
        public async Task<IActionResult> Create()
        {
            StaffAdminVM staff = new StaffAdminVM();
            PopulateAssignedRoleData(staff);

            var teams = await _context.Teams.ToListAsync();
            ViewBag.Teams = new SelectList(teams, "ID", "Name");
            return View(staff);
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email")] Staff staff, StaffAdminVM staffVM, string[] selectedRoles)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Your existing code for creating a staff member
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


                    //Send Email to new User - commented out till email configured
                    await InviteUserToResetPassword(staff, null);

                    // Your new code for checking and assigning coach/scorekeeper
                    var team = await _context.Teams.FindAsync(staffVM.SelectedTeamID);

                    if (team != null)
                    {
                        var existingStaff = await _context.TeamStaff
                            .Include(ts => ts.Staff)
                            .Where(ts => ts.TeamID == staffVM.SelectedTeamID)
                            .ToListAsync();

                        // Check if the team already has a coach or scorekeeper assigned
                        if (existingStaff.Any(ts => ts.Staff.Roles != null && (ts.Staff.Roles.Description == "Coach" || ts.Staff.Roles.Description == "Scorekeeper")))
                        {
                            ModelState.AddModelError(string.Empty, "This team already has a coach or scorekeeper assigned.");
                            // Repopulate the dropdown and return the view with the error message
                            var teamList = await _context.Teams.ToListAsync();
                            ViewBag.Teams = new SelectList(teamList, "ID", "Name");
                            return View(staffVM);
                        }

                        // Assign coach and scorekeeper to the selected team
                        team.TeamStaff.Add(new TeamStaff { StaffID = staff.ID, TeamID = team.ID });
                        await _context.SaveChangesAsync();
                    }

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
            // Here, you should repopulate the dropdown and return the view with the model
            var teams = await _context.Teams.ToListAsync();
            ViewBag.Teams = new SelectList(teams, "ID", "Name");
            PopulateAssignedRoleData(staffVM);
            return View(staffVM);
        }
        // Method to send email notification to user
        private async Task SendRoleUpdateEmail(string userEmail, string[] newRoles)
        {
            var message = "Your role has been updated. You now can access to the application with the following roles: " + string.Join(", ", newRoles);
            try
            {
                // Pass an empty string for plaintext message
                await _emailSender.SendOneAsync("User", userEmail, "Role Update", message);
                // Optionally, add a message to TempData indicating the email was sent successfully
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., email sending failure)
                // Optionally, add a message to TempData indicating the failure
            }
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

            var user = await _userManager.FindByEmailAsync(staff.Email);
            if (user != null)
            {
                // Add the current roles
                var r = await _userManager.GetRolesAsync(user);
                staff.Roles = (List<string>)r;
            }

            // Fetch teams associated with the coach or scorekeeper
            var associatedTeams = await _context.TeamStaff
                .Where(ts => ts.StaffID == id)
                .Select(ts => ts.Team)
                .ToListAsync();

            // Fetch all teams
            var allTeams = await _context.Teams.ToListAsync();

            // Create a list to hold SelectListItems for all teams
            List<SelectListItem> teamList = new List<SelectListItem>();

            

            // Add all other teams to the team list
            foreach (var team in allTeams)
            {
                // Skip the associated team as it's already added to the list
                if (associatedTeams.Any(at => at.ID == team.ID))
                    continue;

                teamList.Add(new SelectListItem { Value = team.ID.ToString(), Text = team.Name });
            }

            // Populate the dropdown list with the team list
            ViewBag.Teams = teamList;

            // Set the selected team to the first team that the coach or scorekeeper is assigned to
            if (associatedTeams.Any())
            {
                ViewBag.SelectedTeamID = associatedTeams.First().ID;
            }

            PopulateAssignedRoleData(staff);

            return View(staff);
        }

        [HttpPost]
        [Route("Staff/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, bool Active, string[] selectedRoles, StaffAdminVM staffVM)
        {
            var staffToUpdate = await _context.Staff.FirstOrDefaultAsync(m => m.ID == id);
            if (staffToUpdate == null)
            {
                return NotFound();
            }

            //Prevent users from changing their own roles
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.Email == staffToUpdate.Email)
            {
                ModelState.AddModelError("", "You cannot change your own role.");
            }

            var user = await _userManager.FindByEmailAsync(staffToUpdate.Email);
            var currentRoles = user != null ? await _userManager.GetRolesAsync(user) : new List<string>();

            var rolesUpdated = !currentRoles.SequenceEqual(selectedRoles);

            if (rolesUpdated)
            {
                await UpdateUserRoles(selectedRoles, staffToUpdate.Email);

                // Update RoleId in the Staff table based on the selected role
                var roleId = await _context.Roles
                    .Where(r => selectedRoles.Contains(r.Description))
                    .Select(r => r.ID)
                    .FirstOrDefaultAsync();

                staffToUpdate.RoleId = roleId;
            }

            bool ActiveStatus = staffToUpdate.Status;
            string databaseEmail = staffToUpdate.Email;

            if (await TryUpdateModelAsync<Staff>(staffToUpdate, "",
                s => s.FirstName, s => s.LastName, s => s.Email, s => s.Status))
            {
                try
                {
                    await _context.SaveChangesAsync();

                    if (staffToUpdate.Status == false && ActiveStatus == true)
                    {
                        await DeleteIdentityUser(staffToUpdate.Email);
                    }
                    else if (staffToUpdate.Status == true && ActiveStatus == false)
                    {
                        InsertIdentityUser(staffToUpdate.Email, selectedRoles);
                    }
                    else if (staffToUpdate.Status == true && ActiveStatus == true)
                    {
                        if (staffToUpdate.Email != databaseEmail)
                        {
                            InsertIdentityUser(staffToUpdate.Email, selectedRoles);
                            await DeleteIdentityUser(databaseEmail);
                        }
                    }
                    var team = await _context.Teams.FindAsync(staffVM.SelectedTeamID);
                    if (team != null)
                    {
                        // Remove existing team associations
                        _context.TeamStaff.RemoveRange(_context.TeamStaff.Where(ts => ts.StaffID == staffToUpdate.ID));

                        // Add the new team association
                        team.TeamStaff.Add(new TeamStaff { StaffID = staffToUpdate.ID, TeamID = team.ID });

                        // Save changes to the database
                        await _context.SaveChangesAsync();
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
        {
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
            var _user = await _userManager.FindByEmailAsync(Email);
            if (_user != null)
            {
                var UserRoles = (List<string>)await _userManager.GetRolesAsync(_user);

                if (selectedRoles == null)
                {
                    
                    foreach (var r in UserRoles)
                    {
                        await _userManager.RemoveFromRoleAsync(_user, r);
                    }
                }
                else
                {
                    
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
         
            if (_userManager.FindByEmailAsync(Email).Result == null)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = Email,
                    Email = Email,
                    EmailConfirmed = true 
                };
                
                string password = MakePassword.Generate();
                IdentityResult result = _userManager.CreateAsync(user, password).Result;

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
            var user = await _userManager.FindByEmailAsync(staff.Email);
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetToken));

            // Construct the reset link based on the route to your reset password page
            var resetLink = Url.Action(
                "ResetPassword",            // Action method name for resetting the password
                "Account",                  // Controller name containing the ResetPassword action
                new { code = encodedToken }, // Route values (token parameter)
                Request.Scheme              // Request scheme (HTTP or HTTPS)
            );

            // Ensure that the controller name includes the Identity folder
            resetLink = resetLink.Replace("/Account", "/Identity/Account");

            message = $"Please reset your password by clicking <a href='{HtmlEncoder.Default.Encode(resetLink)}'>here</a>.";
            try
            {
                await _emailSender.SendOneAsync(staff.FullName, staff.Email, "Account Registration", message);
                TempData["message"] = $"Invitation email sent to {staff.FullName} at {staff.Email}";
            }
            catch (Exception)
            {
                TempData["message"] = $"Could not send Invitation email to {staff.FullName} at {staff.Email}";
            }
        }



        private bool StaffExists(int id)
        {
            return _context.Staff.Any(e => e.ID == id);
        }
    }

}

