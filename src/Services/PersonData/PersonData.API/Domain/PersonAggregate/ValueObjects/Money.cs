using AWC.Shared.Kernel.Base;
using AWC.Shared.Kernel.Guards;

namespace AWC.PersonData.API.Domain.PersonAggregate.ValueObjects;

public sealed class Money : ValueObject
{
    private Money(Currency currency, decimal amount)
    {
        Currency = currency;
        Amount = amount;
    }

    public Currency Currency { get; init; }
    public decimal Amount { get; init; }

    public static Money Create(Currency currency, decimal amount)
    {
        CheckValidity(currency, amount);
        return new Money(currency, amount);
    }

    public static Money operator +(Money left, Money right) => left.Currency == right.Currency
            ? new(left.Currency, left.Amount + right.Amount)
            : throw new ArgumentException($"Left money currency {left.Currency.CurrencyName} does not match right money currency {right.Currency.CurrencyName}.");

    public static Money operator -(Money left, Money right) => left.Currency == right.Currency
            ? new(left.Currency, left.Amount - right.Amount)
            : throw new ArgumentException($"Left money currency {left.Currency.CurrencyName} does not match right money currency {right.Currency.CurrencyName}.");

    private static void CheckValidity(Currency currency, decimal amount)
    {
        Guard.Against.Null(currency, "A currency is required.");
        Guard.Against.LessThan(amount, 0M, "Money can not be negative.");
        Guard.Against.GreaterThanTwoDecimalPlaces(amount);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Currency;
        yield return Amount;
    }
}
