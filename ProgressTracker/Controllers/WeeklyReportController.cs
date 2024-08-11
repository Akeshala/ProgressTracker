using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProgressTracker.Services;
using ProgressTracker.Utils;
using ProgressTracker.ViewModels;
using ProgressTracker.ViewModels.DailyRecord;
using ProgressTracker.ViewModels.WeeklyReport;

namespace ProgressTracker.Controllers;

[AuthorizeToken]
public class WeeklyReportController : Controller
{
    private readonly IDailyRecordService _dailyRecordService;
    private readonly IWeeklyReportService _weeklyReportService;

    public WeeklyReportController(IWeeklyReportService weeklyReportService, IDailyRecordService dailyRecordService)
    {
        _weeklyReportService = weeklyReportService;
        _dailyRecordService = dailyRecordService;
    }

    // GET /WeeklyReport/Index
    [HttpGet]
    public IActionResult Index()
    {
        // Extract user ID from cookies
        string? userId = HttpContext.Request.Cookies["userId"];
        if (userId == null)
        {
            return Unauthorized();
        }

        var viewModel = new WeeklyReportViewModel
        {
            Date = DateTime.Today
        };
        return View(viewModel);
    }

    // Get /WeeklyReport/GenerateReport
    public async Task<IActionResult> Generate(DateTime date)
    {
        // Extract user ID from cookies
        string? userId = HttpContext.Request.Cookies["userId"];
        if (userId == null)
        {
            return Unauthorized();
        }

        var (firstDate, lastDate) = DateTimeLib.GetFirstAndLastDateOfWeek(date);

        // Fetch daily records asynchronously
        var dailyRecordsTask = _dailyRecordService.GetAllInRangeByUser(firstDate, lastDate, int.Parse(userId));

        // Fetch report asynchronously
        var reportTask = dailyRecordsTask.ContinueWith(async task =>
        {
            var localDailyRecords = task.Result;
            var report = await _weeklyReportService.GetReport(localDailyRecords);
            return (dailyRecords: localDailyRecords, report);
        }).Unwrap();

        var (dailyRecords, (weeklySubjectReports, weeklyBreakTime, weeklyUntrackedTime, weeklyTrackedTime)) =
            await reportTask;

        // Create tasks to fetch learned information in parallel
        var dailyRecordTasks = dailyRecords.Select(dailyRecord => Task.Run(async () => new DailyRecordViewModel
        {
            DailyRecordModel = dailyRecord,
            Learned = await _dailyRecordService.GetLearned(dailyRecord),
        }));

        var dailyRecordViews = await Task.WhenAll(dailyRecordTasks);

        var viewModel = new WeeklyReportGeneratedViewModel
        {
            Date = DateTime.Today,
            DailyRecords = dailyRecordViews.ToList(),
            WeeklySubjectReports = weeklySubjectReports,
            BreakTime = weeklyBreakTime,
            UntrackedTime = weeklyUntrackedTime,
            TrackedTime = weeklyTrackedTime,
        };

        TempData["Message"] = "Report generated for the week " + firstDate.ToString(DateTimeLib.Ymd) + " - "
                              + lastDate.ToString(DateTimeLib.Ymd);

        return View(viewModel);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}