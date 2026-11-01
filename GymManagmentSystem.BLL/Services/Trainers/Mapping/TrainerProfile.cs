using AutoMapper;
using GymManagmentSystem.BLL.Services.Trainers.ViewModel;
using GymManagmentSystem.DAL.Models;

namespace GymManagmentSystem.BLL.Services.Trainers.Mapping;
public class TrainerProfile : Profile
{
    public TrainerProfile()
    {
        // GetTrainersAsync
        CreateMap<Trainer, TrainerViewMoldel>()
            .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => src.Specialty));

        // AddTrainerAsyn
        CreateMap<CreateTrainerViewModel, Trainer>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address()
            {
                BuildingNumber = src.BuildingNumber,
                City = src.City,
                Street = src.Street,
            }));

        // GetTrainerAsync
        CreateMap<Trainer, TrainerDetailsViewModel>()
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.ToString("MM/dd/yyyy")))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => $"{src.Address.BuildingNumber} - {src.Address.Street} - {src.Address.City}"))
            .ForMember(dest => dest.Specialty, opt => opt.MapFrom(src => $"{src.Specialty} Trainer"));

        // GetTrainerToUpdateAsync
        CreateMap<Trainer, TrainerToUpdateViewModel>()
            .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.Address != null ? src.Address.BuildingNumber : 0))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address != null ? src.Address.City : "Not Found"))
            .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address != null ? src.Address.Street : "Not Found"));

        // UpdateTrainerAsync
        CreateMap<TrainerToUpdateViewModel,Trainer>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.Ignore())
             .AfterMap((src, dest) =>
             {
                 dest.Address.BuildingNumber = src.BuildingNumber;
                 dest.Address.City = src.City;
                 dest.Address.Street = src.Street;
             });
    }
}
