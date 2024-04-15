using System.Text.RegularExpressions;
using AWC.Shared.Kernel.Base;
using AWC.Shared.Kernel.Guards;

namespace AWC.PersonData.API.Domain.PersonAggregate.ValueObjects;

public sealed partial class PhoneNumber : ValueObject
{
    private const int Max_Length = 25;

    public string Value { get; }

    private PhoneNumber(string phoneNumber)
    {
        Value = phoneNumber;
    }

    public static implicit operator string(PhoneNumber self) => self.Value;

    public static PhoneNumber Create(string phoneNumber)
    {
        CheckValidity(phoneNumber);
        return new PhoneNumber(phoneNumber);
    }

    private static void CheckValidity(string phoneNumber)
    {
        Guard.Against.NullOrEmpty(phoneNumber);
        Guard.Against.LengthGreaterThan(phoneNumber, Max_Length);

        Regex validatePhoneNumberRegex = TelephoneRegex();
        if (!validatePhoneNumberRegex.IsMatch(phoneNumber))
        {

            throw new ArgumentException($"{phoneNumber} is not a valid phone number.");
        }

    }

    [GeneratedRegex("^\\+?\\d{1,4}?[-.\\s]?\\(?\\d{1,3}?\\)?[-.\\s]?\\d{1,4}[-.\\s]?\\d{1,4}[-.\\s]?\\d{1,9}$")]
    private static partial Regex TelephoneRegex();

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
