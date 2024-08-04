using ProgressTracker.Models;
using ProgressTracker.ViewModels.WeeklyReport;

namespace ProgressTracker.Services;

public class WeeklyReportService : IWeeklyReportService
{
    private readonly ISubjectService _subjectService;
    private readonly ISessionService _sessionService;
    private readonly IDailyRecordService _dailyRecordService;

    public WeeklyReportService(ISubjectService subjectService, ISessionService sessionService,
        IDailyRecordService dailyRecordService)
    {
        _subjectService = subjectService;
        _sessionService = sessionService;
        _dailyRecordService = dailyRecordService;
    }

    public async
        Task<(List<WeeklySubjectReportViewModel> weeklySubjectReports, TimeSpan weeklyBreakTime, TimeSpan
            weeklyUntrackedTime, TimeSpan weeklyTrackedTime)> GetReport(List<DailyRecordModel> dailyRecords)
    {
        var weeklySubjectReports = await GetWeeklySubjectReport(dailyRecords);
        var weeklyBreakTime = GetWeeklyBreakTime(dailyRecords);
        var weeklyUntrackedTime = await GetWeeklyUntrackedTime(dailyRecords);
        var weeklyTrackedTime = await GetWeeklyTrackedTime(dailyRecords);

        return (weeklySubjectReports, weeklyBreakTime, weeklyUntrackedTime, weeklyTrackedTime);
    }


    private async Task<List<WeeklySubjectReportViewModel>> GetWeeklySubjectReport(List<DailyRecordModel> dailyRecords)
    {
        if (dailyRecords.Count == 0)
        {
            return [];
        }

        var learnedTimeBySubjectId = new Dictionary<int, TimeSpan>();

        foreach (var dailyRecord in dailyRecords)
        {
            var sessionIds = dailyRecord.SessionIds;
            var sessions = await _sessionService.GetMultiByIds(sessionIds);

            foreach (var session in sessions)
            {
                var subjectId = session.SubjectId;
                var time = session.Time;

                if (!learnedTimeBySubjectId.TryAdd(subjectId, time))
                {
                    learnedTimeBySubjectId[subjectId] += time;
                }
            }
        }

        var viewModelTasks = learnedTimeBySubjectId.Select(async kvp =>
        {
            var subjectName = await _subjectService.GetOneById(kvp.Key);
            return new WeeklySubjectReportViewModel
            {
                SubjectId = kvp.Key,
                SubjectName = subjectName?.Name ?? "Unknown Subject",
                Learned = kvp.Value
            };
        }).ToList();

        var weeklySubjectReports = await Task.WhenAll(viewModelTasks);

        return weeklySubjectReports.ToList();
    }

    private TimeSpan GetWeeklyBreakTime(List<DailyRecordModel> dailyRecords)
    {
        if (dailyRecords.Count == 0)
        {
            return new TimeSpan(0, 0, 0);
        }

        var breakTime = new TimeSpan(0, 0, 0);

        return dailyRecords.Select(dailyRecord => dailyRecord.Break)
            .Aggregate(breakTime, (current, dailyBreakTime) => current + dailyBreakTime);
    }

    private async Task<TimeSpan> GetWeeklyUntrackedTime(List<DailyRecordModel> dailyRecords)
    {
        if (dailyRecords.Count == 0)
        {
            return new TimeSpan(0, 0, 0);
        }

        var unTrackedTime = new TimeSpan(0, 0, 0);

        // Get untracked times asynchronously
        var untrackedTimesTasks = dailyRecords.Select(_dailyRecordService.GetUntracked).ToList();
        var untrackedTimes = await Task.WhenAll(untrackedTimesTasks);

        // Aggregate the untracked times
        foreach (var dailyUntrackedTime in untrackedTimes)
        {
            unTrackedTime += dailyUntrackedTime;
        }

        return unTrackedTime;
    }

    private async Task<TimeSpan> GetWeeklyTrackedTime(List<DailyRecordModel> dailyRecords)
    {
        if (dailyRecords.Count == 0)
        {
            return new TimeSpan(0, 0, 0);
        }

        var trackedTime = new TimeSpan(0, 0, 0);

        // Get tracked times asynchronously
        var trackedTimesTasks = dailyRecords.Select(_dailyRecordService.GetTracked).ToList();
        var trackedTimes = await Task.WhenAll(trackedTimesTasks);

        // Aggregate the tracked times
        foreach (var dailyTrackedTime in trackedTimes)
        {
            trackedTime += dailyTrackedTime;
        }

        return trackedTime;
    }
}