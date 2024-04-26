using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWC.Company.API.Infrastructure.Persistence.DataModels
{
    public sealed class EmployeeManager
    {
        public int BusinessEntityID { get; set; }
        public short DepartmentID { get; set; }
        public string? DepartmentName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
    }
}