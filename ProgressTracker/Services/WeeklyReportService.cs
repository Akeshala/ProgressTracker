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

    public (List<WeeklySubjectReportViewModel> weeklySubjectReports, TimeSpan weeklyBreakTime, TimeSpan
        weeklyUntrackedTime,
        TimeSpan weeklyTrackedTime) GetReport(List<DailyRecordModel> dailyRecords)
    {
        var weeklySubjectReports = GetWeeklySubjectReport(dailyRecords);
        var weeklyBreakTime = GetWeeklyBreakTime(dailyRecords);
        var weeklyUntrackedTime = GetWeeklyUntrackedTime(dailyRecords);
        var weeklyTrackedTime = GetWeeklyTrackedTime(dailyRecords);

        return (weeklySubjectReports, weeklyBreakTime, weeklyUntrackedTime, weeklyTrackedTime);
    }


    private List<WeeklySubjectReportViewModel> GetWeeklySubjectReport(List<DailyRecordModel> dailyRecords)
    {
        if (dailyRecords.Count == 0)
        {
            return [];
        }

        var learnedTimeBySubjectId = new Dictionary<int, TimeSpan>();

        foreach (var dailyRecord in dailyRecords)
        {
            var sessionIds = dailyRecord.SessionIds;
            var sessions = _sessionService.GetMultiByIds(sessionIds);

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

        var weeklySubjectReports = learnedTimeBySubjectId.Select(kvp => new WeeklySubjectReportViewModel
        {
            SubjectId = kvp.Key,
            SubjectName = _subjectService.GetOneById(kvp.Key)?.Name ?? "Unknown Subject",
            Learned = kvp.Value
        }).ToList();

        return weeklySubjectReports;
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

    private TimeSpan GetWeeklyUntrackedTime(List<DailyRecordModel> dailyRecords)
    {
        if (dailyRecords.Count == 0)
        {
            return new TimeSpan(0, 0, 0);
        }

        var unTrackedTime = new TimeSpan(0, 0, 0);

        return dailyRecords.Select(_dailyRecordService.GetUntracked).Aggregate(unTrackedTime,
            (current, dailyUntrackedTime) => current + dailyUntrackedTime);
    }

    private TimeSpan GetWeeklyTrackedTime(List<DailyRecordModel> dailyRecords)
    {
        if (dailyRecords.Count == 0)
        {
            return new TimeSpan(0, 0, 0);
        }

        var trackedTime = new TimeSpan(0, 0, 0);

        return dailyRecords.Select(_dailyRecordService.GetTracked).Aggregate(trackedTime,
            (current, dailyTrackedTime) => current + dailyTrackedTime);
    }
}