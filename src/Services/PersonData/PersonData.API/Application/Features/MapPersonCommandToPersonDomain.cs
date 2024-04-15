#pragma warning disable CS9124

using AWC.PersonData.API.Domain.PersonAggregate;
using AWC.PersonData.API.Domain.PersonAggregate.EntityIDs;
using AWC.PersonData.API.Domain.PersonAggregate.Enums;
using AWC.PersonData.API.Infrastructure.Persistence.Dtos;
using AWC.Shared.Kernel.Utilities;

namespace AWC.PersonData.API.Application.Features;

public sealed class MapPersonCommandToPersonDomain(SharedPersonCommand command)
{
    private readonly SharedPersonCommand _command = command;
    private Person? _person;

    public Result<Person> Map()
    {
        try
        {
            Result<Person> result = Person.Create
            (
                new PersonID(_command.BusinessEntityID),
                _command.PersonType,
                _command.NameStyle,
                _command.Title!,
                _command.FirstName,
                _command.MiddleName!,
                _command.LastName,
                _command.Suffix!,
                _command.EmailPromotion
            );

            if (result.IsFailure)
            {

                return Result<Person>.Failure<Person>(new Error("MapPersonCommandToPersonDomain.Map", result.Error.Message));
            }


            _person = result.Value;

            AddAddressesToPerson();
            AddPersonPhonesToPerson();
            AddEmailAddressesToPerson();

            return _person;
        }
        catch (Exception ex)
        {
            return Result<Person>.Failure<Person>(new Error("MapPersonCommandToPersonDomain.Map",
                                                  Helpers.GetExceptionMessage(ex)));
        }
    }

    private void AddEmailAddressesToPerson()
    {
        foreach (EmailAddressDto email in _command!.EmailAddresses)
        {
            Result<PersonEmailAddress> result =
                _person!.AddEmailAddress
                (
                    new PersonEmailAddressID(email.EmailAddressID),
                    email.MailAddress!
                );

            if (result.IsFailure)
            {

                throw new EmployeeMappingException(result.Error.Message);
            }

        }
    }

    private void AddPersonPhonesToPerson()
    {
        foreach (PersonPhoneDto phone in _command!.Telephones)
        {
            Result<PersonPhone> result = _person!.AddPhoneNumber
            (
                    new PersonPhoneID(_command.BusinessEntityID),
                    (PhoneNumberType)phone.PhoneNumberTypeID,
                    phone.PhoneNumber!
            );

            if (result.IsFailure)
            {

                throw new EmployeeMappingException(result.Error.Message);
            }

        }
    }

    private void AddAddressesToPerson()
    {
        foreach (AddressDto address in _command!.Addresses)
        {
            Result<Address> result = _person!.AddAddress
            (
                new AddressID(_command.BusinessEntityID),
                (AddressType)address.AddressTypeID,
                address!.AddressLine1!,
                address.AddressLine2,
                address!.City!,
                address.StateProvinceID,
                address!.PostalCode!
            );

            if (result.IsFailure)
            {

                throw new EmployeeMappingException(result.Error.Message);
            }

        }
    }

    private sealed class EmployeeMappingException(string message) : Exception(message)
    {
    }
}
