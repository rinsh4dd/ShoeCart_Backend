using Microsoft.Extensions.DependencyInjection;
using ShoeCartBackend.Services;
using ShoeCartBackend.Services.Implementations;
using ShoeCartBackend.Services.Interfaces;

namespace ShoeCartBackend.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICartService, CartService>();

            return services;
        }
    }
}
