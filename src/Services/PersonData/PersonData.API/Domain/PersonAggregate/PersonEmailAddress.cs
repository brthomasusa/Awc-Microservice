#pragma warning disable CS8618

using AWC.PersonData.API.Domain.PersonAggregate.EntityIDs;
using AWC.PersonData.API.Domain.PersonAggregate.ValueObjects;
using AWC.Shared.Kernel.Base;
using AWC.Shared.Kernel.Utilities;

namespace AWC.PersonData.API.Domain.PersonAggregate;

public sealed class PersonEmailAddress : Entity<PersonEmailAddressID>
{
    private PersonEmailAddress
    (
        PersonEmailAddressID emailAddressID,
        Email emailAddress
    )
    {
        Id = emailAddressID;
        EmailAddress = emailAddress;
    }

    internal static Result<PersonEmailAddress> Create(PersonEmailAddressID id, string email)
    {
        try
        {
            PersonEmailAddress emailAddress = new
            (
                id,
                Email.Create(email)
            );

            return emailAddress;
        }
        catch (Exception ex)
        {
            return Result<PersonEmailAddress>.Failure<PersonEmailAddress>(new Error("PersonEmailAddress.Create", Helpers.GetExceptionMessage(ex)));
        }
    }

    internal Result<PersonEmailAddress> UpdateEmailAddress(string email)
    {
        try
        {
            EmailAddress = Email.Create(email);

            UpdateModifiedDate();

            return this;
        }
        catch (Exception ex)
        {
            return Result<PersonEmailAddress>.Failure<PersonEmailAddress>(new Error("PersonEmailAddress.Update", Helpers.GetExceptionMessage(ex)));
        }
    }

    public PersonEmailAddressID EmailAddressID { get; }

    public Email EmailAddress { get; private set; }

}
