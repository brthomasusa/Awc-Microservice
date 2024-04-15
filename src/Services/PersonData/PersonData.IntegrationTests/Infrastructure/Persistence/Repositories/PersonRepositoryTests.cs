using Microsoft.Extensions.Logging.Abstractions;
using AWC.IntegrationTest;
using AWC.PersonData.API.Domain.Interfaces;
using AWC.PersonData.API.Domain.PersonAggregate;
using AWC.PersonData.API.Domain.PersonAggregate.Enums;
using AWC.PersonData.API.Infrastructure.Persistence.Repositories;
using AWC.Shared.Kernel.Utilities;
using MapsterMapper;
using PersonData.IntegrationTests.Data;

namespace PersonData.IntegrationTests.Infrastructure.Persistence.Repositories;

[Collection("Database Test")]
public class PersonRepositoryTests : TestBase
{
    private readonly IPersonRepository _repository;
    private readonly IMapper _mapper = PersonData.IntegrationTest.AddMapsterForUnitTests.GetMapper();

    public PersonRepositoryTests()
    {
        _repository = new PersonRepository
                (
                    new NullLogger<PersonRepository>(),
                    _dbContext,
                    _mapper
                );
    }


    [Fact]
    public async Task GetById_PesonRepository_ShouldSucceed()
    {
        Result<Person> result = await _repository.GetByIdWithChildrenAsync(4);

        Assert.True(result.IsSuccess);

        Assert.True(result.Value.Addresses.Any());
        Assert.True(result.Value.EmailAddresses.Any());
        Assert.True(result.Value.Telephones.Any());
    }

    [Fact]
    public async Task Add_PersonRepository_ShouldSucceed()
    {
        // Arrange
        Result<Person> person = PersonTestData.GetValidNewPersonAggregate();

        // Act
        Result<int> result = await _repository.AddAsync(person.Value);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Update_PersonRepository_ShouldSucceed()
    {
        // Arrange
        Result<Person> person = await _repository.GetByIdWithChildrenAsync(4);

        Result<Person> updateResult =
            person.Value.Update("EM", NameStyle.Western, "Mr.", "Jabu", "Jabi", "J", "Sr.", EmailPromotion.None);

        // Act
        Result result = await _repository.UpdateAsync(updateResult.Value);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact(Skip = "This delete operation will span two microservices")]
    public async Task Delete_PersonRepository_ShouldSucceed()
    {
        // Assert
        int businessEntityId = 4;

        // Act
        Result result = await _repository.DeleteAsync(businessEntityId);

        // Assert
        Result<Person> testResult = await _repository.GetByIdAsync(businessEntityId);
        Assert.True(testResult.IsFailure);
    }
}
