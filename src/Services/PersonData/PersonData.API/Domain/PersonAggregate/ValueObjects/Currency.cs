using AWC.Shared.Kernel.Base;
using AWC.Shared.Kernel.Guards;

namespace AWC.PersonData.API.Domain.PersonAggregate.ValueObjects;

public sealed class Currency : ValueObject
{
    private Currency(string code, string name)
    {
        (CurrencyCode, CurrencyName) = (code, name);
    }


    public string CurrencyCode { get; init; }
    public string CurrencyName { get; init; }

    public static Currency Create(string code, string name)
    {
        CheckValidity(code, name);
        return new Currency(code, name);
    }

    private static void CheckValidity(string currencyCode, string currencyName)
    {
        Guard.Against.NullOrEmpty(currencyCode, "A currency code (USD, EUR, etc) is required.");
        Guard.Against.LengthGreaterThan(currencyCode, 3);

        Guard.Against.NullOrEmpty(currencyName);
        Guard.Against.LengthGreaterThan(currencyName, 50);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return CurrencyCode;
        yield return CurrencyName;
    }
}
