#pragma warning disable CS8618

using System.Text.RegularExpressions;
using AWC.Shared.Kernel.Base;
using AWC.Shared.Kernel.Guards;

namespace AWC.PersonData.API.Domain.PersonAggregate.ValueObjects;

public sealed partial class WebsiteUrl : ValueObject
{
    public const int MaxLength = 50;

    public string Value { get; }

    private WebsiteUrl(string url)
    {
        Value = url;
    }


    public static implicit operator string(WebsiteUrl self) => self.Value!;

    public static WebsiteUrl Create(string url)
    {
        CheckValidity(url);
        return new WebsiteUrl(url);
    }

    private static void CheckValidity(string url)
    {
        Guard.Against.LengthGreaterThan(url, MaxLength);

        Regex Rgx = UrlRegex();

        if (!Rgx.IsMatch(url))
        {
            throw new ArgumentException("Invalid website URL!", nameof(url));
        }
    }

    [GeneratedRegex("^(?:http(s)?:\\/\\/)?[\\w.-]+(?:\\.[\\w\\.-]+)+[\\w\\-\\._~:/?#[\\]@!\\$&'\\(\\)\\*\\+,;=.]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex UrlRegex();

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
