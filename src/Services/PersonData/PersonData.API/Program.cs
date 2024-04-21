using Serilog;
using AWC.PersonData.API.Middleware;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting PersonData.API microservice");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration)
    );

    builder.AddServiceDefaults();

    var startup = new AWC.PersonData.API.Startup(builder.Configuration);
    startup.ConfigureServices(builder.Services);

    var app = builder.Build();

    app.UseMiddleware<RequestLogContextMiddleware>();
    app.UseSerilogRequestLogging();
    app.UseExceptionHandler();

    startup.Configure(app, builder.Environment);

}
catch (Exception ex)
{
    Log.Fatal(ex, "PersonData.API microservice terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}

namespace AWC.PersonData.API
{
    public partial class Program
    {

    }
}


