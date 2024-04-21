using AWC.PersonData.API.DependencyInjection;
using AWC.PersonData.API.Middleware;
using AWC.PersonData.API.Web.Endpoints;
using AWC.PersonData.API.Infrastructure.Persistence.Interfaces;
using AWC.PersonData.API.Infrastructure.Persistence.Caching;

namespace AWC.PersonData.API;

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
        services.AddPersistence();
        services.AddMappings();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddDistributedMemoryCache();
        services.AddSingleton<ICacheService, CacheService>();
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
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseRouting();
        app.UseCors("CorsPolicy");
        app.MapPersonEndpoints();
        app.MapDefaultEndpoints();
        app.Run();
    }
}
