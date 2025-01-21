using Common.Logging;
using Serilog;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Persistence;
using Ordering.Application;
using Ordering.API.Extensions;
using Contracts.Messages;
using Infrastructure.Messages;
using Contracts.Common.Interfaces;
using Infrastructure.Common;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Starting Ordering API up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog(Serilogger.Configure);
    // Add services to the container.
    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddScoped<IMessageProducer, RabbitMQProducer>();
    builder.Services.AddScoped<ISerializerService,SerializerService>();



    builder.Services.AddApplicationServices();
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();


    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    using (var scope = app.Services.CreateScope())
    {
        var orderContextSeed = scope.ServiceProvider.GetRequiredService<OrderContextSeed>();
        await orderContextSeed.InitializeAsync();
        await orderContextSeed.SeedAsync();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal))
    {
        throw;
    }
    Log.Fatal(ex, "Unhandler exception");

}
finally
{
    Log.Information("Shut down Ordering API complete");
    Log.CloseAndFlush();
}
