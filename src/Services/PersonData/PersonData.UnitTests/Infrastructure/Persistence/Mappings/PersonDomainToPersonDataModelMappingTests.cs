using AWC.PersonData.API.Domain.PersonAggregate;
using AWC.PersonData.API.Infrastructure.Persistence.DataModels;
using AWC.Shared.Kernel.Utilities;
using MapsterMapper;
using PersonData.UnitTests.Data;

namespace PersonData.UnitTests.Infrastructure.Persistence.Mappings;

public class PersonDomainToPersonDataModelMappingTests
{
    private readonly Person _personDomainModel;
    private readonly IMapper _mapper;

    public PersonDomainToPersonDataModelMappingTests()
    {
        Result<Person> personDomainModel = PersonTestData.GetValidPersonAggregate();
        _personDomainModel = personDomainModel.Value;
        _mapper = AddMapsterForUnitTests.GetMapper();
    }

    [Fact]
    public void PersonDomainModelToDataModel_Map_From_PersonAggregate_ShouldReturn_PersonDataModel()
    {
        // Arrange

        // Act
        PersonDataModel personDataModel = _mapper.Map<PersonDataModel>(_personDomainModel);

        // Assert
        Assert.NotNull(personDataModel);
    }

    [Fact]
    public void PersonDomainModelToDataModel_Map_From_DomainEmailAddress_ShouldReturn_DataModelEmailAddress()
    {
        // Arrange
        PersonDataModel personDataModel = _mapper.Map<PersonDataModel>(_personDomainModel);

        // Act
        _personDomainModel.EmailAddresses.ToList().ForEach(email =>
            personDataModel.EmailAddresses.Add(_mapper.Map<AWC.PersonData.API.Infrastructure.Persistence.DataModels.EmailAddress>(email))
        );

        // Assert
        Assert.NotEmpty(personDataModel.EmailAddresses);
    }

    [Fact]
    public void PersonDomainModelToDataModel_Map_From_DomainPersonPhone_ShouldReturn_DataModelPersonPhone()
    {
        // Arrange
        PersonDataModel personDataModel = _mapper.Map<PersonDataModel>(_personDomainModel);

        // Act
        _personDomainModel.Telephones.ToList().ForEach(tel =>
            personDataModel.Telephones.Add(_mapper.Map<AWC.PersonData.API.Infrastructure.Persistence.DataModels.PersonPhone>(tel))
        );

        // Assert
        Assert.NotEmpty(personDataModel.Telephones);
    }

    [Fact]
    public void PersonDomainModelToDataModel_Map_From_DomainBusEntityAddr_ShouldReturn_DataModelBusEntityAddr()
    {
        // Arrange
        PersonDataModel personDataModel = _mapper.Map<PersonDataModel>(_personDomainModel);

        // Act
        _personDomainModel.Addresses.ToList().ForEach(addr =>
            {
                // Create BusinessEntityAddress
                BusinessEntityAddress bea = new()
                {
                    BusinessEntityID = _personDomainModel.Id.Value,
                    AddressID = addr.Id.Value,
                    Address = _mapper.Map<AWC.PersonData.API.Infrastructure.Persistence.DataModels.Address>(addr),
                    AddressTypeID = (int)addr.AddressType
                };

                // Add datamodel address to datamodel business entity address
                personDataModel.BusinessEntityAddresses.Add(bea);
            }
        );

        Assert.NotEmpty(personDataModel.BusinessEntityAddresses);
        int dataModelAddressId = personDataModel.BusinessEntityAddresses.FirstOrDefault()!.AddressID;
        int domainModelAddressId = _personDomainModel.Addresses.ToList().FirstOrDefault()!.Id.Value;

        // Assert            
        Assert.Equal(domainModelAddressId, dataModelAddressId);
    }
}
