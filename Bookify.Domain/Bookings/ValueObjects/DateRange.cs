namespace Bookify.Domain.Bookings.ValueObjects;
public record DateRange
{
    private DateRange()
    {
    }

    public DateOnly Start { get; init; }

    public DateOnly End { get; init; }

    public int LengthInDays => End.DayNumber - Start.DayNumber;

    public static DateRange Create(DateOnly start, DateOnly end)
    {
        if (CheckStartAndEndDate(start, end))
        {
            throw new ApplicationException("End date precedes start date");
        }

        return new DateRange() { Start = start, End = end };
    }

    private static bool CheckStartAndEndDate(DateOnly start, DateOnly end)
    {
        return start > end;
    }
}
