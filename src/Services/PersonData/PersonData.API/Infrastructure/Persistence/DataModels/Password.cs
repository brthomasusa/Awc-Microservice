namespace AWC.PersonData.API.Infrastructure.Persistence.DataModels;

public class Password
{
    public int BusinessEntityID { get; set; }
    public string? PasswordHash { get; set; }
    public string? PasswordSalt { get; set; }
    public Guid RowGuid { get; set; }
    public DateTime ModifiedDate { get; set; }
}
