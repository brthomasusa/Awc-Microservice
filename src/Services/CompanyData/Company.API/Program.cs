using Serilog;
using AWC.Company.API.Middleware;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting CompanyData.API microservice");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration)
    );

    builder.AddServiceDefaults();

    var startup = new AWC.Company.API.Startup(builder.Configuration);
    startup.ConfigureServices(builder.Services);

    var app = builder.Build();

    app.UseMiddleware<RequestLogContextMiddleware>();
    app.UseSerilogRequestLogging();
    app.UseExceptionHandler();

    startup.Configure(app, builder.Environment);

}
catch (Exception ex)
{
    Log.Fatal(ex, "CompanyData.API microservice terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}

namespace AWC.Company.API
{
    public partial class Program
    {

    }
}
