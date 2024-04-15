using AWC.Shared.Kernel.Base;
using AWC.Shared.Kernel.Guards;

namespace AWC.PersonData.API.Domain.PersonAggregate.ValueObjects;

public sealed class AddressVO : ValueObject
{
    public const int MAX_ADDRESSLINE_LENGTH = 60;
    public const int MAX_CITY_LENGTH = 30;
    public const int MAX_POSTALCODE_LENGTH = 15;

    public string AddressLine1 { get; }
    public string? AddressLine2 { get; }
    public string City { get; }
    public int StateProvinceID { get; }
    public string PostalCode { get; }

    private AddressVO(string line1, string? line2, string city, int stateCode, string zipcode)
    {
        AddressLine1 = line1;
        AddressLine2 = line2;
        City = city;
        StateProvinceID = stateCode;
        PostalCode = zipcode;
    }

    public static AddressVO Create(string line1, string? line2, string city, int stateCode, string zipcode)
    {
        CheckValidity(line1, line2, city, stateCode, zipcode);
        return new AddressVO(line1, line2, city, stateCode, zipcode);
    }

    private static void CheckValidity(string line1, string? line2, string city, int stateCode, string zipcode)
    {
        Guard.Against.NullOrEmpty(line1);
        Guard.Against.LengthGreaterThan(line1, MAX_ADDRESSLINE_LENGTH);

        if (!string.IsNullOrEmpty(line2))
        {
            Guard.Against.LengthGreaterThan(line2, MAX_ADDRESSLINE_LENGTH);
        }


        Guard.Against.NullOrEmpty(city);
        Guard.Against.LengthGreaterThan(city, MAX_CITY_LENGTH);

        Guard.Against.LessThan(stateCode, 1);

        Guard.Against.NullOrEmpty(zipcode);
        Guard.Against.LengthGreaterThan(zipcode, MAX_POSTALCODE_LENGTH);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return AddressLine1;
        yield return AddressLine2!;
        yield return City;
        yield return StateProvinceID;
        yield return PostalCode;
    }
}
