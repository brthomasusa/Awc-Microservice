namespace AWC.PersonData.API.Infrastructure.Persistence.Dtos;

public sealed record AddressDto
{
    public int AddressID { get; init; }
    public string? AddressLine1 { get; init; }
    public string? AddressLine2 { get; init; }
    public string? City { get; init; }
    public int StateProvinceID { get; init; }
    public string? PostalCode { get; init; }
    public int AddressTypeID { get; init; }
    public override string ToString()
        => AddressLine1 +
           AddressLine2 +
           City +
           StateProvinceID.ToString() +
           PostalCode +
           AddressTypeID.ToString();
}
