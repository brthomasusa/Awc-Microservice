using Microsoft.Extensions.Logging;
using AWC.IntegrationTest;
using AWC.PersonData.API.Application.Features.CreatePerson;
using AWC.PersonData.API.Infrastructure.Persistence.Dtos;
using AWC.PersonData.API.Infrastructure.Persistence.Repositories;
using AWC.Shared.Kernel.Utilities;
using PersonData.IntegrationTest;
using PersonData.IntegrationTests.Data;

namespace PersonData.IntegrationTests.Application.Features.CreatePerson;


public class CreatePersonCommandHandlerTests : TestBase
{
    private readonly CreatePersonCommandHandler _commandHandler;

    public CreatePersonCommandHandlerTests()
    {
        using var loggerFactory = LoggerFactory.Create(c => c.AddConsole());
        var logger = loggerFactory.CreateLogger<PersonRepository>();
        _commandHandler = new(logger, _dbContext, AddMapsterForUnitTests.GetMapper());
    }

    [Fact]
    public async Task Handle_CreatePersonCommandHandler_CreateWithValidData_ShouldSucceed()
    {
        // Arrange
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();

        // Act
        Result<int> result = await _commandHandler.Handle(command, new CancellationToken());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value > 0);
    }
}
