#pragma warning disable CS9124

using AWC.PersonData.API.Application.Abstractions.Messaging;
using AWC.PersonData.API.Infrastructure.Persistence.Interfaces;
using AWC.PersonData.API.Infrastructure.Persistence.Repositories;
using AWC.Shared.Kernel.Utilities;
using MapsterMapper;

namespace AWC.PersonData.API.Application.Features.DeletePerson;

public sealed class DeletePersonCommandHandler(
    ILogger<PersonRepository> logger,
    IApplicationDbContext context,
    IMapper mapper,
    ICacheService cacheService
    ) : ICommandHandler<DeletePersonCommand, int>
{
    private readonly ILogger<PersonRepository> _logger = logger;
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result<int>> Handle(DeletePersonCommand command, CancellationToken cancellationToken)
    {
        try
        {
            PersonRepository repository = new(_logger, _context, _mapper);
            Result deleteResult = await repository.DeleteAsync(command.BusinessEntityID);

            if (deleteResult.IsFailure)
            {
                Result<int>.Failure<int>(new Error("DeletePersonCommandHandler.Handle", deleteResult.Error.Message));
            }

            await _cacheService.RemoveAsync($"person-{command.BusinessEntityID}", cancellationToken);

            return Result<int>.Success<int>(0);
            ;
        }
        catch (Exception ex)
        {
            return Result<int>.Failure<int>(new Error("DeletePersonCommandHandler.Handle",
                                                       Helpers.GetExceptionMessage(ex)));
        }
    }
}
