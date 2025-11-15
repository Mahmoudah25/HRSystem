using HRManagmentSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HRManagmentSystem.Data
{
    public static class Dbinitializer
    {
        public static async Task SeedAsync(HRContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {



            //SeedRoles
            string[] RolesNmaes = { "Admin", "HR", "Employee" };

            foreach (var RoleName in RolesNmaes)
            {
                if (!await roleManager.RoleExistsAsync(RoleName))
                {
                    await roleManager.CreateAsync(new ApplicationRole
                    {
                        Name = RoleName,
                        NormalizedName = RoleName.ToUpper(),
                        Description = $"{RoleName}"
                    });
                }
            }

            //Seed Admin User
            string adminEmail = "admin@hrsystem.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "Admin",
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true,
                };
                var ressult = await userManager.CreateAsync(user, "Admin@123");
                if (ressult.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }

            //Seed department & Position

            if (!context.Departments.Any())
            {
                context.Departments.Add(new Department { Name = "Human Resources" });
                context.Departments.Add(new Department { Name = "Finance" });
            }
            if (!context.Positions.Any())
            {
                context.Positions.Add(new Position { Title = "HR Manger" });
                context.Positions.Add(new Position { Title = "Accountant" });
            }
            await context.SaveChangesAsync();

        }
    }
}
