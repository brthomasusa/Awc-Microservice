using MediatR;
using AWC.Shared.Kernel.Interfaces;

namespace AWC.PersonData.API.Application.Abstractions.Messaging;

public interface IDomainEventHandler<TEvent> : INotificationHandler<TEvent>
    where TEvent : IDomainEvent
{
}
