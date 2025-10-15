using Microsoft.Extensions.DependencyInjection;
using ShoeCartBackend.Repositories;
using ShoeCartBackend.Repositories.Implementations;
using ShoeCartBackend.Repositories.Interfaces;

namespace ShoeCartBackend.Extensions
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IWishlistRepository, WishlistRepository>();
            return services;
        }
    }
}
