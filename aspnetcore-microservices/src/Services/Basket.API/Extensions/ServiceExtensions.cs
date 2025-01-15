using Basket.API.Repositories.Interfaces;
using Basket.API.Repositories;
using Contracts.Common.Interfaces;
using Infrastructure.Common;

namespace Basket.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddInfrastructure(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddControllers();
            service.AddEndpointsApiExplorer();
            service.AddSwaggerGen();

            service.ConfigureRedis(configuration);
            service.AddInfrastructureService();
        }

        private static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConnectionString = configuration.GetSection("CacheSettings:ConnectionString").Value;
            if (string.IsNullOrEmpty(redisConnectionString))
            {
                throw new ArgumentException("Redis Conenction string is not configured!");
            }

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
            });
        }

        private static void AddInfrastructureService(this IServiceCollection services)
        {
            services.AddScoped<IBasketRepository, BasketRepository>()
                   .AddTransient<ISerializerService, SerializerService>();
        }
    }
}
