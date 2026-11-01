using GymManagmentSystem.DAL.Applications;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymManagmentSystem.DAL.DataSeed;
public static class IdentityDataSeeding
{
    public static async Task  SeedIdentityDataAsync(RoleManager<IdentityRole> roleManager , 
        UserManager<ApplicationUser> userManager , ILogger logger)
    {
        try
        {
            logger.LogInformation("Identity data seeding started.");

            var hasUsers = await userManager.Users.AnyAsync();
            var hasRoles = await roleManager.Roles.AnyAsync();

            if (hasUsers && hasRoles)
            {
                logger.LogInformation("Seeding skipped: users and roles already exist.");
                return;
            }

            if (!hasRoles)
            {
                var roles = new List<IdentityRole>()
            {
                new IdentityRole("SuperAdmin"),
                new IdentityRole("Admin")
            };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role.Name!))
                    {
                        var roleResult = await roleManager.CreateAsync(role);
                        if (!roleResult.Succeeded)
                        {
                            logger.LogError("Failed to Create Role {Role}: {Errors}", role.Name,
                                string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                        }
                        else
                        {
                            logger.LogInformation("Role {Role} created successfully", role.Name);
                        }
                    }
                    else
                    {
                        logger.LogWarning("Role {Role} already exists", role.Name);
                    }
                }
            }

            if (!hasUsers)
            {
                var MainAdmin = new ApplicationUser()
                {
                    FirstName = "Nasr",
                    LastName = "Mohamed",
                    Email = "NasrMohamed@gmail.com",
                    UserName = "NasrMohamed",
                    PhoneNumber = "1234567890"
                };

                var mainAdminResult = await userManager.CreateAsync(MainAdmin, "P@ssw0rd");

                if (mainAdminResult.Succeeded)
                {
                    logger.LogInformation("User {UserName} created successfully", MainAdmin.UserName);

                    var mainAdminRoleResult = await userManager.AddToRoleAsync(MainAdmin, "SuperAdmin");
                    if (mainAdminRoleResult.Succeeded)
                        logger.LogInformation("User {UserName} added to role SuperAdmin", MainAdmin.UserName);
                    else
                        logger.LogError("Failed to add user {UserName} to role SuperAdmin: {Errors}", MainAdmin.UserName,
                            string.Join(", ", mainAdminRoleResult.Errors.Select(e => e.Description)));
                }
                else
                {
                    logger.LogError("Failed to create user {UserName}: {Errors}", MainAdmin.UserName,
                        string.Join(", ", mainAdminResult.Errors.Select(e => e.Description)));
                }

                var Admin = new ApplicationUser()
                {
                    FirstName = "Zead",
                    LastName = "Sherif",
                    Email = "ZeadSherif@gmail.com",
                    UserName = "ZeadSherif",
                    PhoneNumber = "1234567891"
                };

                var adminResult = await userManager.CreateAsync(Admin, "P@ssw0rd");

                if (adminResult.Succeeded)
                {
                    logger.LogInformation("User {UserName} created successfully", Admin.UserName);

                    var adminRoleResult = await userManager.AddToRoleAsync(Admin, "Admin");
                    if (adminRoleResult.Succeeded)
                        logger.LogInformation("User {UserName} added to role Admin", Admin.UserName);
                    else
                        logger.LogError("Failed to add user {UserName} to role Admin: {Errors}", Admin.UserName,
                            string.Join(", ", adminRoleResult.Errors.Select(e => e.Description)));
                }
                else
                {
                    logger.LogError("Failed to create user {UserName}: {Errors}", Admin.UserName,
                        string.Join(", ", adminResult.Errors.Select(e => e.Description)));
                }
            }
            logger.LogInformation("Identity data seeding finished.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while seeding identity data.");
        }
    }
}
