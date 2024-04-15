using AWC.PersonData.API.Domain.PersonAggregate.EntityIDs;
using AWC.PersonData.API.Domain.PersonAggregate.Enums;
using AWC.PersonData.API.Domain.PersonAggregate.ValueObjects;
using AWC.Shared.Kernel.Base;
using AWC.Shared.Kernel.Utilities;

namespace AWC.PersonData.API.Domain.PersonAggregate;

public sealed class Person : AggregateRoot<PersonID>
{
    private readonly List<Address> _addresses = [];
    private readonly List<PersonEmailAddress> _emailAddresses = [];
    private readonly List<PersonPhone> _telephones = [];

    private Person
    (
        PersonID personID,
        PersonType personType,
        NameStyle nameStyle,
        Title title,
        PersonName name,
        Suffix suffix,
        EmailPromotion emailPromotionEnum
    )
    {
        Id = personID;
        PersonType = personType;
        NameStyle = nameStyle;
        Title = title;
        Name = name;
        Suffix = suffix;
        EmailPromotions = emailPromotionEnum;
    }

    public static Result<Person> Create
    (
        PersonID personId,
        string personType,
        int nameStyle,
        string? title,
        string firstName,
        string? middleInitial,
        string lastName,
        string? suffix,
        int emailPromotion
    )
    {
        try
        {
            Person person = new
            (
                personId,
                PersonType.Create(personType),
                Enum.IsDefined(typeof(NameStyle), nameStyle) ? (NameStyle)nameStyle : throw new ArgumentException("Invalid name style flag"),
                Title.Create(title),
                PersonName.Create(lastName, firstName, middleInitial),
                Suffix.Create(suffix),
                Enum.IsDefined(typeof(EmailPromotion), emailPromotion) ? (EmailPromotion)emailPromotion : throw new ArgumentException("Invalid email promotion flag")
            );

            return person;
        }
        catch (Exception ex)
        {
            return Result<Person>.Failure<Person>(new Error("Person.Create", Helpers.GetExceptionMessage(ex)));
        }
    }

    public Result<Person> Update
    (
        string personType,
        NameStyle nameStyle,
        string title,
        string firstName,
        string lastName,
        string middleName,
        string suffix,
        EmailPromotion emailPromotion
    )
    {
        try
        {
            PersonType = PersonType.Create(personType);
            NameStyle = Enum.IsDefined(typeof(NameStyle), nameStyle) ? nameStyle : throw new ArgumentException("Invalid names style");
            Title = Title.Create(title);
            Name = PersonName.Create(lastName, firstName, middleName);
            Suffix = Suffix.Create(suffix);
            EmailPromotions = Enum.IsDefined(typeof(EmailPromotion), emailPromotion) ? emailPromotion : throw new ArgumentException("Invalid email promotion flag");

            UpdateModifiedDate();
            return this;
        }
        catch (Exception ex)
        {
            return Result<Person>.Failure<Person>(new Error("Person.UpdatePerson", Helpers.GetExceptionMessage(ex)));
        }
    }

    public PersonType PersonType { get; private set; }

    public NameStyle NameStyle { get; private set; }

    public Title Title { get; private set; }

    public PersonName Name { get; private set; }

    public Suffix Suffix { get; private set; }

    public EmailPromotion EmailPromotions { get; private set; }


    public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

    public Result<Address> AddAddress
    (
        AddressID addressID,
        AddressType addressType,
        string line1,
        string? line2,
        string city,
        int stateProvinceID,
        string postalCode
    )
    {
        try
        {
            // No duplicate non-zero IDs
            if (addressID.Value != 0)
            {
                if (_addresses.Exists(addr => addr.Id.Value == addressID.Value))
                {

                    return Result.Failure<Address>(new Error("Person.AddAddress", "There is already an address with this Id."));
                }

            }

            // No duplicate address
            AddressVO addressVO = AddressVO.Create(line1, line2, city, stateProvinceID, postalCode);
            bool foundAddressVO = _addresses.Exists(addr => addr.Location.Equals(addressVO) && addr.AddressType == addressType);
            if (foundAddressVO)
            {

                return Result.Failure<Address>(new Error("Person.AddAddress", "There is already an address matching this one."));
            }

            Result<Address> result = Address.Create
            (
                addressID, addressType, line1, line2, city, stateProvinceID, postalCode
            );

            if (result.IsFailure)
            {
                return Result<Address>.Failure<Address>(new Error("Address.Create", result.Error.Message));
            }


            result.Value.EntityStatus = EntityStatus.Added;

            _addresses.Add(result.Value);
            return result.Value;
        }
        catch (Exception ex)
        {
            return Result<Address>.Failure<Address>(new Error("Person.AddAddress", Helpers.GetExceptionMessage(ex)));
        }
    }

