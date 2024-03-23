﻿using Microsoft.AspNetCore.Identity;
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
                    // Create the database if it does not exist and apply the Migration
                    context.Database.Migrate();

                    // Create Roles
                    var roleManager = applicationBuilder.ApplicationServices.CreateScope()
                        .ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                    string[] roleNames = { "Admin", "Coach", "Scorekeeper", "RookieConvenor", "IntermediateConvenor", "SeniorConvenor" };
                    IdentityResult roleResult;
                    foreach (var roleName in roleNames)
                    {
                        var roleExist = await roleManager.RoleExistsAsync(roleName);
                        if (!roleExist)
                        {
                            roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                        }
                    }

                    // Create Users and Assign Roles
                    var userManager = applicationBuilder.ApplicationServices.CreateScope()
                        .ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                var users = new[]
{
    new { Email = "admin@outlook.com", Role = "Admin" },
    new { Email = "michael.brown@example.com", Role = "Coach" },
    new { Email = "alexander.taylor@example.com", Role = "Scorekeeper" },
    new { Email = "rookie@outlook.com", Role = "RookieConvenor" },
    new { Email = "intermediate@outlook.com", Role = "IntermediateConvenor" },
    new { Email = "senior@outlook.com", Role = "SeniorConvenor" },

    // Add more users as needed
    new { Email = "john.doe@example.com", Role = "Coach" },
    new { Email = "jane.smith@example.com", Role = "Coach" },
    new { Email = "robert.johnson@example.com", Role = "Coach" },
    new { Email = "emily.williams@example.com", Role = "Coach" },
    new { Email = "olivia.miller@example.com", Role = "Coach" },
    new { Email = "william.davis@example.com", Role = "Coach" },
    new { Email = "sophia.garcia@example.com", Role = "Coach" },
    new { Email = "james.martinez@example.com", Role = "Coach" },
    new { Email = "emma.jackson@example.com", Role = "Coach" },
    new { Email = "ava.anderson@example.com", Role = "Scorekeeper" },
    new { Email = "daniel.thomas@example.com", Role = "Scorekeeper" },
    new { Email = "mia.moore@example.com", Role = "Scorekeeper" },
    new { Email = "ethan.clark@example.com", Role = "Scorekeeper" },
    new { Email = "isabella.lewis@example.com", Role = "Scorekeeper" },
    new { Email = "benjamin.hill@example.com", Role = "Scorekeeper" },
    new { Email = "avery.king@example.com", Role = "Scorekeeper" },
    new { Email = "ryan.cooper@example.com", Role = "Scorekeeper" },
    new { Email = "nora.baker@example.com", Role = "Scorekeeper" }
};

                foreach (var user in users)
                    {
                        var existingUser = await userManager.FindByEmailAsync(user.Email);
                        if (existingUser == null)
                        {
                            var newUser = new IdentityUser
                            {
                                UserName = user.Email,
                                Email = user.Email,
                                EmailConfirmed = true
                            };
                            var createUserResult = await userManager.CreateAsync(newUser, "Ba55eb@ll");
                            if (createUserResult.Succeeded)
                            {
                                await userManager.AddToRoleAsync(newUser, user.Role);
                            }
                            else
                            {
                                throw new Exception($"Error creating user: {user.Email}");
                            }
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
