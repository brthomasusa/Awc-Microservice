using AWC.PersonData.API.Application.Abstractions.Messaging;
using AWC.PersonData.API.Infrastructure.Persistence.Dtos;

namespace AWC.PersonData.API.Application.Features.GetByIdWithChildren;

public sealed record GetByIdWithChildrenQuery(int PersonId) : IQuery<PersonByIdWithChildrenDto>;

