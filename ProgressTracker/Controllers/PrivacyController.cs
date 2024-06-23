using Microsoft.AspNetCore.Mvc;

namespace ProgressTracker.Controllers;

public class PrivacyController : Controller
{
    // GET /Privacy/
    public IActionResult Index()
    {
        return View();
    }
}