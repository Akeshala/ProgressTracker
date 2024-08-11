using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProgressTracker.Services;
using ProgressTracker.Utils;
using ProgressTracker.ViewModels;

namespace ProgressTracker.Controllers;

[AuthorizeToken]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}