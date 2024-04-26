#pragma warning disable CS8604

using System.Reflection;
using AWC.Company.API.Infrastructure.Persistence.DataModels;
using AWC.Company.API.Infrastructure.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace AWC.Person.API.Infrastructure.Persistence;

public class AwcContext(DbContextOptions options, IPublisher publisher) : DbContext(options), IApplicationDbContext, IUnitOfWork
{
    private readonly IPublisher _publisher = publisher;

    public virtual DbSet<AWC.Company.API.Infrastructure.Persistence.DataModels.Company>? Company { get; set; }
    public virtual DbSet<Department>? Department { get; set; }
    public virtual DbSet<Shift>? Shift { get; set; }
    public virtual DbSet<Employee>? Employee { get; set; }
    public virtual DbSet<EmployeePayHistory>? EmployeePayHistory { get; set; }
    public virtual DbSet<EmployeeDepartmentHistory>? EmployeeDepartmentHistory { get; set; }
    public virtual DbSet<EmployeeManager>? EmployeeManager { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        => await base.SaveChangesAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
