using MediatR;
using AWC.Shared.Kernel.Utilities;

namespace AWC.Company.API.Application.Abstractions.Messaging;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}
