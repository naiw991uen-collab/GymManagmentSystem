using AutoMapper;
using GymManagmentSystem.BLL.Services.Sessions.ViewModel;
using GymManagmentSystem.DAL.Models;

namespace GymManagmentSystem.BLL.Services.Sessions.Mapping;

public class SessionProfile : Profile
{
    public SessionProfile()
    {
        CreateMap<Session, SessionViewModel>()
            .ForMember(dest => dest.TrainerName, opt => opt.MapFrom(src => src.Trainer.Name))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName));

        CreateMap<Trainer, TrainerSelectedViewModel>();
        CreateMap<Category, CategorySelectedViewModel>();

        CreateMap<CreateSessionViewModel, Session>()
            .ForMember(dest => dest.Discription, opt => opt.MapFrom(src => src.Description));

        // GetSessionDetailsAsync
        CreateMap<Session, SessionDetailsViewModel>()
            .ForMember(dest => dest.TrainerName, opt => opt.MapFrom(src => src.Trainer.Name))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Discription))
            .ForMember(dest => dest.AvailableSlots, opt => opt.Ignore());

        CreateMap<Session, SessionToUpdateViewModel>();

        CreateMap<SessionToUpdateViewModel, Session>()
            .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
            .ForMember(dest => dest.Capacity, opt => opt.Ignore());
    }
}