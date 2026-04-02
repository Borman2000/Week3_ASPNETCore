using AutoMapper;
using NotificationAPI.Domain;
using NotificationAPI.Domain.Entities;

namespace NotificationAPI.Application.Mappings;

public class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
	    CreateMap<Notification, NotificationRequest>().ReverseMap();
	    CreateMap<Notification, NotificationResult>().ReverseMap();
    }
}