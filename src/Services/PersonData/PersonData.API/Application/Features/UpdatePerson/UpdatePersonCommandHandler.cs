#pragma warning disable CS9124

using AWC.PersonData.API.Application.Abstractions.Messaging;
using AWC.PersonData.API.Domain.PersonAggregate;
using AWC.PersonData.API.Infrastructure.Persistence.Interfaces;
using AWC.PersonData.API.Infrastructure.Persistence.Repositories;
using AWC.Shared.Kernel.Utilities;
using MapsterMapper;

namespace AWC.PersonData.API.Application.Features.UpdatePerson;

public sealed class UpdatePersonCommandHandler(
    ILogger<PersonRepository> logger,
    IApplicationDbContext context,
    IMapper mapper,
    ICacheService cacheService
    ) : ICommandHandler<UpdatePersonCommand>
{
    private readonly ILogger<PersonRepository> _logger = logger;
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result> Handle(UpdatePersonCommand command, CancellationToken cancellationToken)
    {
        try
        {
            SharedPersonCommand sharedPersonCommand = _mapper.Map<SharedPersonCommand>(command);
            MapPersonCommandToPersonDomain mapPersonCommandToPersonDomain = new(sharedPersonCommand);

            Result<Person> mappedResult = mapPersonCommandToPersonDomain.Map();

            if (mappedResult.IsFailure)
            {
                return Result<int>.Failure<int>(new Error("UpdatePersonCommandHandler.Handle", mappedResult.Error.Message));
            }

            PersonRepository repository = new(_logger, _context, _mapper);
            Result savedResult = await repository.UpdateAsync(mappedResult.Value);

            if (savedResult.IsFailure)
            {
                return Result<int>.Failure<int>(new Error("UpdatePersonCommandHandler.Handle", savedResult.Error.Message));
            }

            await _cacheService.RemoveAsync($"person-{command.BusinessEntityID}", cancellationToken);

            return savedResult;
        }
        catch (Exception ex)
        {
            return Result<int>.Failure<int>(new Error("UpdatePersonCommandHandler.Handle",
                                                       Helpers.GetExceptionMessage(ex)));
        }

        throw new NotImplementedException();
    }
}

