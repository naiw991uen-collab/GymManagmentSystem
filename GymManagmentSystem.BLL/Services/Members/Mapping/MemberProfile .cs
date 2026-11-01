using AutoMapper;
using GymManagmentSystem.BLL.Services.Members.ViewModel;
using GymManagmentSystem.DAL.Models;

namespace GymManagmentSystem.BLL.Services.Members.Mapping;
public class MemberProfile : Profile
{
    public MemberProfile()
    {
        CreateMap<Member, MemberDetailsViewModel>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => $"{src.Address.BuildingNumber} - {src.Address.Street} - {src.Address.City}"))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString()))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.ToString("d/M/yyyy")));

        CreateMap<Member, MemberViewModel>()
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString()))
            .ReverseMap();


        CreateMap<HealthRecoed, HealthRecordViewModel>().ReverseMap();

        CreateMap<Member, MemberToUpdateViewModel>()
            .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.Address.BuildingNumber))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
            .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street));

        CreateMap<MemberToUpdateViewModel, Member>()
            .ForMember(dest => dest.Name, opt => opt.Ignore())
            .ForMember(dest => dest.Photo, opt => opt.Ignore())
             .AfterMap((src, dest) =>
             {
                 dest.Address.BuildingNumber = src.BuildingNumber;
                 dest.Address.City = src.City;
                 dest.Address.Street = src.Street;
             });

        CreateMap<CreateMemberViewModel, Member>()
    .ForMember(dest => dest.Photo, opt => opt.Ignore())
    .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address()
    {
        BuildingNumber = src.BuildingNumber,
        City = src.City,
        Street = src.Street
    }))
    .ForMember(dest => dest.HealthRecoed, opt => opt.MapFrom(src => src.HealthRecordViewModel));
    }
}
