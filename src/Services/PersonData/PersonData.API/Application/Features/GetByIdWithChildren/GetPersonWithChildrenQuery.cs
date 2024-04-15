using AWC.PersonData.API.Application.Abstractions.Messaging;
using AWC.PersonData.API.Infrastructure.Persistence.Dtos;

namespace AWC.PersonData.API.Application.Features.GetByIdWithChildren;

public class GetPersonWithChildrenQuery : IQuery<PersonByIdWithChildrenDto>
{
    public int PersonId { get; init; }
}
