using Basket.API;
using Basket.API.Extensions;
using Common.Logging;
using Serilog;


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

Log.Information($"Start {builder.Environment.ApplicationName} up");

try
{
    builder.Host.UseSerilog(Serilogger.Configure);

    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));

    var app = builder.Build();

    app.UseInfrastructure();

    app.Run();
}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal))
    {
        throw;
    }
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
