namespace ProgressTracker.Utils;

public static class DateTimeLib
{
    public static (DateTime firstDate, DateTime lastDate) GetFirstAndLastDateOfWeek(DateTime date)
    {
        var firstDate = date.AddDays(-(int)date.DayOfWeek + 1);
        var lastDate = firstDate.AddDays(6);
        return (firstDate, lastDate);
    }
}