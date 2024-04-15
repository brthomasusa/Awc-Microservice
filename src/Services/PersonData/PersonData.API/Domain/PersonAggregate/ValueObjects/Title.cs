using AWC.Shared.Kernel.Base;
using AWC.Shared.Kernel.Guards;

namespace AWC.PersonData.API.Domain.PersonAggregate.ValueObjects;

public sealed class Title : ValueObject
{
    public const int MaxLength = 8;
    public string? Value { get; }

    private Title(string? title)
    {
        Value = title;
    }


    public static implicit operator string(Title self) => self.Value!;

    public static Title Create(string? value)
    {
        CheckValidity(value!);
        return new Title(value);
    }

    private static void CheckValidity(string? title)
    {
        if (!string.IsNullOrEmpty(title))
        {
            Guard.Against.LengthGreaterThan(title, MaxLength);
        }

    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value!;
    }
}
