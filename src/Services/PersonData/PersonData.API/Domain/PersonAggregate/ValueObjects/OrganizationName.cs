using AWC.Shared.Kernel.Base;
using AWC.Shared.Kernel.Guards;

namespace AWC.PersonData.API.Domain.PersonAggregate.ValueObjects;

public sealed class OrganizationName : ValueObject
{
    public const int Max_Length = 50;

    public string? Value { get; }

    private OrganizationName(string? orgName)
    {
        Value = orgName;
    }


    public static implicit operator string(OrganizationName self) => self.Value!;

    public static OrganizationName Create(string? orgName)
    {
        CheckValidity(orgName!);
        return new OrganizationName(orgName);
    }

    private static void CheckValidity(string? organizationName)
    {
        if (!string.IsNullOrEmpty(organizationName))
        {
            Guard.Against.LengthGreaterThan(organizationName, Max_Length);
        }

    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value!;
    }
}
