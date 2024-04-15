namespace AWC.PersonData.API.Infrastructure.Persistence.DataModels;

public class PersonPhone
{
    public int BusinessEntityID { get; set; }
    public string? PhoneNumber { get; set; }
    public int PhoneNumberTypeID { get; set; }
    public DateTime ModifiedDate { get; set; }
}
