namespace ProgressTracker.Utils;

public static class DateTimeLib
{
    public static (DateTime firstDate, DateTime lastDate) GetFirstAndLastDateOfWeek(DateTime date)
    {
        var firstDate = date.AddDays(-(int)date.DayOfWeek + 1);
        var lastDate = firstDate.AddDays(6);
        return (firstDate, lastDate);
    }

    public static (DateTime firstDate, DateTime lastDate) GetDayBeforeMonths(int numberOfMonths)
    {
        DateTime currentDate = DateTime.Now;
        DateTime dateSixMonthsAgo = currentDate.AddMonths(-numberOfMonths);
        DateTime dayBeforeSixMonthsAgo = dateSixMonthsAgo.AddDays(-1);
        return (dayBeforeSixMonthsAgo, currentDate);
    }
}