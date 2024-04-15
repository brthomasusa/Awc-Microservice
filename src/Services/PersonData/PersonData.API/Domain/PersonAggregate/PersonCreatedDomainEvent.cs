using AWC.PersonData.API.Domain.PersonAggregate.EntityIDs;
using AWC.Shared.Kernel.Base;

namespace AWC.PersonData.API.Domain.PersonAggregate;

public record PersonCreatedDomainEvent(Guid Id, PersonID PersonID) : DomainEvent(Id);
