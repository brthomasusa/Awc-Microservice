using AWC.Shared.Kernel.Utilities;

namespace AWC.PersonData.API.Infrastructure.Persistence;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
