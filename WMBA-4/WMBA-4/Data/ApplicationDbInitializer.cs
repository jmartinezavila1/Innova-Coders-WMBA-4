using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace WMBA_4.Data
{
    public static class ApplicationDbInitializer
    {
        public static async void Seed(IApplicationBuilder applicationBuilder)
        {
            ApplicationDbContext context = applicationBuilder.ApplicationServices.CreateScope()
                .ServiceProvider.GetRequiredService<ApplicationDbContext>();
            try
            {
                //Create the database if it does not exist and apply the Migration
                context.Database.Migrate();
                //Create Roles
                var RoleManager = applicationBuilder.ApplicationServices.CreateScope()
                    .ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                string[] roleNames = { "Admin", "Convenor", "Coach", "Scorekeeper" };
                IdentityResult roleResult;
                foreach (var roleName in roleNames)
                {
                    var roleExist = await RoleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }
                //Create Users
                var userManager = applicationBuilder.ApplicationServices.CreateScope()
                    .ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                if (userManager.FindByEmailAsync("admin@outlook.com").Result == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = "admin@outlook.com",
                        Email = "admin@outlook.com"
                    };
                    IdentityResult result = userManager.CreateAsync(user, "Ba55eb@ll").Result;
                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Admin").Wait();
                    }
                }
                if (userManager.FindByEmailAsync("convenor@outlook.com").Result == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = "convenor@outlook.com",
                        Email = "convenor@outlook.com"
                    };
                    IdentityResult result = userManager.CreateAsync(user, "Ba55eb@ll").Result;
                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Convenor").Wait();
                    }
                }
                if (userManager.FindByEmailAsync("coach@outlook.com").Result == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = "coach@outlook.com",
                        Email = "coach@outlook.com"
                    };
                    IdentityResult result = userManager.CreateAsync(user, "Ba55eb@ll").Result;
                    
                }
                if (userManager.FindByEmailAsync("scorekeeper@outlook.com").Result == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = "scorekeeper@outlook.com",
                        Email = "scorekeeper@outlook.com"
                    };
                    IdentityResult result = userManager.CreateAsync(user, "Ba55eb@ll").Result;
                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Scorekeeper").Wait();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException().Message);
            }
        }
    }

}
