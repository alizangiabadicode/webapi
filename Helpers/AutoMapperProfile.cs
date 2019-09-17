using System;
using System.Linq;
using AutoMapper;
using datingapp.api.Dtos;
using datingapp.api.Models;

namespace datingapp.api.Helpers
{

    class AutoMapperProfile : Profile{
        public AutoMapperProfile()
        {
            var l = default(Func<User,int>);
            l = (User u) => {
                    var thisYear = DateTime.Today.Year;
                    var birthYear = u.DateOfBirth.Year;
                    int years = thisYear - birthYear;
                    if(u.DateOfBirth.AddYears(years) > DateTime.Today){
                        years--;
                    }
                    return years;
            };

            CreateMap<User, UserForDetailDTO>()
            .ForMember(dest => dest.PhotoUrl, opt => {
                opt.MapFrom(src => src.Photos.FirstOrDefault(e => e.IsMain).Url);
            }).ForMember(e=>e.Age, opt => {
                opt.MapFrom(u => l(u));
            });
            CreateMap<User, UserForLIstDTO>()
            .ForMember(e=> e.PhotoUrl, opt => {
                opt.MapFrom(sourceMember=> sourceMember.Photos.FirstOrDefault(n => n.IsMain).Url);
            }).ForMember(e=>e.Age, opt => {
                opt.MapFrom(u => l(u));
            });
            CreateMap<Photo, PhotosForDetailDTO>();
            CreateMap<UserForUpdateDTO,User>();
            CreateMap<Photo, PhotoForReturnDTO>();
            CreateMap<PhotoForCreationDTO, Photo>();
            CreateMap<UserForRegisterDTO, User>();
            CreateMap<CreateMsgDTO, Message>().ReverseMap();
            CreateMap<Message, MessageToReturnDTO>()
            .ForMember(e=>e.SenderPhotoUrl, opt => opt.MapFrom(e=>e.Sender.Photos.FirstOrDefault(b=>b.IsMain).Url))
            .ForMember(e=>e.ReciverPhotoUrl, opt => opt.MapFrom(e=>e.Reciver.Photos.FirstOrDefault(b=>b.IsMain).Url));
        }
    }
}