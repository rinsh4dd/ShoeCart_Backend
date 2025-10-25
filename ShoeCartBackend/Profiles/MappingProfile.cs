
using AutoMapper;
using ShoeCartBackend.Models;
using ShoeCartBackend.DTOs;

namespace ShoeCartBackend.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size));
              

            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.BillingStreet, opt => opt.MapFrom(src => src.BillingStreet))
                .ForMember(dest => dest.BillingCity, opt => opt.MapFrom(src => src.BillingCity))
                .ForMember(dest => dest.BillingState, opt => opt.MapFrom(src => src.BillingState))
                .ForMember(dest => dest.BillingZip, opt => opt.MapFrom(src => src.BillingZip))
                .ForMember(dest => dest.BillingCountry, opt => opt.MapFrom(src => src.BillingCountry))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus.ToString()))
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus.ToString()))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
        }
    }
}