    public Result<Address> UpdateAddress
    (
        AddressID addressID,
        AddressType addressType,
        string line1,
        string? line2,
        string city,
        int stateProvinceID,
        string postalCode
    )
    {
        try
        {
            Address? address = _addresses.Find(addr => addr.Id.Value == addressID.Value);

            if (address is null)
            {

                return Result<Address>.Failure<Address>(new Error("Person.AddAddress", $"Unable to locate an address with this Id {addressID.Value}."));
            }

            AddressVO addressVO = AddressVO.Create(line1, line2, city, stateProvinceID, postalCode);
            Address? duplicateAddress = _addresses.Find(addr => addr.Location.Equals(addressVO) &&
                                                                addr.AddressType.Equals(addressType) &&
                                                                addr.Id.Value != addressID.Value);

            if (duplicateAddress is not null && !duplicateAddress.Id.Value.Equals(addressID.Value))
            {
                return Result.Failure<Address>(new Error("Person.UpdateAddress", "Updating this address would result in duplicate addresses."));
            }

            Result<Address> result = address.Update
            (
                addressType, line1, line2, city, stateProvinceID, postalCode
            );

            if (result.IsFailure)
            {
                return Result<Address>.Failure<Address>(new Error("Person.UpdateAddress", result.Error.Message));
            }

            if (result.Value.EntityStatus == EntityStatus.Unmodified)
            {
                result.Value.EntityStatus = EntityStatus.Modified;
            }

            return address;
        }
        catch (Exception ex)
        {
            return Result<Address>.Failure<Address>(new Error("Person.UpdateAddress", Helpers.GetExceptionMessage(ex)));
        }
    }

