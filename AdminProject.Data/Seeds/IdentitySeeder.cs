using AdminProject.Data.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AdminProject.Data.Seeds;

public static class IdentitySeeder
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        // Define roles to seed
        var roles = new[] { "Admin", "Default_Pages" };

        // Seed roles
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Define the admin user details
        var adminEmail = "admin@gmail.com";
        var adminPassword = "Admin@123";

        // Check if the admin user already exists
        var userExist = await userManager.FindByEmailAsync(adminEmail);
        if (userExist == null)
        {
            var adminUser = new User
            {
                UserName = "admin",
                Email = adminEmail,
                PhoneNumber = "123",
                EmailConfirmed = true
            };

            // Create the admin user
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                // Assign the Admin role to the user
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
            else
            {
                throw new Exception("Failed to create the admin user: " + string.Join(", ", result.Errors));
            }
        }
    }

    public static async Task SeedClaimAsync(IServiceProvider serviceProvider)
    {
        
    }
}
