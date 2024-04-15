using AWC.PersonData.API.Application.Abstractions.Messaging;

namespace AWC.PersonData.API.Application.Features.DeletePerson;

public record DeletePersonCommand
(
    int BusinessEntityID
) : ICommand<int>;
