namespace AWC.PersonData.API.Infrastructure.Persistence.DataModels;

public sealed class ContactType
{
    public int ContactTypeID { get; set; }
    public string? Name { get; set; }
    public DateTime ModifiedDate { get; set; }
}
