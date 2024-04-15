namespace AWC.PersonData.API.Infrastructure.Persistence.DataModels;

public class BusinessEntity
{
    public int BusinessEntityID { get; set; }
    public virtual PersonDataModel? PersonModel { get; set; }
    public Guid RowGuid { get; set; }
    public DateTime ModifiedDate { get; set; }
}
