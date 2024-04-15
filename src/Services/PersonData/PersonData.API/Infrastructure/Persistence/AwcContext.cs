#pragma warning disable CS8604

using System.Reflection;
using AWC.PersonData.API.Infrastructure.Persistence.DataModels;
using AWC.PersonData.API.Infrastructure.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace AWC.PersonData.API.Infrastructure.Persistence;

public class AwcContext(DbContextOptions options, IPublisher publisher) : DbContext(options), IApplicationDbContext, IUnitOfWork
{
    private readonly IPublisher _publisher = publisher;

    public virtual DbSet<AddressType>? AddressType { get; set; }
    public virtual DbSet<ContactType>? ContactType { get; set; }
    public virtual DbSet<PhoneNumberType>? PhoneNumberType { get; set; }
    public virtual DbSet<CountryRegion>? CountryRegion { get; set; }
    public virtual DbSet<StateProvince>? StateProvince { get; set; }
    public virtual DbSet<BusinessEntity>? BusinessEntity { get; set; }
    public virtual DbSet<BusinessEntityAddress>? BusinessEntityAddress { get; set; }
    public virtual DbSet<BusinessEntityContact>? BusinessEntityContact { get; set; }
    public virtual DbSet<Address>? Address { get; set; }
    public virtual DbSet<PersonDataModel>? Person { get; set; }
    public virtual DbSet<EmailAddress>? EmailAddress { get; set; }
    public virtual DbSet<PersonPhone>? PersonPhone { get; set; }
    public virtual DbSet<Password>? Password { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        => await base.SaveChangesAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
