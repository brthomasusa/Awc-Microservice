using AWC.Shared.Kernel.Base;
using AWC.Shared.Kernel.Guards;

namespace AWC.PersonData.API.Domain.PersonAggregate.ValueObjects;

public sealed class Suffix : ValueObject
{
    private const int MaxLength = 10;

    public string? Value { get; }

    private Suffix(string? value)
    {
        Value = value;
    }


    public static implicit operator string(Suffix self) => self.Value!;

    public static Suffix Create(string? value)
    {
        CheckValidity(value!);
        return new Suffix(value!);
    }

    private static void CheckValidity(string suffix)
    {
        if (!string.IsNullOrEmpty(suffix))
        {
            Guard.Against.LengthGreaterThan(suffix, MaxLength);
        }

    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value!;
    }
}
