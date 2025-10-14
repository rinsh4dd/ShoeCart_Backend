using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShoeCartBackend.Common;
using ShoeCartBackend.Data;
using ShoeCartBackend.DTOs;
using ShoeCartBackend.Models;
using ShoeCartBackend.Repositories.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShoeCartBackend.Services.Implementations
{
    public class ProductService : IProductService
    {

        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Product> _repository;
        public ProductService(AppDbContext context,IMapper mapper,IGenericRepository<Product> repository )
        {
            _context = context;
            _mapper = mapper;
            _repository = repository;
        }   
        public async Task<ApiResponse<ProductDTO>> AddProductAsync(CreateProductDTO dto)
        {
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
            var isAdded =  await _context.SaveChangesAsync()>0;

            return isAdded? new ApiResponse<ProductDTO>(200,"Product Added Successfully"):
                new ApiResponse<ProductDTO>(500, "Failed to add product");
        }

        public async Task<ApiResponse<ProductDTO>> UpdateProductAsync(UpdateProductDTO dto)
        {
            var product = await _context.Products
                .Include(p => p.AvailableSizes)
                .Include(p => p.Images)
                .SingleOrDefaultAsync(p => p.Id == dto.Id);

            if (product == null)
                return new ApiResponse<ProductDTO>(404, "Product not Found");

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

            if (dto.AvailableSizes != null && dto.AvailableSizes.Any())
            {
                product.AvailableSizes.Clear();
                product.AvailableSizes = dto.AvailableSizes.Select(s => new ProductSize { Size = s }).ToList();
            }

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
                .Where(p => p.IsActive && p.IsDeleted==false)
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

        public async Task<ApiResponse<IEnumerable<ProductDTO>>> GetFilteredProducts(
      string? name = null,
      int? categoryId = null,
      string? brand = null,
      decimal? minPrice = null,
      decimal? maxPrice = null,
      bool? inStock = null,
      int page = 1,
      int pageSize = 20,
      string? sortBy = null,
      bool descending = false)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.AvailableSizes)
                .Where(p => p.IsActive && p.IsDeleted == false)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(p => p.Name.Contains(name) || p.Category.Name.Contains(name) || p.Brand.Contains(name));
            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);
            if (!string.IsNullOrWhiteSpace(brand))
                query = query.Where(p => p.Brand.Contains(brand));
            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);
            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);
            if (inStock.HasValue)
                query = query.Where(p => p.InStock == inStock.Value);
            if (!string.IsNullOrWhiteSpace(sortBy))
                query = descending
                    ? query.OrderByDescending(p => EF.Property<object>(p, sortBy))
                    : query.OrderBy(p => EF.Property<object>(p, sortBy));

            query = query.Skip((page - 1) * pageSize).Take(pageSize);
            var products = await query.ToListAsync();
            var productDto = _mapper.Map<IEnumerable<ProductDTO>>(products);
            return new ApiResponse<IEnumerable<ProductDTO>>(200, "Filtered products successfully", productDto);
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
                    .Select(i => $"data:{i.ImageMimeType};base64," +
                    $"{Convert.ToBase64String(i.ImageData)}")
                    .ToList()
            };
        }
    }
}
