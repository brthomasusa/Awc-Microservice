namespace AWC.PersonData.API.Infrastructure.Persistence.Dtos;

public sealed record EmailAddressDto
{
    public int EmailAddressID { get; init; }
    public string? MailAddress { get; init; }
}
