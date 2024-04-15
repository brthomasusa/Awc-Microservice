namespace AWC.PersonData.API.Infrastructure.Persistence.DataModels;

public sealed class EmailAddress
{
    public int BusinessEntityID { get; set; }
    public int EmailAddressID { get; set; }
    public string? MailAddress { get; set; }
    public Guid RowGuid { get; set; }
    public DateTime ModifiedDate { get; set; }
}
