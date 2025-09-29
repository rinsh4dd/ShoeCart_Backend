using Microsoft.EntityFrameworkCore;
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
        public async Task<ProductDTO> AddProductAsync(CreateProductDTO dto)
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

            return MapToDTO(product);
        }

        // ================= Update Product =================
        public async Task<ProductDTO> UpdateProductAsync(UpdateProductDTO dto)
        {
            var product = await _context.Products
                .Include(p => p.AvailableSizes)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == dto.Id);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            // Update basic fields
            product.Name = dto.Name;
            product.Brand = dto.Brand;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.CategoryId = dto.CategoryId;
            product.CurrentStock = dto.CurrentStock;
            product.InStock = dto.CurrentStock > 0;

            if (dto.IsActive.HasValue)
                product.IsActive = dto.IsActive.Value;

            // Update sizes
            product.AvailableSizes.Clear();
            product.AvailableSizes = dto.AvailableSizes.Select(s => new ProductSize { Size = s }).ToList();

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
            return MapToDTO(product);
        }

        // ================= Get Product by Id =================
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

        // ================= Get Products by Category =================
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

        // ================= Get All Products =================
        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            var products = await _context.Products
                .Include(p => p.AvailableSizes)
                .Include(p => p.Images)
                .Include(p => p.Category)
                .ToListAsync();

            return products.Select(MapToDTO).ToList();
        }

        // ================= Delete Product =================
        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        // ================= Helper: Map Product → ProductDTO =================
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
                CategoryName = p.Category?.Name,
                AvailableSizes = p.AvailableSizes.Select(s => s.Size).ToList(),
                ImageBase64 = p.Images
                    .Select(i => $"data:{i.ImageMimeType};base64,{Convert.ToBase64String(i.ImageData)}")
                    .ToList()
            };
        }
    }
}
