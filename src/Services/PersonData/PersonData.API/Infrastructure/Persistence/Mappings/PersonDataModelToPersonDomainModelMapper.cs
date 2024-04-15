using AWC.PersonData.API.Domain.PersonAggregate;
using AWC.PersonData.API.Domain.PersonAggregate.EntityIDs;
using AWC.PersonData.API.Domain.PersonAggregate.Enums;
using AWC.PersonData.API.Infrastructure.Persistence.DataModels;
using AWC.Shared.Kernel.Interfaces;
using AWC.Shared.Kernel.Utilities;

namespace AWC.PersonData.API.Infrastructure.Persistence.Mappings;

public sealed class PersonDataModelToPersonDomainModelMapper : ModelMapper<PersonDataModel, Person>
{
    private Person? _personDomainModel;
    private PersonDataModel? _personDateModel;

    public override Result<Person> Map(PersonDataModel dataModel)
    {
        try
        {
            Result<Person> result = Person.Create
            (
                new PersonID(dataModel.BusinessEntityID),
                dataModel.PersonType!,
                dataModel.NameStyle ? 1 : 0,
                dataModel.Title,
                dataModel.FirstName!,
                dataModel.MiddleName!,
                dataModel.LastName!,
                dataModel.Suffix,
                dataModel.EmailPromotion
            );

            _personDomainModel = result.Value;
            _personDateModel = dataModel;

            MapAddresses();
            MapEmailAddresses();
            MapPersonPhones();

            return result.Value;
        }
        catch (Exception ex)
        {
            return Result<Person>.Failure<Person>(new Error("PersonDataModelToPersonDomainModelMapper.Map", Helpers.GetExceptionMessage(ex)));
        }
    }

    private void MapAddresses()
    {
        foreach (BusinessEntityAddress bea in _personDateModel!.BusinessEntityAddresses)
        {
            Result<AWC.PersonData.API.Domain.PersonAggregate.Address> result =
                _personDomainModel!.AddAddress
                (
                    new AddressID(bea.AddressID),
                    (AWC.PersonData.API.Domain.PersonAggregate.Enums.AddressType)bea.AddressTypeID,
                    bea.Address!.AddressLine1!,
                    bea.Address.AddressLine2,
                    bea.Address!.City!,
                    bea.Address.StateProvinceID,
                    bea.Address!.PostalCode!
                );

            if (result.IsFailure)
            {

                throw new EmployeeMappingException(result.Error.Message);
            }

        }
    }

    private void MapEmailAddresses()
    {
        foreach (EmailAddress email in _personDateModel!.EmailAddresses)
        {
            Result<PersonEmailAddress> result =
                _personDomainModel!.AddEmailAddress
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

    private void MapPersonPhones()
    {
        foreach (AWC.PersonData.API.Infrastructure.Persistence.DataModels.PersonPhone phone in _personDateModel!.Telephones)
        {
            Result<AWC.PersonData.API.Domain.PersonAggregate.PersonPhone> result = _personDomainModel!.AddPhoneNumber
            (
                    new PersonPhoneID(phone.BusinessEntityID),
                    (AWC.PersonData.API.Domain.PersonAggregate.Enums.PhoneNumberType)phone.PhoneNumberTypeID,
                    phone.PhoneNumber!
            );

            if (result.IsFailure)
            {

                throw new EmployeeMappingException(result.Error.Message);
            }

        }
    }

    public sealed class EmployeeMappingException(string message) : Exception(message)
    {
    }
}
