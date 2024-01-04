using Bookify.Application.Abstractions.Clock;

namespace Bookify.Infrasctucture.Clock;
internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
