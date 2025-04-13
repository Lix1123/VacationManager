using Microsoft.AspNetCore.Identity;
using VacationManager.Models;

namespace VacationManager.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Създаване на роли
            string[] roles = { "Admin", "TeamLead", "Developer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Създаване на админ потребител
            string adminEmail = "admin@vacation.com";
            string adminUserName = "admin";
            string adminPassword = "Admin123!";

            if (await userManager.FindByNameAsync(adminUserName) == null)
            {
                var admin = new User
                {
                    UserName = adminUserName,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
                else
                {
                    throw new Exception("Admin creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
