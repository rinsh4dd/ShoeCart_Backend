using AutoMapper;
using ShoeCartBackend.DTOs;
using ShoeCartBackend.DTOs.CategoryDTO;
using ShoeCartBackend.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Source -> Target
        CreateMap<Product, ProductDTO>();
        CreateMap<Category, CategoryDTO>();

        // For creating/updating, you might map from a DTO to the entity
        //CreateMap<CreateProductDTO, Product>();
    }
}