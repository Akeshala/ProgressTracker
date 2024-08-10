using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProgressTracker.Services;
using ProgressTracker.Utils;
using ProgressTracker.ViewModels;
using ProgressTracker.ViewModels.ResultPredictor;

namespace ProgressTracker.Controllers;

public class ResultPredictorController : Controller
{
    private readonly IDailyRecordService _dailyRecordService;
    private readonly IResultPredictorService _resultPredictorService;

    public ResultPredictorController(IResultPredictorService resultPredictorService,
        IDailyRecordService dailyRecordService)
    {
        _resultPredictorService = resultPredictorService;
        _dailyRecordService = dailyRecordService;
    }

    // Get /ResultPredictor/Index
    public async Task<IActionResult> Index()
    {
        // Extract user ID from cookies
        string? userId = HttpContext.Request.Cookies["userId"];
        if (userId == null)
        {
            return Unauthorized();
        }
        
        var (firstDate, lastDate) = DateTimeLib.GetDayBeforeMonths(6);
        var dailyRecords = await _dailyRecordService.GetAllInRangeByUser(firstDate, lastDate, int.Parse(userId));
        var predictions = await _resultPredictorService.GetPredictions(dailyRecords);
        
        var subjectReports = predictions.Select(kvp =>
        {
            var (subjectId, subjectName, learned, target, grade, level) = kvp;
            return new ResultPredictorSubjectViewModel
            {
                SubjectId = subjectId,
                SubjectName = subjectName,
                Learned = learned,
                Target = target,
                Grade = grade,
                Level = level,
            };
        }).ToList();

        var viewModel = new ResultPredictorGeneratedViewModel
        {
            SubjectReports = subjectReports,
            FirstDate = firstDate,
            LastDate = lastDate,
        };
        return View(viewModel);
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}