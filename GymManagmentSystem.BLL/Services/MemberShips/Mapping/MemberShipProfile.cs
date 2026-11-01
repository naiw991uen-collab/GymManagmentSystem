using AutoMapper;
using GymManagmentSystem.BLL.Services.MemberShips.ViewModel;
using GymManagmentSystem.DAL.Models;

namespace GymManagmentSystem.BLL.Services.MemberShips.Mapping
{
    public class MemberShipProfile : Profile
    {
        public MemberShipProfile()
        {
            CreateMap<CreateMemberShipViewModel, MemberShip>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.EndDate, opt => opt.Ignore());

            CreateMap<MemberShip, MemberShipForMemberViewModel>()
                .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Member.Name))
                .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.Plan.Name))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<MemberShip, MemberShipViewModel>()
                .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Member.Name))
                .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.Plan.Name))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<Member, MemberSelectListViewModel>();
            CreateMap<Plan, PlanSelectListViewModel>();
        }
    }
}
