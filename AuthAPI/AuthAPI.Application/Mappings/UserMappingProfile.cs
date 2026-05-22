using AuthAPI.Application.DTOs;
using AuthAPI.Domain.Entities;
using AutoMapper;

namespace AuthAPI.Application.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
	    CreateMap<UserDto, ApplicationUser>().ConstructUsing((source, dst) =>
	    {
		    return new ApplicationUser{UserName = source.UserName, Email = source.UserName};
	    });

	    CreateMap<ApplicationUser, UserDto>().ConstructUsing((source, dst) =>
	    {
		    return new UserDto{Id = source.Id, UserName = source.UserName};
	    });
    }
}