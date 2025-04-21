using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLogicLayer.DTO;

namespace BusinessLogicLayer.Mappers
{
    public class UserDtoOrderResponseMappingProfile:Profile
    {
        public UserDtoOrderResponseMappingProfile()
        {

            CreateMap<UserDto, OrderResponse>()
                .ForMember(dest => dest.UserPersonName, opt => opt.MapFrom(src => src.PersonName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
        }
    }
}
