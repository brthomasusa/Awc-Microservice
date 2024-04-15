namespace AWC.PersonData.API.Infrastructure.Persistence.Dtos;

public sealed class PersonByIdWithChildrenDto
{
    public int BusinessEntityID { get; init; }
    public string? PersonType { get; init; }
    public bool NameStyle { get; init; }
    public string? Title { get; init; }
    public string? FirstName { get; init; }
    public string? MiddleName { get; init; }
    public string? LastName { get; init; }
    public string? Suffix { get; init; }
    public int EmailPromotion { get; init; }
    public List<EmailAddressDto> EmailAddresses { get; init; } = [];
    public List<PersonPhoneDto> Telephones { get; init; } = [];
    public List<AddressDto> Addresses { get; init; } = [];
}
