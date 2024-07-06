
namespace ProgressTracker.ViewModels.WeeklyReport;
public class WeeklyReportGeneratedViewModel
{
    public DateTime Date { get; set; }
    public List<WeeklySubjectReportViewModel?> WeeklySubjectReports { get; set; }
    public TimeSpan BreakTime { get; set; }
    public TimeSpan UntrackedTime { get; set; }
    public TimeSpan TrackedTime { get; set; }
}