using Microsoft.AspNetCore.Mvc;
using ProgressTracker.Services;
using ProgressTracker.Utils;
using ProgressTracker.ViewModels.DailyRecord;
using ProgressTracker.ViewModels.WeeklyReport;

namespace ProgressTracker.Controllers;

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
        var viewModel = new WeeklyReportViewModel
        {
            Date = DateTime.Today
        };
        return View(viewModel);
    }

    // Get /WeeklyReport/GenerateReport
    public async Task<IActionResult> Generate(DateTime date)
    {
        var (firstDate, lastDate) = DateTimeLib.GetFirstAndLastDateOfWeek(date);
        var dailyRecords = _dailyRecordService.GetAllInRange(firstDate, lastDate);
        var (weeklySubjectReports, weeklyBreakTime, weeklyUntrackedTime, weeklyTrackedTime) =
            _weeklyReportService.GetReport(dailyRecords);
        var viewModel = new WeeklyReportGeneratedViewModel
        {
            Date = DateTime.Today,
            DailyRecords = dailyRecords.Select(dailyRecord => new DailyRecordViewModel
            {
                DailyRecordModel = dailyRecord,
                Learned = _dailyRecordService.GetLearned(dailyRecord),
            }).ToList(),
            WeeklySubjectReports = await weeklySubjectReports,
            BreakTime = weeklyBreakTime,
            UntrackedTime = weeklyUntrackedTime,
            TrackedTime = weeklyTrackedTime,
        };

        TempData["Message"] = "Report generated for the week " + firstDate.ToString(DateTimeLib.Ymd) + " - "
                              + lastDate.ToString(DateTimeLib.Ymd);

        return View(viewModel);
    }
}