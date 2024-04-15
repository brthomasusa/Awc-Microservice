namespace AWC.PersonData.API.Infrastructure.Persistence.Dtos;

public sealed record PersonPhoneDto
{
    public string? PhoneNumber { get; init; }
    public int PhoneNumberTypeID { get; init; }
}
