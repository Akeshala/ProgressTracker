using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProgressTracker.ViewModels;

namespace ProgressTracker.Controllers;

public class PrivacyController : Controller
{
    // GET /Privacy/
    public IActionResult Index()
    {
        // Extract user ID from cookies
        var userId = HttpContext.Request.Cookies["userId"];
        if (userId == null)
        {
            return Unauthorized();
        }
        return View();
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}