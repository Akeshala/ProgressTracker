using Microsoft.AspNetCore.Mvc.Rendering;
using ProgressTracker.Models;
using ProgressTracker.ViewModels.Session;

namespace ProgressTracker.ViewModels.DailyRecord;
public class EditDailyRecordViewModel
{
    public int? BreakHours { get; set; }
    public int? BreakMinutes { get; set; }
    public int? SubjectSelectedValue { get; set; }
    public int? SubjectHours { get; set; }
    public int? SubjectMinutes { get; set; }
    public TimeSpan? Learned { get; set; }
    public DailyRecordModel DailyRecord { get; set; }
    public List<SelectListItem>? SubjectOptions { get; set; }
    public List<SessionViewModel>? Sessions { get; set; }
}