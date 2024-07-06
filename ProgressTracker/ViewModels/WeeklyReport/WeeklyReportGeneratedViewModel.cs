using Microsoft.AspNetCore.Mvc.Rendering;
using ProgressTracker.Models;

namespace ProgressTracker.ViewModels.WeeklyReport;
public class WeeklyReportGeneratedViewModel
{
    public DateTime Date { get; set; }
    public IEnumerable<DailyRecordModel?> DailyRecords { get; set; }
}