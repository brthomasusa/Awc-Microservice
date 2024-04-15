using MediatR;
using AWC.Shared.Kernel.Utilities;

namespace AWC.PersonData.API.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
