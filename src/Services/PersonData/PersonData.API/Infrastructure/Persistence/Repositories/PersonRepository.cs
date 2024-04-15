#pragma warning disable CS9124

using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AWC.PersonData.API.Infrastructure.Persistence.DataModels;
using AWC.PersonData.API.Infrastructure.Persistence.Interfaces;
using AWC.Shared.Kernel.Utilities;
using AWC.PersonData.API.Domain.PersonAggregate;
using AWC.PersonData.API.Domain.Interfaces;
using MapsterMapper;
using AWC.PersonData.API.Infrastructure.Persistence.Mappings;

using DomainAddress = AWC.PersonData.API.Domain.PersonAggregate.Address;
using DataModelAddress = AWC.PersonData.API.Infrastructure.Persistence.DataModels.Address;

namespace AWC.PersonData.API.Infrastructure.Persistence.Repositories;

public sealed class PersonRepository(
    ILogger<PersonRepository> logger,
    IApplicationDbContext context,
    IMapper mapper
    ) : IPersonRepository
{
    private readonly ILogger<PersonRepository> _logger = logger;
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public Task<Result<Person>> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
    public async Task<Result<Person>> GetByIdWithChildrenAsync(int id)
    {
        try
        {
            PersonDataModel? person = await _context.Person!
                .Where(person => person.BusinessEntityID == id)
                .Include(person => person.EmailAddresses!)
                .Include(person => person.Telephones!)
                .Include(person => person.Password)
                .Include(person => person.BusinessEntityAddresses!)
                    .ThenInclude(addr => addr.Address!)
                .SingleOrDefaultAsync();

            PersonDataModelToPersonDomainModelMapper mapToDomain = new();
            Result<Person> domainModel = mapToDomain.Map(person!);

            return domainModel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"PersonRepository.GetByIdWithChildrenAsync - {Helpers.GetExceptionMessage(ex)}");
            return Result<Person>.Failure<Person>(new Error("PersonRepository.GetByIdWithChildrenAsync",
                                                            Helpers.GetExceptionMessage(ex)));
        }
    }

    public async Task<Result<int>> AddAsync(Person person)
    {
        try
        {
            // Step 1 Start a transaction
            using var transaction = _context.Database.BeginTransaction();

            // Step 2: Insert address into database, address must exist before BusinessEntityAddress
            DomainAddress? domainAddress = person.Addresses.ToList().FirstOrDefault()!;
            DataModelAddress dataModelAddress = _mapper.Map<DataModels.Address>(domainAddress!);
            await _context.Address!.AddAsync(dataModelAddress);
            await _context.SaveChangesAsync();  // Do this in order to get an AddressID assigned to dataModelAddress

            // Step 3: Map data from person domain model to person data model
            PersonDataModel personDataModel = _mapper.Map<PersonDataModel>(person);

            person.EmailAddresses.ToList().ForEach(email =>
                personDataModel.EmailAddresses.Add(_mapper.Map<DataModels.EmailAddress>(email))
            );

            person.Telephones.ToList().ForEach(tel =>
                personDataModel.Telephones.Add(_mapper.Map<DataModels.PersonPhone>(tel))
            );

            // Step 4: Create a BusinessEntity instance and connect it to person data model
            BusinessEntity businessEntity = new() { PersonModel = personDataModel };

            // Step 5: Save changes which will give us db assigned BusinessEntityID
            await _context.BusinessEntity!.AddAsync(businessEntity);
            await _context.SaveChangesAsync();

            // Step 6: Insert BusinessEntityAddress that links Person to an address
            await _context.BusinessEntityAddress!.AddAsync
            (
                new BusinessEntityAddress()
                {
                    BusinessEntityID = personDataModel.BusinessEntityID,
                    AddressID = dataModelAddress.AddressID,
                    AddressTypeID = (int)domainAddress.AddressType
                }
            );
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return businessEntity.BusinessEntityID;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"PersonRepository.AddAsync - {Helpers.GetExceptionMessage(ex)}");
            return Result<int>.Failure<int>(new Error("PersonRepository.AddAsync",
                                            Helpers.GetExceptionMessage(ex)));
        }
    }

    public async Task<Result> UpdateAsync(Person person)
    {
        try
        {
            PersonDataModel personDataModel = _mapper.Map<PersonDataModel>(person);

            person.EmailAddresses.ToList().ForEach(email =>
                personDataModel.EmailAddresses.Add(_mapper.Map<DataModels.EmailAddress>(email))
            );

            person.Telephones.ToList().ForEach(tel =>
                personDataModel.Telephones.Add(_mapper.Map<DataModels.PersonPhone>(tel))
            );

            person.Addresses.ToList().ForEach(addr =>
                personDataModel.BusinessEntityAddresses.Add
                (
                    new BusinessEntityAddress()
                    {
                        BusinessEntityID = personDataModel.BusinessEntityID,
                        AddressID = addr.Id.Value,
                        Address = _mapper.Map<DataModels.Address>(addr),
                        AddressTypeID = (int)addr.AddressType
                    }
                )
            );

            await UpdatePerson(personDataModel);
            await UpdateAddress(personDataModel);
            await UpdateEmailAddress(personDataModel);
            await UpdatePhoneNumber(personDataModel);

            await _context.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"PersonRepository.UpdateAsync - {Helpers.GetExceptionMessage(ex)}");
            return Result.Failure(new Error("PersonRepository.UpdateAsync",
                                  Helpers.GetExceptionMessage(ex)));
        }
    }

    public async Task<Result> DeleteAsync(int id)
    {
        try
        {
            BusinessEntityAddress? bea = await _context.BusinessEntityAddress!.Where(b => b.BusinessEntityID == id).FirstOrDefaultAsync();
            int addressId = bea!.AddressID;

            using var transaction = _context.Database.BeginTransaction();

            // Delete Person.EmailAddress
            await _context.EmailAddress!.Where(email => email.BusinessEntityID == id).ExecuteDeleteAsync();

            // Delete Person.Phone
            await _context.PersonPhone!.Where(ph => ph.BusinessEntityID == id).ExecuteDeleteAsync();

            // Delete Person.PassWord
            await _context.Password!.Where(ph => ph.BusinessEntityID == id).ExecuteDeleteAsync();

            // Delete Person.BusinessEntityAddress
            await _context.BusinessEntityAddress!.Where(bea => bea.BusinessEntityID == id).ExecuteDeleteAsync();

            // Delete Person.Address                
            await _context.Address!.Where(a => a.AddressID == addressId).ExecuteDeleteAsync();

            // Delete Person.Person
            await _context.Person!.Where(p => p.BusinessEntityID == id).ExecuteDeleteAsync();

            // Finally, delete Person.BusinessEntity
            await _context.BusinessEntity!.Where(bea => bea.BusinessEntityID == id).ExecuteDeleteAsync();

            await transaction.CommitAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"PersonRepository.DeleteAsync - {Helpers.GetExceptionMessage(ex)}");
            return Result.Failure(new Error("PersonRepository.DeleteAsync",
                                  Helpers.GetExceptionMessage(ex)));
        }
    }

    public async Task<Result<bool>> IsNameUniqueForCreate(string firstName, string? middleInitial, string lastName)
    {
        return !await _context.Person!.AsNoTracking().AnyAsync(p => p.FirstName == firstName &&
                                                     p.MiddleName == middleInitial &&
                                                     p.LastName == lastName);
    }

    public async Task<Result<bool>> IsNameUniqueForUpdate(int id, string firstName, string? middleInitial, string lastName)
    {
        return !await _context.Person!.AsNoTracking().AnyAsync(p => p.FirstName == firstName &&
                                                     p.MiddleName == middleInitial &&
                                                     p.LastName == lastName &&
                                                     p.BusinessEntityID != id);
    }

    public async Task<Result<bool>> IsEmailUniqueForCreate(string email)
    {
        return !await _context.EmailAddress!.AsNoTracking().AnyAsync(p => p.MailAddress == email);
    }

    public async Task<Result<bool>> IsEmailUniqueForUpdate(int id, string email)
    {
        return !await _context.EmailAddress!.AsNoTracking().AnyAsync(p => p.MailAddress == email && p.BusinessEntityID != id);
    }

    public async Task<Result<bool>> IsValidStateProvinceId(int stateProvinceId)
    {
        return await _context.StateProvince!.AsNoTracking().AnyAsync(s => s.StateProvinceID == stateProvinceId);
    }

    public async Task<Result<bool>> IsValidBusinessEntityId(int businessEntityId)
    {
        return await _context.Person!.AsNoTracking().AnyAsync(s => s.BusinessEntityID == businessEntityId);
    }

    private async Task UpdatePerson(PersonDataModel dataModel)
        => await _context.Person!
            .Where(p => p.BusinessEntityID == dataModel.BusinessEntityID)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(person => person.PersonType, dataModel.PersonType)
                .SetProperty(person => person.NameStyle, dataModel.NameStyle)
                .SetProperty(person => person.Title, dataModel.Title)
                .SetProperty(person => person.FirstName, dataModel.FirstName)
                .SetProperty(person => person.MiddleName, dataModel.MiddleName)
                .SetProperty(person => person.LastName, dataModel.LastName)
                .SetProperty(person => person.Suffix, dataModel.Suffix)
                .SetProperty(person => person.EmailPromotion, dataModel.EmailPromotion)
            );

    private async Task UpdateAddress(PersonDataModel dataModel)
    {
        DataModels.Address address = dataModel.BusinessEntityAddresses.SingleOrDefault()!.Address!;

        await _context.Address!
            .Where(a => a.AddressID == address.AddressID)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(addr => addr.AddressLine1, address.AddressLine1)
                .SetProperty(addr => addr.AddressLine2, address.AddressLine2)
                .SetProperty(addr => addr.City, address.City)
                .SetProperty(addr => addr.StateProvinceID, address.StateProvinceID)
                .SetProperty(addr => addr.PostalCode, address.PostalCode)
            );
    }

    private async Task UpdateEmailAddress(PersonDataModel dataModel)
    {
        EmailAddress emailAddress = dataModel.EmailAddresses.SingleOrDefault()!;

        await _context.EmailAddress!
            .Where(a => a.BusinessEntityID == dataModel.BusinessEntityID)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(email => email.MailAddress, emailAddress.MailAddress)
            );
    }

    private async Task UpdatePhoneNumber(PersonDataModel dataModel)
    {
        DataModels.PersonPhone phone = dataModel.Telephones.SingleOrDefault()!;

        await _context.PersonPhone!
            .Where(tel => tel.BusinessEntityID == dataModel.BusinessEntityID)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(ph => ph.PhoneNumber, phone.PhoneNumber)
                .SetProperty(ph => ph.PhoneNumberTypeID, phone.PhoneNumberTypeID)
            );
    }
}
