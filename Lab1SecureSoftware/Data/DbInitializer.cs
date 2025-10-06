using Lab1SecureSoftware.Data;
using Lab1SecureSoftware.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab1SecureSoftware.Data
{
    public static class DbInitializer
    {
        public static async Task<int> SeedUsersAndRoles(IServiceProvider
        serviceProvider)
        {
            // create the database if it doesn't exist
            var context =
            serviceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var config = serviceProvider.GetRequiredService<IConfiguration>();

            string adminEmail = config["SeedUser:Admin:Email"]!;
            string adminPassword = config["SeedUser:Admin:Password"]!;
            string employeeEmail = config["SeedUser:Employee:Email"]!;
            string employeePassword = config["SeedUser:Employee:Password"]!;

            // Check if roles already exist and exit if there are
            if (roleManager.Roles.Count() > 0)
                return 1; // should log an error message here
                          // Seed roles
            int result = await SeedRoles(roleManager);
            if (result != 0)
                return 2; // should log an error message here
                          // Check if users already exist and exit if there are
            if (userManager.Users.Count() > 0)
                return 3; // should log an error message here
                          // Seed users
            result = await SeedUsers(userManager, adminEmail, adminPassword, employeeEmail, employeePassword);
            if (result != 0)
                return 4; // should log an error message here
            return 0;
        }
        private static async Task<int> SeedRoles(RoleManager<IdentityRole>
        roleManager)
        {
            // Create Admin Role
            var result = await roleManager.CreateAsync(new IdentityRole("Supervisor"));
            if (!result.Succeeded)
                return 1; // should log an error message here
                          // Create Member Role
            result = await roleManager.CreateAsync(new IdentityRole("Employee"));
            if (!result.Succeeded)
                return 2; // should log an error message here
            return 0;
        }
        private static async Task<int> SeedUsers(UserManager<ApplicationUser> userManager, string adminEmail, string adminPassword, string employeeEmail, string employeePassword)
        {
            // Create Admin User
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "The",
                LastName = "Supervisors",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (!result.Succeeded)
                return 1; // should log an error message here

            // Assign user to Admin role
            result = await userManager.AddToRoleAsync(adminUser, "Supervisor");
            if (!result.Succeeded)
                return 2; // should log an error message here

            // Create Member User
            var memberUser = new ApplicationUser
            {
                UserName = employeeEmail,
                Email = employeeEmail,
                FirstName = "The",
                LastName = "Employees",
                EmailConfirmed = true
            };

            result = await userManager.CreateAsync(memberUser, employeePassword);
            if (!result.Succeeded)
                return 3; // should log an error message here

            // Assign user to Member role
            result = await userManager.AddToRoleAsync(memberUser, "Employee");
            if (!result.Succeeded)
                return 4; // should log an error message here

            return 0;
        }
    }
}