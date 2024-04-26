using AWC.Company.API.DependencyInjection;
using AWC.Company.API.Middleware;


namespace AWC.Company.API;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.ConfigureCors();
        services.AddMediatrServices();
        services.AddFluentValidators();
        services.AddMappings();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

    }

    public void Configure(WebApplication app, IWebHostEnvironment env)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseHsts();
        }

        app.UseRouting();
        app.UseCors("CorsPolicy");
        // app.MapPersonEndpoints();
        app.MapDefaultEndpoints();
        app.Run();
    }
}