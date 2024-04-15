using AWC.PersonData.API.Domain.PersonAggregate;
using AWC.PersonData.API.Infrastructure.Persistence.DataModels;
using AWC.PersonData.API.Infrastructure.Persistence.Mappings;
using AWC.Shared.Kernel.Utilities;
using PersonData.UnitTests.Data;

namespace PersonData.UnitTests.Infrastructure.Persistence.Mappings;

public class PersonDataModelToPersonDomainModelMapperTests
{
    [Fact]
    public void PersonDataModelToPersonDomainModelMapper_Map_ShouldReturn_PersonAggregate()
    {
        // Arrange
        PersonDataModel person = PersonTestData.GetPersonDataModel();
        PersonDataModelToPersonDomainModelMapper dataMapper = new();

        // Act
        Result<Person> domainModel = dataMapper.Map(person);

        // Assert
        Assert.True(domainModel.IsSuccess);
    }

    [Fact]
    public void PersonDataModelToPersonDomainModelMapper_Map_ShouldReturn_PersonAggregate_WithOne_Address()
    {
        // Arrange
        PersonDataModel person = PersonTestData.GetPersonDataModel();
        PersonDataModelToPersonDomainModelMapper dataMapper = new();

        // Act
        Result<Person> domainModel = dataMapper.Map(person);

        // Assert
        Assert.Single(domainModel.Value.Addresses);
    }

    [Fact]
    public void PersonDataModelToPersonDomainModelMapper_Map_ShouldReturn_PersonAggregate_WithOne_EmailAddress()
    {
        // Arrange
        PersonDataModel person = PersonTestData.GetPersonDataModel();
        PersonDataModelToPersonDomainModelMapper dataMapper = new();

        // Act
        Result<Person> domainModel = dataMapper.Map(person);

        // Assert
        Assert.Single(domainModel.Value.EmailAddresses);
    }

    [Fact]
    public void PersonDataModelToPersonDomainModelMapper_Map_ShouldReturn_PersonAggregate_WithOne_PersonPhone()
    {
        // Arrange
        PersonDataModel person = PersonTestData.GetPersonDataModel();
        PersonDataModelToPersonDomainModelMapper dataMapper = new();

        // Act
        Result<Person> domainModel = dataMapper.Map(person);

        // Assert
        Assert.Single(domainModel.Value.Telephones);
    }
}
