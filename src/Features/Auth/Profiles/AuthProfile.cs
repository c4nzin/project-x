using AutoMapper;
using src.Features.Auth.Dtos;
using src.features.user.entities;

namespace src.Features.Auth.Profiles;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<RegisterUserDto, User>()
            .ForSourceMember(x => x.Password, opt => opt.DoNotValidate())
            .ForMember(x => x.UserName, opt => opt.MapFrom(x => x.Name));

        CreateMap<User, RegisterUserDto>()
            .ForMember(x => x.Name, opt => opt.MapFrom(x => x.UserName));
    }
}
