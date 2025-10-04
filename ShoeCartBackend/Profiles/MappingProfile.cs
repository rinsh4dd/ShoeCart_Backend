using AutoMapper;
using ShoeCartBackend.DTOs;
using ShoeCartBackend.DTOs.CategoryDTO;
using ShoeCartBackend.Models;

public class MappingProfile : Profile
{
     public MappingProfile()
    {
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

        CreateMap<Order, OrderDto>();
    }
}