using Microsoft.AspNetCore.Identity;
using VehicleParts.Domain.Enums;

namespace VehicleParts.Infrastructure.Data;

public static class RoleSeeder
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, UserManager<VehicleParts.Domain.Entities.AppUser> userManager)
    {
        var roles = new[] { UserRoles.Admin, UserRoles.Staff, UserRoles.Customer };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminEmail = "admin@example.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new VehicleParts.Domain.Entities.AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Administrator",
                PhoneNumber = "1234567890"
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, UserRoles.Admin);
            }
        }
    }
}
