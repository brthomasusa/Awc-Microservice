#pragma warning disable CS8604

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using AWC.Company.API.Application.Behaviors;
using AWC.Company.API.Application.Features.CreateEmployee;
using AWC.Shared.Kernel.Guards;
using AWC.Shared.Kernel.Utilities;
using FluentValidation;
using Mapster;
using MapsterMapper;


namespace AWC.Company.API.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static void ConfigureCors(this IServiceCollection services) =>
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding", "validation-errors-text"));
        });

    public static void AddMediatrServices(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(Startup).Assembly);
            config.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });
    }

    public static void AddFluentValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateEmployeeCommandValidator>();
    }

    public static void AddMappings(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetAssembly(typeof(Program)));
        config.Default.NameMatchingStrategy(NameMatchingStrategy.IgnoreCase);

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
    }

}
