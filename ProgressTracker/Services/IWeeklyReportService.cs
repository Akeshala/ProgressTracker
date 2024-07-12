using ProgressTracker.Models;
using ProgressTracker.ViewModels.WeeklyReport;

namespace ProgressTracker.Services;

public interface IWeeklyReportService
{
    public (List<WeeklySubjectReportViewModel> weeklySubjectReports, TimeSpan weeklyBreakTime, TimeSpan weeklyUntrackedTime,
        TimeSpan weeklyTrackedTime) GetReport(List<DailyRecordModel> dailyRecords);
}