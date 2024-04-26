using MediatR;
using AWC.Shared.Kernel.Interfaces;

namespace AWC.Company.API.Application.Abstractions.Messaging;

public interface IDomainEventHandler<TEvent> : INotificationHandler<TEvent>
    where TEvent : IDomainEvent
{
}