    public Result DeleteAddress(AddressID id)
    {
        try
        {
            Address? search = _addresses.Find(a => a.Id.Value == id.Value);

            if (search is null)
            {
                return Result.Failure(new Error("Person.DeleteAddress", $"Delete failed, could not locate address with ID {id}."));
            }

            search.EntityStatus = EntityStatus.Deleted;
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("Person.DeleteAddress", Helpers.GetExceptionMessage(ex)));
        }
    }

    public IReadOnlyCollection<PersonEmailAddress> EmailAddresses => _emailAddresses.AsReadOnly();

    public Result<PersonEmailAddress> AddEmailAddress(PersonEmailAddressID emailAddressId, string emailAddress)
    {
        try
        {
            bool searchByID = _emailAddresses.Exists(email => email.Id.Value.Equals(emailAddressId.Value) && emailAddressId.Value != 0);
            if (searchByID)
            {
                return Result<PersonEmailAddress>.Failure<PersonEmailAddress>(new Error("Person.AddEmailAddress", "There is already an email address with this Id."));
            }

            bool searchByEmail = _emailAddresses.Exists(email => email.EmailAddress.ToString()!.ToUpper() == emailAddress.ToUpper());
            if (searchByEmail)
            {
                return Result<PersonEmailAddress>.Failure<PersonEmailAddress>(new Error("Person.AddEmailAddress", "Adding this would result in duplicate email addresses."));
            }

            Result<PersonEmailAddress> result = PersonEmailAddress.Create(emailAddressId, emailAddress);

            if (result.IsFailure)
            {
                return Result<PersonEmailAddress>.Failure<PersonEmailAddress>(new Error("Person.AddEmailAddress", result.Error.Message));
            }

            _emailAddresses.Add(result.Value);

            if (result.Value.Id.Equals(0))
            {
                result.Value.EntityStatus = EntityStatus.Added;
            }

            return result.Value;
        }
        catch (Exception ex)
        {
            return Result<PersonEmailAddress>.Failure<PersonEmailAddress>(new Error("Person.AddEmailAddress", Helpers.GetExceptionMessage(ex)));
        }
    }

    public Result<PersonEmailAddress> UpdateEmailAddress(PersonEmailAddressID emailAddressId, string emailAddress)
    {
        try
        {
            PersonEmailAddress? searchByID = _emailAddresses.Find(email => email.Id.Value.Equals(emailAddressId.Value));
            if (searchByID is null)
            {
                return Result<PersonEmailAddress>.Failure<PersonEmailAddress>(new Error("Person.UpdateEmailAddress", "Update failed; unable to locate email address with Id {}"));
            }

            bool searchByEmail = _emailAddresses.Exists(email => string.Equals(email.EmailAddress, emailAddress, StringComparison.OrdinalIgnoreCase));
            if (searchByEmail)
            {
                return Result<PersonEmailAddress>.Failure<PersonEmailAddress>(new Error("Person.AddEmailAddress", "Adding this would result in duplicate email addresses."));
            }

            Result<PersonEmailAddress> result = searchByID.UpdateEmailAddress(emailAddress);

            return searchByID;

        }
        catch (Exception ex)
        {
            return Result<PersonEmailAddress>.Failure<PersonEmailAddress>(new Error("Person.UpdateEmailAddress", Helpers.GetExceptionMessage(ex)));
        }
        throw new NotImplementedException();
    }

    public Result DeleteEmailAddress(PersonEmailAddressID id)
    {
        try
        {
            PersonEmailAddress? search = _emailAddresses.Find(a => a.Id.Value == id.Value);

            if (search is null)
            {
                return Result.Failure(new Error("Person.DeleteEmailAddress", $"Delete failed, could not locate email address with ID {id}."));
            }

            search.EntityStatus = EntityStatus.Deleted;
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("Person.DeleteEmailAddress", Helpers.GetExceptionMessage(ex)));
        }
    }

    public IReadOnlyCollection<PersonPhone> Telephones => _telephones.AsReadOnly();

    public Result<PersonPhone> AddPhoneNumber(PersonPhoneID id, PhoneNumberType phoneType, string phoneNumber)
    {
        try
        {
            bool isDupeID = _telephones.Exists(tel => tel.Id.Value.Equals(id.Value) && !id.Value.Equals(0));
            if (isDupeID)
            {
                return Result<PersonPhone>.Failure<PersonPhone>(new Error("PersonPhone.AddPhoneNumber", $"There is already a phone number with id {id.Value}."));
            }

            bool isDupePhoneNumber = _telephones.Exists(tel => tel.PhoneNumberType == phoneType && tel.Telephone == phoneNumber);
            if (isDupePhoneNumber)
            {
                return Result<PersonPhone>.Failure<PersonPhone>(new Error("PersonPhone.AddPhoneNumber", "This is a duplicate phone number."));
            }

            Result<PersonPhone> result = PersonPhone.Create(id, phoneType, phoneNumber);

            if (result.IsFailure)
            {
                return Result<PersonPhone>.Failure<PersonPhone>(new Error("PersonPhone.AddPhoneNumber", result.Error.Message));
            }

            _telephones.Add(result.Value);

            return result.Value;
        }
        catch (Exception ex)
        {
            return Result<PersonPhone>.Failure<PersonPhone>(new Error("", Helpers.GetExceptionMessage(ex)));
        }
    }

    public Result<PersonPhone> UpdatePhoneNumber(PersonPhoneID id, PhoneNumberType phoneType, string phoneNumber)
    {
        try
        {
            Result<PersonPhone> telephone = _telephones.Find(tel => tel.Id.Value.Equals(id.Value));
            if (telephone is null)
            {
                return Result<PersonPhone>.Failure<PersonPhone>(new Error("PersonPhone.UpdatePhoneNumber", $"Update failed, a phone number with id {id.Value} was not found."));
            }


            bool isDupePhoneNumber = _telephones.Exists(tel => tel.PhoneNumberType.Equals(phoneType) && tel.Telephone.Value.Equals(phoneNumber));

            if (isDupePhoneNumber)
            {

                return Result<PersonPhone>.Failure<PersonPhone>(new Error("PersonPhone.UpdatePhoneNumber", "Updating this telephone would result in duplicate telephone numbers."));
            }

            Result<PersonPhone> result = telephone.Value.Update(phoneType, phoneNumber);

            if (result.IsFailure)
            {
                return Result<PersonPhone>.Failure<PersonPhone>(new Error("PersonPhone.UpdatePhoneNumber", result.Error.Message));
            }

            _telephones.Add(result.Value);

            return result.Value;
        }
        catch (Exception ex)
        {
            return Result<PersonPhone>.Failure<PersonPhone>(new Error("", Helpers.GetExceptionMessage(ex)));
        }
    }

    public Result DeletePhoneNumber(PersonPhoneID id)
    {
        try
        {
            PersonPhone? search = _telephones.Find(ph => ph.Id.Value == id.Value);

            if (search is null)
            {

                return Result.Failure(new Error("Person.DeletePhoneNumber", $"Delete failed, could not locate phone number with ID {id}."));
            }


            search.EntityStatus = EntityStatus.Deleted;
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("Person.DeletePhoneNumber", Helpers.GetExceptionMessage(ex)));
        }
    }
}
