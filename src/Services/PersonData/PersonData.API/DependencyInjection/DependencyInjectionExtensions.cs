using Microsoft.EntityFrameworkCore;
using AWC.PersonData.API.Application.Features.CreatePerson;
using AWC.PersonData.API.Application.Behaviors;
using AWC.PersonData.API.Domain.Interfaces;
using AWC.PersonData.API.Infrastructure.Persistence;
using AWC.PersonData.API.Infrastructure.Persistence.Caching;
using AWC.PersonData.API.Infrastructure.Persistence.Interfaces;
using AWC.PersonData.API.Infrastructure.Persistence.Repositories;
using AWC.Shared.Kernel.Guards;
using AWC.Shared.Kernel.Utilities;
using FluentValidation;
using Mapster;
using MapsterMapper;


namespace AWC.PersonData.API.DependencyInjection;

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
        services.AddValidatorsFromAssemblyContaining<CreatePersonCommandValidator>();
    }

    public static void AddMappings(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(ServerAssembly.Instance);
        config.Default.NameMatchingStrategy(NameMatchingStrategy.IgnoreCase);

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
    }

    public static void AddPersistence(this IServiceCollection services)
    {
        string? connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__AwcDb");
        Guard.Against.NullOrEmpty(connectionString!);

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddDbContext<AwcContext>(options =>
            options.UseSqlServer(
                connectionString,
                msSqlOptions => msSqlOptions.MigrationsAssembly(typeof(AwcContext).Assembly.FullName)
            )
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
        );

        _ = services.AddSingleton<DapperContext>(_ => new DapperContext(connectionString!));

        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<AwcContext>());

        services.AddScoped<IUnitOfWork>(sp =>
            sp.GetRequiredService<AwcContext>());

        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, CacheService>();
    }
}
