using Microsoft.AspNetCore.Mvc.Rendering;
using ProgressTracker.ViewModels.Session;

namespace ProgressTracker.ViewModels.DailyRecord;
public class EditDailyRecordViewModel
{
    public int DailyTarget { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }
    public int BreakHours { get; set; }
    public int BreakMinutes { get; set; }
    public int SubjectSelectedValue { get; set; }
    public List<SelectListItem> SubjectOptions { get; set; }
    public int SubjectHours { get; set; }
    public int SubjectMinutes { get; set; }
    public TimeSpan Covered { get; set; }
    public List<SessionViewModel> Sessions { get; set; }
}