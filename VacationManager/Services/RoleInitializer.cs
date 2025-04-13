using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using VacationManager.Models;

namespace VacationManager.Services
{
    public class RoleInitializer
    {
        public static async Task InitializeRolesAndAdmin(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminEmail = "admin@vacation.com";
            string adminPassword = "Admin123!";

            // 🔹 Проверява дали ролите вече съществуват, ако не – създава ги
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("TeamLead"))
                await roleManager.CreateAsync(new IdentityRole("TeamLead"));

            if (!await roleManager.RoleExistsAsync("Developer"))
                await roleManager.CreateAsync(new IdentityRole("Developer"));

            // 🔹 Проверява дали има админ
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var user = new User { UserName = "admin", Email = adminEmail, FirstName = "Admin", LastName = "User" };
                var result = await userManager.CreateAsync(user, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}
