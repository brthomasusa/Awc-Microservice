using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using AWC.PersonData.API.Infrastructure.Persistence.DataModels;

namespace AWC.PersonData.API.Infrastructure.Persistence.Interfaces;

public interface IApplicationDbContext
{
    public DbSet<AddressType>? AddressType { get; set; }
    public DbSet<ContactType>? ContactType { get; set; }
    public DbSet<PhoneNumberType>? PhoneNumberType { get; set; }
    public DbSet<CountryRegion>? CountryRegion { get; set; }
    public DbSet<StateProvince>? StateProvince { get; set; }
    public DbSet<BusinessEntity>? BusinessEntity { get; set; }
    public DbSet<BusinessEntityAddress>? BusinessEntityAddress { get; set; }
    public DbSet<BusinessEntityContact>? BusinessEntityContact { get; set; }
    public DbSet<Address>? Address { get; set; }
    public DbSet<PersonDataModel>? Person { get; set; }
    public DbSet<EmailAddress>? EmailAddress { get; set; }
    public DbSet<PersonPhone>? PersonPhone { get; set; }
    public DbSet<Password>? Password { get; set; }
    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
