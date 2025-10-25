using AutoMapper;
using ShoeCartBackend.DTOs;
using ShoeCartBackend.Models;
using System.Linq;

public class GeneralMappingProfile : Profile
{
    public GeneralMappingProfile()
    {
        // DTO → Entity (for Add/Update)
        CreateMap<CreateProductDTO, Product>()
            .ForMember(dest => dest.AvailableSizes,
                       opt => opt.MapFrom(src => src.AvailableSizes.Select(s => new ProductSize { Size = s }).ToList()))
            .ForMember(dest => dest.Images, opt => opt.Ignore()) // handled in service
            .ForMember(dest => dest.InStock, opt => opt.Ignore()) // handled in service
            .ForMember(dest => dest.IsActive, opt => opt.Ignore()); // handled in service

        CreateMap<UpdateProductDTO, Product>()
            .ForMember(dest => dest.AvailableSizes,
                       opt => opt.MapFrom(src => src.AvailableSizes.Select(s => new ProductSize { Size = s }).ToList()))
            .ForMember(dest => dest.Images, opt => opt.Ignore())
            .ForMember(dest => dest.InStock, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore());

        // Entity → DTO (for Get operations)
        CreateMap<Product, ProductDTO>()
            .ForMember(dest => dest.AvailableSizes,
                       opt => opt.MapFrom(src => src.AvailableSizes.Select(s => s.Size)));
    }
}
