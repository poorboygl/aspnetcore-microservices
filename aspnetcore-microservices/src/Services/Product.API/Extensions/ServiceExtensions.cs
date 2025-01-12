using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Product.API.Persistence;

namespace Product.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.ConfigureProductDbContext(configuration);

        return services;
    }

    private static IServiceCollection ConfigureProductDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnectionString");
        var builder = new MySqlConnectionStringBuilder(connectionString); // Build ra 1 chuỗi ConnectString chuẩn cho mysql.

        services.AddDbContext<ProductContext>(m => m.UseMySql(builder.ConnectionString,
            ServerVersion.AutoDetect(builder.ConnectionString), e => // Tự động auto Detect version
            {
                e.MigrationsAssembly("Product.API"); // Migration sẽ tạo Migration ra project nào
                e.SchemaBehavior(MySqlSchemaBehavior.Ignore);
            }));

        return services;
    }
}


