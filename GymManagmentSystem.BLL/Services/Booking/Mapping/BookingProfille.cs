using AutoMapper;
using GymManagmentSystem.BLL.Services.Booking.ViewModel;
using GymManagmentSystem.BLL.Services.MemberShips.ViewModel;
using GymManagmentSystem.BLL.Services.Sessions.ViewModel;
using GymManagmentSystem.DAL.Models;

namespace GymManagmentSystem.BLL.Services.Booking.Mapping;
public class BookingProfille : Profile
{
    public BookingProfille()
    {
        CreateMap<CreateBookingViewModel, MemberSession>()
            .ForMember(dest => dest.IsAttended, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now));


        CreateMap<MemberSession, MemberForSessionViewModel>()
           .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Member.Name))
           .ForMember(dest => dest.IsAttend, opt => opt.MapFrom(src => src.IsAttended))
           .ForMember(dest => dest.BookingDays, opt => opt.MapFrom(src => src.CreatedAt.ToString("yyyy-MM-dd HH:mm")));

        CreateMap<Session, SessionViewModel>()
           .ForMember(dest => dest.TrainerName, opt => opt.MapFrom(src => src.Trainer.Name))
           .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
           .ForMember(dest => dest.AvailableSlots, opt => opt.Ignore());

        CreateMap<Member, MemberSelectListViewModel>();
    }
}
