using Serilog;
using Web.Bff.Middleware;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting web gateway");

    var builder = WebApplication.CreateBuilder(args);

    builder.AddServiceDefaults();

    builder.Host.UseSerilog((context, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration)
    );

    builder.Services.AddReverseProxy()
        .LoadFromConfig(builder.Configuration
        .GetSection("ReverseProxy"))
        .AddServiceDiscoveryDestinationResolver();

    var app = builder.Build();

    app.UseMiddleware<RequestLogContextMiddleware>();
    app.UseSerilogRequestLogging();
    // app.UseExceptionHandler();
    app.MapReverseProxy();

    app.Run();
}
finally
{
    Log.CloseAndFlush();
}
