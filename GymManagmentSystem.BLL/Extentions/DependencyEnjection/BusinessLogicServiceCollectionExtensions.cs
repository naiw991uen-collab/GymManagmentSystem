using FluentValidation;
using GymManagmentSystem.BLL.Services.Members.Classes;
using GymManagmentSystem.BLL.Services.Members.Mapping;
using GymManagmentSystem.BLL.Services.Members.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace GymManagmentSystem.BLL.Extentions.DependencyEnjection;
public static class BusinessLogicServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblyOf<MemberService>()
            .AddClasses(c => c.Where(t => t.Name.EndsWith("Service")))
                .AsMatchingInterface()
                .WithScopedLifetime());

        services.AddAutoMapper(x => x.AddMaps(typeof(MemberProfile).Assembly));

        services.AddValidatorsFromAssemblyContaining<CreateMemberViewModelValidator>();

        return services;
    }
}
