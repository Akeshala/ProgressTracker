using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProgressTracker.Utils;
using ProgressTracker.ViewModels;
using ProgressTracker.ViewModels.WeeklyReport;

namespace ProgressTracker.Controllers;

public class WeeklyReportController : Controller
{
    
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

    // Post /WeeklyReport/GenerateReport
    [HttpPost]
    public IActionResult GenerateReport(WeeklyReportViewModel model)
    {
        (DateTime firstDate, DateTime lastDate) = DateTimeLib.GetFirstAndLastDateOfWeek(model.Date);
        TempData["Message"] = "Report generated for the week " 
                              + firstDate.ToString("yyyy-MM-dd") + " - " + lastDate.ToString("yyyy-MM-dd");
        return RedirectToAction("Index");
    }
}