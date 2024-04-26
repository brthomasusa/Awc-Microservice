namespace AWC.Company.API.Infrastructure.Persistence.DataModels
{
    public sealed class Department
    {
        public Int16 DepartmentID { get; set; }
        public string? Name { get; set; }
        public string? GroupName { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}