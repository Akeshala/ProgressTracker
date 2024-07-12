using Microsoft.AspNetCore.Mvc;
using ProgressTracker.Services;
using ProgressTracker.Utils;
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
    public IActionResult Index()
    {
        var (firstDate, lastDate) = DateTimeLib.GetDayBeforeMonths(6);
        var dailyRecords = _dailyRecordService.GetAllInRange(firstDate, lastDate);
        var predictions = _resultPredictorService.GetPredictions(dailyRecords);
        
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
}