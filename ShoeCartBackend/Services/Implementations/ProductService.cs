using Microsoft.EntityFrameworkCore;
using ShoeCartBackend.Common;
using ShoeCartBackend.Data;
using ShoeCartBackend.DTOs;
using ShoeCartBackend.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShoeCartBackend.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        // ================= Add Product =================
        public async Task<ApiResponse<ProductDTO>> AddProductAsync(CreateProductDTO dto)
        {
            // Map basic fields
            var product = new Product
            {
                Name = dto.Name,
                Brand = dto.Brand,
                Description = dto.Description,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                CurrentStock = dto.CurrentStock,
                InStock = dto.CurrentStock > 0,
                SpecialOffer = dto.SpecialOffer,
                IsActive = true,
                AvailableSizes = dto.AvailableSizes.Select(s => new ProductSize { Size = s }).ToList(),
                Images = new List<ProductImage>()
            };

            // Process uploaded images
            foreach (var file in dto.Images)
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                product.Images.Add(new ProductImage
                {
                    ImageData = ms.ToArray(),
                    ImageMimeType = file.ContentType
                });
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return new ApiResponse<ProductDTO>(200,"Product Added Successfully");
        }

        // ================= Update Product =================
        public async Task<ApiResponse<ProductDTO>> UpdateProductAsync(UpdateProductDTO dto)
        {
            var product = await _context.Products
                .Include(p => p.AvailableSizes)
                .Include(p => p.Images)
                .SingleOrDefaultAsync(p => p.Id == dto.Id);

            if (product == null)
                return new ApiResponse<ProductDTO>(404, "Product not Found");

            // Update only if values are provided
            if (!string.IsNullOrWhiteSpace(dto.Name)) product.Name = dto.Name.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Description)) product.Description = dto.Description.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Brand)) product.Brand = dto.Brand.Trim();
            if (!string.IsNullOrWhiteSpace(dto.SpecialOffer)) product.SpecialOffer = dto.SpecialOffer.Trim();
            if (dto.Price.HasValue) product.Price = dto.Price.Value;
            if (dto.CategoryId.HasValue) product.CategoryId = dto.CategoryId.Value;
            if (dto.CurrentStock.HasValue)
            {
                product.CurrentStock = dto.CurrentStock.Value;
                product.InStock = dto.CurrentStock.Value > 0;
            }

            if (dto.IsActive.HasValue)
                product.IsActive = dto.IsActive.Value;

            // Update sizes if provided
            if (dto.AvailableSizes != null && dto.AvailableSizes.Any())
            {
                product.AvailableSizes.Clear();
                product.AvailableSizes = dto.AvailableSizes.Select(s => new ProductSize { Size = s }).ToList();
            }

            // Add new images if any
            if (dto.NewImages != null && dto.NewImages.Any())
            {
                foreach (var file in dto.NewImages)
                {
                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    product.Images.Add(new ProductImage
                    {
                        ImageData = ms.ToArray(),
                        ImageMimeType = file.ContentType
                    });
                }
            }

            await _context.SaveChangesAsync();
            return new ApiResponse<ProductDTO>(200,"Product Updated Successfully");
        }

        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.AvailableSizes)
                .Include(p => p.Images)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return null;
            return MapToDTO(product);
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _context.Products
                .Include(p => p.AvailableSizes)
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();

            return products.Select(MapToDTO).ToList();
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            var products = await _context.Products
                .Include(p => p.AvailableSizes)
                .Include(p => p.Images)
                .Include(p => p.Category)
                .ToListAsync();

            return products.Select(MapToDTO).ToList();
        }

        public async Task<ApiResponse<string>> ToggleProductStatusAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return new ApiResponse<string>(404, "Product not found");
            }
            product.IsActive = !product.IsActive;
           
            product.IsDeleted = !product.IsDeleted;
            await _context.SaveChangesAsync();

            if (product.IsActive==true && product.IsDeleted == false)
            {
                return new ApiResponse<string>(200, "Product Activated Successfully");
            }
            else
            {
                return new ApiResponse<string>(200, "Product Deactivated Successfully");
            }
           

        }
        private ProductDTO MapToDTO(Product p)
        {
            return new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Brand = p.Brand,
                Description = p.Description,
                Price = p.Price,
                InStock = p.InStock,
                CurrentStock = p.CurrentStock,
                SpecialOffer = p.SpecialOffer,
                CategoryId =p.CategoryId,
                CategoryName = p.Category?.Name,
                AvailableSizes = p.AvailableSizes.Select(s => s.Size).ToList(),
                ImageBase64 = p.Images
                    .Select(i => $"data:{i.ImageMimeType};base64,{Convert.ToBase64String(i.ImageData)}")
                    .ToList()
            };
        }
    }
}
