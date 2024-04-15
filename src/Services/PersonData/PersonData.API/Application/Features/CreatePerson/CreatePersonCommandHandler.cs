#pragma warning disable CS9124

using AWC.PersonData.API.Application.Abstractions.Messaging;
using AWC.PersonData.API.Domain.PersonAggregate;
using AWC.PersonData.API.Domain.PersonAggregate.EntityIDs;
using AWC.PersonData.API.Infrastructure.Persistence.Interfaces;
using AWC.PersonData.API.Infrastructure.Persistence.Repositories;
using AWC.Shared.Kernel.Utilities;
using MapsterMapper;

namespace AWC.PersonData.API.Application.Features.CreatePerson;

public sealed class CreatePersonCommandHandler(
    ILogger<PersonRepository> logger,
    IApplicationDbContext context,
    IMapper mapper
    ) : ICommandHandler<CreatePersonCommand, int>
{
    private readonly ILogger<PersonRepository> _logger = logger;
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<int>> Handle(CreatePersonCommand command, CancellationToken cancellationToken)
    {
        try
        {
            SharedPersonCommand sharedPersonCommand = _mapper.Map<SharedPersonCommand>(command);
            MapPersonCommandToPersonDomain mapPersonCommandToPersonDomain = new(sharedPersonCommand);

            Result<Person> mappedResult = mapPersonCommandToPersonDomain.Map();

            if (mappedResult.IsFailure)
            {

                return Result<int>.Failure<int>(new Error("CreatePersonCommandHandler.Handle", mappedResult.Error.Message));
            }

            PersonRepository repository = new(_logger, _context, _mapper);
            Result<int> savedResult = await repository.AddAsync(mappedResult.Value);

            if (savedResult.IsFailure)
            {
                return Result<int>.Failure<int>(new Error("CreatePersonCommandHandler.Handle", savedResult.Error.Message));
            }


            mappedResult.Value.Raise(new PersonCreatedDomainEvent(Guid.NewGuid(), new PersonID(savedResult.Value)));

            return savedResult;
        }
        catch (Exception ex)
        {
            return Result<int>.Failure<int>(new Error("CreatePersonCommandHandler.Handle",
                                                       Helpers.GetExceptionMessage(ex)));
        }
    }
}
