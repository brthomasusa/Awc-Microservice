using AWC.PersonData.API.Domain.PersonAggregate.Enums;
using AWC.PersonData.API.Domain.PersonAggregate.EntityIDs;
using AWC.PersonData.API.Domain.PersonAggregate.ValueObjects;
using AWC.Shared.Kernel.Base;
using AWC.Shared.Kernel.Utilities;

namespace AWC.PersonData.API.Domain.PersonAggregate;

public sealed class PersonPhone : Entity<PersonPhoneID>
{
    private PersonPhone
    (
        PersonPhoneID id,
        PhoneNumberType phoneType,
        PhoneNumber phoneNumber
    )
    {
        Id = id;
        PhoneNumberType = phoneType;
        Telephone = phoneNumber;
    }

    internal static Result<PersonPhone> Create
    (
        PersonPhoneID id,
        PhoneNumberType phoneNumberType,
        string telephone
    )
    {
        try
        {
            PersonPhone phone = new
            (
                id,
                Enum.IsDefined(typeof(PhoneNumberType), phoneNumberType) ? phoneNumberType : throw new ArgumentException("Invalid phone number type."),
                PhoneNumber.Create(telephone)
            );

            return phone;
        }
        catch (Exception ex)
        {
            return Result<PersonPhone>.Failure<PersonPhone>(new Error("PersonPhone.Create", Helpers.GetExceptionMessage(ex)));
        }
    }

    internal Result<PersonPhone> Update(PhoneNumberType phoneNumberType, string telephone)
    {
        try
        {
            PhoneNumberType = phoneNumberType;
            Telephone = PhoneNumber.Create(telephone);

            UpdateModifiedDate();

            return this;
        }
        catch (Exception ex)
        {
            return Result<PersonPhone>.Failure<PersonPhone>(new Error("PersonPhone.Update", Helpers.GetExceptionMessage(ex)));
        }
    }

    public PhoneNumberType PhoneNumberType { get; private set; }

    public PhoneNumber Telephone { get; private set; }
}
