using Basket.API.Repositories.Interfaces;
using Basket.API.Repositories;
using Contracts.Common.Interfaces;
using Infrastructure.Common;
using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared.Configurations;
using Infrastructure.Extensions;
using EventBus.Messages.IntergrationEvents.Events;


namespace Basket.API.Extensions;

public static class ServiceExtensions
{
    public static void AddInfrastructure(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddControllers();
        service.AddEndpointsApiExplorer();
        service.AddSwaggerGen();

        service.ConfigureRedis();
        service.ConfigureMassTransit();
        service.AddInfrastructureService();
    }

    private static void ConfigureRedis(this IServiceCollection services)
    {
        var settings = services.GetOptions<CacheSettings>(nameof(CacheSettings));
        if (string.IsNullOrEmpty(settings.ConnectionStrings))
        {
            throw new ArgumentException("Redis Conenction string is not configured!");
        }

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = settings.ConnectionStrings;
        });
    }

    private static void AddInfrastructureService(this IServiceCollection services)
    {
        services.AddScoped<IBasketRepository, BasketRepository>()
               .AddTransient<ISerializerService, SerializerService>();
    }

    private static void ConfigureMassTransit(this IServiceCollection services)
    {
        var settings = services.GetOptions<EventBusSettings>(nameof(EventBusSettings));
        if (settings == null || string.IsNullOrEmpty(settings.HostAddress))
        {
            throw new ArgumentException("EventBusSettings is not configured!");
        }

        var mqConnection = new Uri(settings.HostAddress);
        //format "BasketCheckoutEventQueue" => "basket-checkout-event-queue"
        services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);

        services.AddMassTransit(config =>
        {
            config.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(mqConnection);
            });
            // Publish submit order message, instead of sending it to a specific queue directly.
            config.AddRequestClient<BasketCheckoutEvent>();
        });
    }
}
