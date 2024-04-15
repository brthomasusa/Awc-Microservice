namespace AWC.PersonData.API.Infrastructure.Persistence.DataModels;

public class BusinessEntityAddress
{
    public int BusinessEntityID { get; set; }
    public int AddressID { get; set; }
    public virtual Address? Address { get; set; }
    public int AddressTypeID { get; set; }
    public Guid RowGuid { get; set; }
    public DateTime ModifiedDate { get; set; }
}
