using GymManagmentSystem.DAL.Applications;
using GymManagmentSystem.DAL.Contexts;
using GymManagmentSystem.DAL.DataSeed;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymManagmentSystem.PL.Extentions;
public static class ProgramExtention
{
    public static async Task SeedDataAndApplyPendingMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
            await dbContext.Database.MigrateAsync();

        var seedFolderPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "SeedData", "PlanModel");

        await PlanDataSeeding.SeedAsync(dbContext, seedFolderPath, logger);

        await IdentityDataSeeding.SeedIdentityDataAsync(roleManager, userManager, logger);
    }
}
