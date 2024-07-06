using Microsoft.AspNetCore.Mvc;
using ProgressTracker.Models;
using ProgressTracker.Services;
using ProgressTracker.Utils;
using ProgressTracker.ViewModels.DailyRecord;
using ProgressTracker.ViewModels.WeeklyReport;

namespace ProgressTracker.Controllers;

public class WeeklyReportController : Controller
{
    private readonly ISubjectService _subjectService;
    private readonly IDailyRecordService _dailyRecordService;
    private readonly ISessionService _sessionService;

    public WeeklyReportController(ISubjectService subjectService, IDailyRecordService dailyRecordService,
        ISessionService sessionService)
    {
        _subjectService = subjectService;
        _dailyRecordService = dailyRecordService;
        _sessionService = sessionService;
    }

    // GET /WeeklyReport/Index
    [HttpGet]
    public IActionResult Index()
    {
        var viewModel = new WeeklyReportViewModel
        {
            Date = DateTime.Today
        };
        return View(viewModel);
    }

    // Get /WeeklyReport/GenerateReport
    public IActionResult Generate(DateTime date)
    {
        (DateTime firstDate, DateTime lastDate) = DateTimeLib.GetFirstAndLastDateOfWeek(date);
        TempData["Message"] = "Report generated for the week "
                              + firstDate.ToString("yyyy-MM-dd") + " - " + lastDate.ToString("yyyy-MM-dd");
        var dailyRecords = _dailyRecordService.GetAllInRange(firstDate, lastDate);
        var weeklySubjectReports = GetWeeklySubjectReport(dailyRecords);
        var weeklyBreakTime = GetWeeklyBreakTime(dailyRecords);
        var weeklyUntrackedTime = GetWeeklyUntrackedTime(dailyRecords);
        var weeklyTrackedTime = GetWeeklyTrackedTime(dailyRecords);

        var viewModel = new WeeklyReportGeneratedViewModel
        {
            Date = DateTime.Today,
            DailyRecords = dailyRecords.Select(dailyRecord => new DailyRecordViewModel
            {
                DailyRecordModel = dailyRecord,
                Learned = GetLearned(dailyRecord),
            }).ToList(),
            WeeklySubjectReports = weeklySubjectReports.ToList(),
            BreakTime = weeklyBreakTime,
            UntrackedTime = weeklyUntrackedTime,
            TrackedTime = weeklyTrackedTime,
        };
        return View(viewModel);
    }

    private IEnumerable<WeeklySubjectReportViewModel> GetWeeklySubjectReport(
        IEnumerable<DailyRecordModel?> dailyRecords)
    {
        if (!dailyRecords.Any())
        {
            return Enumerable.Empty<WeeklySubjectReportViewModel>();
        }
        
        Dictionary<int, TimeSpan> learnedTimeBySubjectId = new Dictionary<int, TimeSpan>();

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

    private TimeSpan GetWeeklyBreakTime(IEnumerable<DailyRecordModel?> dailyRecords)
    {
        if (!dailyRecords.Any())
        {
            return new TimeSpan(0, 0, 0);
        }

        var breakTime = new TimeSpan(0, 0, 0);
        foreach (var dailyRecord in dailyRecords)
        {
            var DailyBreakTime = dailyRecord.Break;
            breakTime += DailyBreakTime;
        }
        
        return breakTime;
    }
    
    private TimeSpan GetWeeklyUntrackedTime(IEnumerable<DailyRecordModel?> dailyRecords)
    {
        if (!dailyRecords.Any())
        {
            return new TimeSpan(0, 0, 0);
        }

        var unTrackedTime = new TimeSpan(0, 0, 0);
        foreach (var dailyRecord in dailyRecords)
        {
            var dailyUntrackedTime = GetUntracked(dailyRecord);
            unTrackedTime += dailyUntrackedTime;
        }
        
        return unTrackedTime;
    }
    
    private TimeSpan GetUntracked(DailyRecordModel dailyRecord)
    {
        var sessionIds = dailyRecord.SessionIds;
        var breakTime = dailyRecord.Break;
        var target = dailyRecord.GetTarget();
        return target - (_sessionService.GetMultiByIds(sessionIds)
            .Aggregate(TimeSpan.Zero, (ac, session) => ac + session.Time) + breakTime);
    }
    
    private TimeSpan GetWeeklyTrackedTime(IEnumerable<DailyRecordModel?> dailyRecords)
    {
        if (!dailyRecords.Any())
        {
            return new TimeSpan(0, 0, 0);
        }

        var trackedTime = new TimeSpan(0, 0, 0);
        foreach (var dailyRecord in dailyRecords)
        {
            var dailyTrackedTime = GetTracked(dailyRecord);
            trackedTime += dailyTrackedTime;
        }
        
        return trackedTime;
    }
    
    private TimeSpan GetTracked(DailyRecordModel dailyRecord)
    {
        var sessionIds = dailyRecord.SessionIds;
        var breakTime = dailyRecord.Break;
        return _sessionService.GetMultiByIds(sessionIds)
            .Aggregate(TimeSpan.Zero, (ac, session) => ac + session.Time) + breakTime;
    }
    
    private TimeSpan GetLearned(DailyRecordModel dailyRecord)
    {
        var sessionIds = dailyRecord.SessionIds;
        return _sessionService.GetMultiByIds(sessionIds)
            .Aggregate(TimeSpan.Zero, (ac, session) => ac + session.Time);
    }
}