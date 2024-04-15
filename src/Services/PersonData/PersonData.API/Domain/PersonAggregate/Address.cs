#pragma warning disable CS8618

using AWC.PersonData.API.Domain.PersonAggregate.Enums;
using AWC.PersonData.API.Domain.PersonAggregate.EntityIDs;
using AWC.PersonData.API.Domain.PersonAggregate.ValueObjects;
using AWC.Shared.Kernel.Base;
using AWC.Shared.Kernel.Utilities;

namespace AWC.PersonData.API.Domain.PersonAggregate;

public sealed class Address : Entity<AddressID>
{
    private Address
    (
        AddressID addressID,
        AddressType addressType,
        AddressVO address
    )
    {
        Id = addressID;
        AddressType = addressType;
        Location = address;
    }

    internal static Result<Address> Create
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
            Address address = new
            (
                addressID,
                Enum.IsDefined(typeof(AddressType), addressType) ? addressType : throw new ArgumentException("Invalid address type."),
                AddressVO.Create(line1, line2, city, stateProvinceID, postalCode)
            );

            return address;
        }
        catch (Exception ex)
        {
            return Result<Address>.Failure<Address>(new Error("Address.Create", Helpers.GetExceptionMessage(ex)));
        }
    }

    internal Result<Address> Update
    (
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
            AddressType = Enum.IsDefined(typeof(AddressType), addressType) ? addressType : throw new ArgumentException("Invalid address type.");
            Location = AddressVO.Create(line1, line2, city, stateProvinceID, postalCode);

            UpdateModifiedDate();

            return this;
        }
        catch (Exception ex)
        {
            return Result<Address>.Failure<Address>(new Error("Address.Update", Helpers.GetExceptionMessage(ex)));
        }
    }

    public AddressType AddressType { get; private set; }
    public AddressVO Location { get; private set; }
}
