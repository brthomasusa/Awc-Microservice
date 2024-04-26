namespace AWC.Company.API.Infrastructure.Persistence.DataModels
{
    public sealed class EmployeeDepartmentHistory
    {
        public int BusinessEntityID { get; set; }
        public Int16 DepartmentID { get; set; }
        public byte ShiftID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}