using System;
using API.DTOs;
using API.Entities;
using API.Extentions;
using AutoMapper;

namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, MemberDTOs>()
            .ForMember(d => d.Age, o => o.MapFrom(s => s.DateOfBirth.CalculateAge()))
            .ForMember(
                d => d.PhotoUrl,
                o => o.MapFrom(s => s.Photos.FirstOrDefault(x => x.IsMain)!.Url)
            );
        CreateMap<Photo, PhotoDTOs>();
        CreateMap<MemberUpdateDTOs, AppUser>();
        CreateMap<RegisterDTOs, AppUser>();
        CreateMap<string, DateOnly>().ConvertUsing(s => DateOnly.Parse(s));
        CreateMap<Message, MessageDto>()
            .ForMember(
                d => d.SenderPhotoUrl,
                o => o.MapFrom(s => s.Sender.Photos.FirstOrDefault(x => x.IsMain)!.Url)
            )
            .ForMember(
                d => d.RecipientPhotoUrl,
                o => o.MapFrom(s => s.Recipient.Photos.FirstOrDefault(x => x.IsMain)!.Url)
            );
    }
}
