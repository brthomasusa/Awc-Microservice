using AWC.Shared.Kernel.Base;
using AWC.Shared.Kernel.Guards;

namespace AWC.PersonData.API.Domain.PersonAggregate.ValueObjects;

public sealed class PersonType : ValueObject
{
    private static readonly string[] _personTypes = { "SC", "IN", "SP", "EM", "VC", "GC" };
    public string Value { get; }

    private PersonType(string personType)
    {
        Value = personType;
    }


    public static implicit operator string(PersonType self) => self.Value!;

    public static PersonType Create(string value)
    {
        CheckValidity(value);
        return new PersonType(value);
    }

    private static void CheckValidity(string personType)
    {
        Guard.Against.NullOrEmpty(personType);

        if (!Array.Exists(_personTypes, element => element == personType.ToUpper()))
        {
            throw new ArgumentException("Invalid person type. Valid types are SC, IN, SP, EM, VC, and GC.");
        }
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
