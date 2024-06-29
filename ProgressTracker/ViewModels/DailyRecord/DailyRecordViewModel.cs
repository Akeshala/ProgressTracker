namespace ProgressTracker.ViewModels.DailyRecord;
public class DailyRecordViewModel
{    
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Target { get; set; }
    public TimeSpan Break { get; set; }
    public TimeSpan Covered { get; set; }
}