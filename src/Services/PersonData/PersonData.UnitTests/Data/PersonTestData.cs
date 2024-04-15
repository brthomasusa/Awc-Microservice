using AWC.PersonData.API.Application.Features.CreatePerson;
using AWC.PersonData.API.Domain.PersonAggregate;
using AWC.PersonData.API.Domain.PersonAggregate.EntityIDs;
using AWC.PersonData.API.Domain.PersonAggregate.Enums;
using AWC.PersonData.API.Domain.PersonAggregate.ValueObjects;
using AWC.PersonData.API.Infrastructure.Persistence.DataModels;
using AWC.Shared.Kernel.Utilities;
using AWC.PersonData.API.Infrastructure.Persistence.Dtos;

namespace PersonData.UnitTests.Data;

public static class PersonTestData
{

    public static Result<Person> GetValidPersonAggregate()
    {
        Result<Person> person = Person.Create
        (
            new PersonID(1),
            "EM",
            0,
            "Mr.",
            "John",
            "J",
            "Doe",
            null,
            0
        );

        person.Value.AddEmailAddress(new PersonEmailAddressID(1), "david0@adventure-works.com");
        person.Value.AddPhoneNumber(new PersonPhoneID(1), AWC.PersonData.API.Domain.PersonAggregate.Enums.PhoneNumberType.Cell, "913-555-0172");
        person.Value.AddAddress
        (
            new AddressID(1),
            AWC.PersonData.API.Domain.PersonAggregate.Enums.AddressType.Home,
            "3768 Door Way",
            "Door 2",
            "Redmond",
            79,
            "98052"
        );

        return person;
    }

    public static PersonDataModel GetPersonDataModel()
    {
        PersonDataModel person = new()
        {
            BusinessEntityID = 16,
            PersonType = "EM",
            NameStyle = false,
            Title = null,
            FirstName = "David",
            MiddleName = "M",
            LastName = "Bradley",
            Suffix = null,
            EmailPromotion = 1,
            Password = new() { BusinessEntityID = 16, PasswordHash = "oaeJoTn5hbyNfemp2qzIpGTP5uNle8NRPki9Ur3Znl8=", PasswordSalt = "CTdtN+Q=" },
            EmailAddresses = new() { new EmailAddress() { BusinessEntityID = 16, EmailAddressID = 16, MailAddress = "david0@adventure-works.com" } },
            Telephones =
                [
                    new AWC.PersonData.API.Infrastructure.Persistence.DataModels.PersonPhone()
                    {
                        BusinessEntityID = 16,
                        PhoneNumber = "913-555-0172",
                        PhoneNumberTypeID = 3
                    }
                ],
            BusinessEntityAddresses =
                [
                    new BusinessEntityAddress()
                    {
                        BusinessEntityID = 16,
                        AddressID = 214,
                        Address = new AWC.PersonData.API.Infrastructure.Persistence.DataModels.Address()
                            {
                                AddressID = 214,
                                AddressLine1 = "3768 Door Way",
                                AddressLine2 = null,
                                City = "Redmond",
                                StateProvinceID = 79,
                                PostalCode = "98052"
                            },
                        AddressTypeID = 2
                    }
                ]
        };

        return person;
    }

    public static CreatePersonCommand GetCreatePesonCommand()
    {
        return new(
                BusinessEntityID: 0,
                PersonType: "EM",
                NameStyle: 0,
                Title: "Mr.",
                FirstName: "Johnny",
                LastName: "Doe",
                MiddleName: "J",
                Suffix: null!,
                EmailPromotion: 0,


                EmailAddresses:
                [
                    new EmailAddressDto(){ EmailAddressID = 0, MailAddress = "johnny0@adventure-works.com" }
                ],
                Telephones:
                [
                    new PersonPhoneDto() {PhoneNumber = "555-555-5555", PhoneNumberTypeID = 3}
                ],
                Addresses:
                [
                    new AddressDto()
                    {
                        AddressID = 0,
                        AddressLine1 = "3768 Door One",
                        AddressLine2 = null,
                        City = "Redmond",
                        StateProvinceID = 79,
                        PostalCode = "98052",
                        AddressTypeID = 2
                    }
                ]
            );
    }

    public static CreatePersonCommand GetCreatePesonCommandWithDupeEmail()
    {
        return new(
                BusinessEntityID: 0,
                PersonType: "EM",
                NameStyle: 0,
                Title: "Mr.",
                FirstName: "Johnny",
                LastName: "Doe",
                MiddleName: "J",
                Suffix: null!,
                EmailPromotion: 0,


                EmailAddresses:
                [
                    new EmailAddressDto(){ EmailAddressID = 0, MailAddress = "rob0@adventure-works.com" }
                ],
                Telephones:
                [
                    new PersonPhoneDto() {PhoneNumber = "555-555-5555", PhoneNumberTypeID = 3}
                ],
                Addresses:
                [
                    new AddressDto()
                    {
                        AddressID = 0,
                        AddressLine1 = "3768 Door One",
                        AddressLine2 = null,
                        City = "Redmond",
                        StateProvinceID = 79,
                        PostalCode = "98052"
                    }
                ]
            );
    }
}
