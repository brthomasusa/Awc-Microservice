namespace AWC.PersonData.API.Infrastructure.Persistence.DataModels;

public sealed class BusinessEntityContact
{
    public int BusinessEntityID { get; set; }
    public int PersonID { get; set; }
    public int ContactTypeID { get; set; }
    public Guid RowGuid { get; set; }
    public DateTime ModifiedDate { get; set; }
}
