using GymManagmentSystem.DAL.Contexts;
using GymManagmentSystem.DAL.Repositories.Classes;
using GymManagmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GymManagmentSystem.DAL.Extentions.DependencyEnjection;
public static class DataAccessServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<GymDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.Scan(scan => scan
        .FromAssemblyOf<MemberRepository>()
        .AddClasses(c => c.Where(t => t.Name.EndsWith("Repository")))
        .AsMatchingInterface()
        .WithScopedLifetime());

        return services;
    }
}
