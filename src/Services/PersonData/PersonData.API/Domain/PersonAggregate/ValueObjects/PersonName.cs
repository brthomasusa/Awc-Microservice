using AWC.Shared.Kernel.Base;
using AWC.Shared.Kernel.Guards;

namespace AWC.PersonData.API.Domain.PersonAggregate.ValueObjects;

public sealed class PersonName : ValueObject
{
    private PersonName(string last, string first, string? mi)
    {
        FirstName = first;
        LastName = last;
        MiddleName = mi;
    }

    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string? MiddleName { get; init; }

    public static PersonName Create(string last, string first, string? mi)
    {
        CheckValidity(last, first, mi);
        return new PersonName(last, first, mi);
    }

    private static void CheckValidity(string lastName, string firstName, string? middleName)
    {
        Guard.Against.NullOrEmpty(lastName);
        Guard.Against.LengthGreaterThan(lastName, 50);

        Guard.Against.NullOrEmpty(firstName);
        Guard.Against.LengthGreaterThan(firstName, 50);

        if (!string.IsNullOrEmpty(middleName))
        {
            Guard.Against.LengthGreaterThan(middleName, 50);
        }
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return FirstName;
        yield return LastName;
        yield return MiddleName!;
    }
}
