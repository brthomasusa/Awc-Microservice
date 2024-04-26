using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWC.Company.API.Infrastructure.Persistence.DataModels
{
    public sealed class EmployeePayHistory
    {
        public int BusinessEntityID { get; set; }
        public DateTime RateChangeDate { get; set; }
        public decimal Rate { get; set; }
        public byte PayFrequency { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}