using GymManagmentSystem.DAL.Contexts;
using GymManagmentSystem.DAL.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GymManagmentSystem.DAL.DataSeed;
public static class PlanDataSeeding
{
    public static async Task SeedAsync(GymDbContext dbContext , string SeedFolderPath , ILogger logger)
    {
        try
        {
            if(!dbContext.Plans.Any())
            {
                var plans = LoadDataFromJsonFile<Plan>(SeedFolderPath, "plans.json");

                if (plans.Any())
                {
                    dbContext.Plans.AddRange(plans);
                    logger.LogInformation($"Plans Seeded with Count = {plans.Count}");
                }
                
                if(dbContext.ChangeTracker.HasChanges())
                    await dbContext.SaveChangesAsync();
                else
                    logger.LogInformation("Plans Seeded Already");
            }
        }
        catch(Exception ex) 
        {
            logger.LogError(ex, "Plan Data Seed Failed");
        }
    }

    private static List<T> LoadDataFromJsonFile<T>(string FolderPath, string FileName)
    {
        // D:\MVC\Demo MVC 01\GymManagmentSystem\GymManagmentSystem.MVC\wwwroot\SeedData\PlanModel\
        // D:\MVC\Demo MVC 01\GymManagmentSystem\GymManagmentSystem.MVC\wwwroot\SeedData\PlanModel\plans.json

        var filePath = Path.Combine(FolderPath, FileName);

        if(!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Seed Data File Not Found : {filePath}");
        }

        var data = File.ReadAllText(filePath);

        var returnedData = JsonSerializer.Deserialize<List<T>>(data, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
        });

        return returnedData ?? [];
    }

}
