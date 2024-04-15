using Microsoft.Extensions.Logging;
using AWC.PersonData.API.Infrastructure.Persistence.Interfaces;
using AWC.Shared.Kernel.Utilities;
using AWC.PersonData.API.Domain.PersonAggregate;
using AWC.PersonData.API.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using AWC.PersonData.API.Infrastructure.Persistence;
using MediatR;
using Moq;

namespace AWC.IntegrationTest;

public abstract class TestBase : IDisposable
{
    protected readonly AwcContext _dbContext;
    protected readonly DapperContext _dapperCtx;

    protected TestBase()
    {
        string? connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__AwcDb");
        var mock = new Mock<IPublisher>();
        var optionsBuilder = new DbContextOptionsBuilder<AwcContext>();

        optionsBuilder.UseSqlServer(
            connectionString!,
            msSqlOptions => msSqlOptions.MigrationsAssembly(typeof(AwcContext).Assembly.FullName)
        )
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors();

        _dbContext = new AwcContext(optionsBuilder.Options, mock.Object);
        _dapperCtx = new DapperContext(connectionString!);

        _dbContext.Database.ExecuteSqlRaw("EXEC dbo.usp_InitializeTestDb");
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        GC.SuppressFinalize(this);
    }
}
