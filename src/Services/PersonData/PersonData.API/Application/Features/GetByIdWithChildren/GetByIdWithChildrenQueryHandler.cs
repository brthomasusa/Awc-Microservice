using Microsoft.EntityFrameworkCore;
using AWC.PersonData.API.Application.Abstractions.Messaging;
using AWC.PersonData.API.Infrastructure.Persistence.Dtos;
using AWC.PersonData.API.Infrastructure.Persistence.Interfaces;
using AWC.Shared.Kernel.Utilities;

namespace AWC.PersonData.API.Application.Features.GetByIdWithChildren;

public sealed class GetByIdWithChildrenQueryHandler
(
    IApplicationDbContext dbContext,
    ILogger<GetByIdWithChildrenQueryHandler> logger,
    ICacheService cacheService
) : IQueryHandler<GetByIdWithChildrenQuery, PersonByIdWithChildrenDto>
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly ILogger<GetByIdWithChildrenQueryHandler> _logger = logger;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result<PersonByIdWithChildrenDto>> Handle
    (
        GetByIdWithChildrenQuery query,
        CancellationToken cancellationToken
    ) =>
            await _cacheService.GetAsync<PersonByIdWithChildrenDto>($"person-{query.PersonId}", async () =>
                {
                    PersonByIdWithChildrenDto? person = await _dbContext.Person!
                        .AsNoTracking()
                        .Where(person => person.BusinessEntityID == query.PersonId)
                        .Include(person => person.EmailAddresses!)
                        .Include(person => person.Telephones!)
                        .Include(person => person.Password)
                        .Include(person => person.BusinessEntityAddresses!)
                            .ThenInclude(addr => addr.Address!)
                        .Select(p => new PersonByIdWithChildrenDto
                        {
                            BusinessEntityID = p.BusinessEntityID,
                            PersonType = p.PersonType!,
                            NameStyle = p.NameStyle,
                            Title = p.Title!,
                            FirstName = p.FirstName,
                            MiddleName = p.MiddleName,
                            LastName = p.LastName,
                            Suffix = p.Suffix!,
                            EmailPromotion = p.EmailPromotion,
                            EmailAddresses = p.EmailAddresses.Select(email => new EmailAddressDto
                            {
                                EmailAddressID = email.EmailAddressID,
                                MailAddress = email.MailAddress
                            }
                            ).ToList(),
                            Telephones = p.Telephones.Select(tel => new PersonPhoneDto
                            {
                                PhoneNumber = tel.PhoneNumber,
                                PhoneNumberTypeID = tel.PhoneNumberTypeID
                            }
                            ).ToList(),
                            Addresses = p.BusinessEntityAddresses.Select(bea => new AddressDto
                            {
                                AddressID = bea.Address!.AddressID,
                                AddressLine1 = bea.Address!.AddressLine1,
                                AddressLine2 = bea.Address!.AddressLine2,
                                City = bea.Address!.City,
                                StateProvinceID = bea.Address!.StateProvinceID,
                                PostalCode = bea.Address!.PostalCode,
                                AddressTypeID = bea.AddressTypeID
                            }
                            ).ToList()
                        })
                        .SingleOrDefaultAsync(cancellationToken: cancellationToken);

                    return person!;
                }, cancellationToken);
}
