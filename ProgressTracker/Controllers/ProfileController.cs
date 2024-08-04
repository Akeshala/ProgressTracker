using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProgressTracker.ViewModels;
using ProgressTracker.ViewModels.Profile;

namespace ProgressTracker.Controllers;

public class ProfileController : Controller
{
    // GET /Profile/
    public IActionResult Index()
    {
        // Extract user ID from cookies
        var userId = HttpContext.Request.Cookies["userId"];
        if (userId == null)
        {
            return Unauthorized();
        }
        
        var studentUser = StudentViewModal.Instance();
        var university = UniversityViewModal.Instance();
        var viewModel = new ProfileViewModal()
        {
            UniversityViewModal = university,
            StudentViewModal = studentUser,
        };
        return View(viewModel);
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}