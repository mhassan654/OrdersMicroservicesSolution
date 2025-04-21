using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLogicLayer.DTO;
using DataAccessLayer.Entities;

namespace BusinessLogicLayer.Mappers
{
    public class ProductDtoToOrderItemResponseMappingProfile :Profile
    {
        public ProductDtoToOrderItemResponseMappingProfile() {
            CreateMap<ProductDto, OrderItemResponse>()
          .ForMember(
              dest => dest.ProductName,
              opt =>
                  opt.MapFrom(src => src.ProductName))
          .ForMember(dest => dest.Category,
              opt =>
                  opt.MapFrom(src => src.Category));
        }
    }
}
