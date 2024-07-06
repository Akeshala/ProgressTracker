using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProgressTracker.Models;
using ProgressTracker.Services;
using ProgressTracker.Utils;
using ProgressTracker.ViewModels;
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

        var viewModel = new WeeklyReportGeneratedViewModel
        {
            Date = DateTime.Today,
            DailyRecords = dailyRecords,
        };

        return View(viewModel);
    }

    private IEnumerable<WeeklySubjectReportViewModel> GetWeeklySubjectReport(
        IEnumerable<DailyRecordModel> dailyRecordModels)
    {
        foreach (var dailyRecord in dailyRecordModels)
        {
            var sessionIds = dailyRecord.SessionIds;
        }

        return [];
    }
}