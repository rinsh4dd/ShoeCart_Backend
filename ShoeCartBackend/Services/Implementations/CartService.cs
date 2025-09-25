using Microsoft.EntityFrameworkCore;
using ShoeCartBackend.Data;
using ShoeCartBackend.Models;

public class CartService : ICartService
{
    private readonly AppDbContext _context;
    private readonly IProductRepository _productRepository;

    public CartService(AppDbContext context, IProductRepository productRepository)
    {
        _context = context;
        _productRepository = productRepository;
    }

    public async Task AddToCartAsync(int userId, int productId, string size, int quantity)
    {
        var product = await _productRepository.GetProductWithDetailsAsync(productId);
        if (product == null)
            throw new Exception("Product not found");

        // Get user cart (cart for this user)
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);

        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId
            };
            _context.Carts.Add(cart);
        }

        // Check if the product + size already exists
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId && i.Size == size);

        if (existingItem != null)
        {
            // Increase quantity
            existingItem.Quantity += quantity;
        }
        else
        {
            // Add new item
            var mainImage = product.Images.FirstOrDefault(i => i.IsMain);
            cart.Items.Add(new CartItem
            {
                ProductId = product.Id,
                Name = product.Name,
                Price = product.Price,
                Size = size,
                Quantity = quantity,
                ImageData = mainImage?.ImageData,
                ImageMimeType = mainImage?.ImageMimeType
            });
        }

        await _context.SaveChangesAsync(); }


        public async Task<Cart?> GetCartByUserIdAsync(int userId)
    {
        return await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.product)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);
    }

    public async Task UpdateCartItemAsync(int userId, int cartItemId, int quantity)
    {
        if (quantity < 1 || quantity > 5)
            throw new Exception("Quantity must be between 1 and 5");

        var cartItem = await _context.CartItems
            .Include(ci => ci.Cart)
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart.UserId == userId);

        if (cartItem == null)
            throw new Exception("Cart item not found");

        cartItem.Quantity = quantity;
        await _context.SaveChangesAsync();
    }



    public async Task RemoveCartItemAsync(int userId, int cartItemId)
    {
        var cartItem = await _context.CartItems
            .Include(ci => ci.Cart)
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart.UserId == userId);

        if (cartItem == null) throw new Exception("Cart item not found");
        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();
    }

    public async Task ClearCartAsync(int userId)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);

        if (cart != null)
        {
            _context.CartItems.RemoveRange(cart.Items);
            await _context.SaveChangesAsync();
        }
    }

}
    


