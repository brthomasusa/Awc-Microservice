using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using AWC.Company.API.Infrastructure.Persistence.DataModels;

namespace AWC.Company.API.Infrastructure.Persistence.Interfaces;

public interface IApplicationDbContext
{
    DbSet<AWC.Company.API.Infrastructure.Persistence.DataModels.Company>? Company { get; set; }
    DbSet<Department>? Department { get; set; }
    DbSet<Shift>? Shift { get; set; }
    DbSet<Employee>? Employee { get; set; }
    DbSet<EmployeePayHistory>? EmployeePayHistory { get; set; }
    DbSet<EmployeeDepartmentHistory>? EmployeeDepartmentHistory { get; set; }
    DbSet<EmployeeManager>? EmployeeManager { get; set; }

    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
