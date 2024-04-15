using AWC.PersonData.API.Application.Abstractions.Messaging;
using AWC.PersonData.API.Infrastructure.Persistence.Dtos;

namespace AWC.PersonData.API.Application.Features.UpdatePerson;

public record UpdatePersonCommand
(
    int BusinessEntityID,
    string PersonType,
    int NameStyle,
    string? Title,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? Suffix,
    int EmailPromotion,
    List<EmailAddressDto> EmailAddresses,
    List<PersonPhoneDto> Telephones,
    List<AddressDto> Addresses
) : ICommand;
