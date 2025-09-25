using ShoeCartBackend.DTOs;
using ShoeCartBackend.Models;
using ShoeCartBackend.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoeCartBackend.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        // Get all products by category
        public async Task<IEnumerable<ProductDTO>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _repository.GetProductsByCategoryAsync(categoryId);

            return products.Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Brand = p.Brand,
                InStock = p.InStock,
                SpecialOffer = p.SpecialOffer,
                CategoryName = p.Category.Name,
                Sizes = p.AvailableSizes.Select(s => s.Size).ToList(),
                ImageUrl = p.Images.FirstOrDefault(i => i.IsMain)?.ImageData != null
                    ? $"data:{p.Images.FirstOrDefault(i => i.IsMain)?.ImageMimeType};base64," +
                      Convert.ToBase64String(p.Images.FirstOrDefault(i => i.IsMain)!.ImageData)
                    : null
            }).ToList();
        }

        // Get product by Id
        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            var p = await _repository.GetProductWithDetailsAsync(id);
            if (p == null) return null;

            return new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Brand = p.Brand,
                InStock = p.InStock,
                SpecialOffer = p.SpecialOffer,
                CategoryName = p.Category.Name,
                Sizes = p.AvailableSizes.Select(s => s.Size).ToList(),
                ImageUrl = p.Images.FirstOrDefault(i => i.IsMain)?.ImageData != null
                    ? $"data:{p.Images.FirstOrDefault(i => i.IsMain)?.ImageMimeType};base64," +
                      Convert.ToBase64String(p.Images.FirstOrDefault(i => i.IsMain)!.ImageData)
                    : null
            };
        }

        // Add new product
        public async Task<ProductDTO> AddProductAsync(ProductDTO dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Brand = dto.Brand,
                InStock = dto.InStock,
                SpecialOffer = dto.SpecialOffer,
                CategoryId = dto.CategoryId
            };

            await _repository.AddAsync(product);

            dto.Id = product.Id; // assign the generated Id
            return dto;
        }
    }
}
