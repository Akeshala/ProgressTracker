using Microsoft.AspNetCore.Mvc;
using ProgressTracker.Models;
using ProgressTracker.Services;
using ProgressTracker.Utils;
using ProgressTracker.ViewModels.ResultPredictor;

namespace ProgressTracker.Controllers;

public class ResultPredictorController : Controller
{
    private readonly ISubjectService _subjectService;
    private readonly IDailyRecordService _dailyRecordService;
    private readonly ISessionService _sessionService;

    public ResultPredictorController(ISubjectService subjectService, IDailyRecordService dailyRecordService,
        ISessionService sessionService)
    {
        _subjectService = subjectService;
        _dailyRecordService = dailyRecordService;
        _sessionService = sessionService;
    }

    // Get /ResultPredictor/Index
    public IActionResult Index()
    {
        var (firstDate, lastDate) = DateTimeLib.GetDayBeforeMonths(6);
        var dailyRecords = _dailyRecordService.GetAllInRange(firstDate, lastDate);
        var subjectReports = GetSubjectReport(dailyRecords);

        var viewModel = new ResultPredictorGeneratedViewModel
        {
            SubjectReports = subjectReports.ToList(),
            FirstDate = firstDate,
            LastDate = lastDate,
        };
        return View(viewModel);
    }

    private IEnumerable<ResultPredictorSubjectViewModel> GetSubjectReport(
        IEnumerable<DailyRecordModel?> dailyRecords)
    {
        if (!dailyRecords.Any())
        {
            return Enumerable.Empty<ResultPredictorSubjectViewModel>();
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

        var subjectReports = learnedTimeBySubjectId.Select(kvp =>
        {
            var subject = _subjectService.GetOneById(kvp.Key);
            var target = new TimeSpan(subject?.LearningHours ?? 0, 0, 0);
            return new ResultPredictorSubjectViewModel
            {
                SubjectId = kvp.Key,
                SubjectName = subject?.Name ?? "Unknown Subject",
                Learned = kvp.Value,
                Target = target,
                Grade = GetStudyRatio(kvp.Value, target),
            };
        }).ToList();

        return subjectReports;
    }

    private string GetStudyRatio(TimeSpan studied, TimeSpan target)
    {
        if (studied > target)
        {
            throw new ArgumentException("The studied date must be before the target date.");
        }

        var studyRatio = studied.TotalSeconds / target.TotalSeconds;

        return studyRatio switch
        {
            >= 0.85 => "A+",
            >= 0.75 => "A",
            >= 0.70 => "A-",
            >= 0.65 => "B+",
            >= 0.60 => "B",
            >= 0.55 => "B-",
            >= 0.50 => "C+",
            >= 0.45 => "C",
            >= 0.40 => "C-",
            >= 0.35 => "D",
            _ => "F",
        };
    }

}