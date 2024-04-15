using AWC.Shared.Kernel.Base;
using AWC.Shared.Kernel.Guards;

namespace AWC.PersonData.API.Domain.PersonAggregate.ValueObjects;

public sealed partial class PointOfContact : ValueObject
{
    public const int Name_Max_Length = 50;
    public const int Phone_Max_Length = 25;

    private PointOfContact(string fname, string lname, string? mi, string telephone)
    {
        FirstName = fname;
        LastName = lname;
        MiddleInitial = mi;
        Telephone = telephone;
    }

    public string FirstName { get; }
    public string LastName { get; }
    public string? MiddleInitial { get; }
    public string Telephone { get; }

    public static PointOfContact Create
    (
        string fname,
        string lname,
        string? mi,
        string telephone
    )
    {
        CheckValidity(fname, lname, mi, telephone);
        return new PointOfContact(fname, lname, mi, PhoneNumber.Create(telephone));
    }

    private static void CheckValidity(string lastName, string firstName, string? middleName, string phoneNumber)
    {
        Guard.Against.NullOrEmpty(lastName);
        Guard.Against.NullOrEmpty(firstName);

        Guard.Against.LengthGreaterThan(lastName, Name_Max_Length);
        Guard.Against.LengthGreaterThan(firstName, Name_Max_Length);

        Guard.Against.NullOrEmpty(phoneNumber);
        Guard.Against.LengthGreaterThan(phoneNumber, Phone_Max_Length);

        if (!string.IsNullOrEmpty(middleName))
        {
            Guard.Against.LengthGreaterThan(middleName, Name_Max_Length);
        }

    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return FirstName;
        yield return LastName;
        yield return MiddleInitial!;
        yield return Telephone;
    }
}
