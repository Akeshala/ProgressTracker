using Microsoft.AspNetCore.Mvc.Rendering;
using ProgressTracker.Models;

namespace ProgressTracker.ViewModels.WeeklyReport;
public class WeeklySubjectReportViewModel
{
    public string SubjectName { get; set; }
    public TimeSpan Learned { get; set; }
}