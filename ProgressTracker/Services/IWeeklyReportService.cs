using ProgressTracker.Models;
using ProgressTracker.ViewModels.WeeklyReport;

namespace ProgressTracker.Services;

public interface IWeeklyReportService
{
    public
        Task<(List<WeeklySubjectReportViewModel> weeklySubjectReports, TimeSpan weeklyBreakTime, TimeSpan
            weeklyUntrackedTime, TimeSpan weeklyTrackedTime)> GetReport(List<DailyRecordModel> dailyRecords);
}