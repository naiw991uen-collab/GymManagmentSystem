using AutoMapper;
using GymManagmentSystem.BLL.Services.Plansa.ViewModel;
using GymManagmentSystem.DAL.Models;

namespace GymManagmentSystem.BLL.Services.Plans.Mapping;
public class PlanProfile : Profile
{
    public PlanProfile()
    {
        // GetPlanAsync
        CreateMap<Plan, EditPlanViewModel>()
            .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.Name));

        // UpdatePlanAsync
        CreateMap<EditPlanViewModel, Plan>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.Ignore());
    }
}
