using Microsoft.AspNetCore.Mvc;
using ProgressTracker.ViewModels.ProfileInfo;

namespace ProgressTracker.Controllers;

public class ProfileController : Controller
{
    // GET /Profile/
    public IActionResult Index()
    {
        var studentUser = Student.Instance();
        var university = University.Instance();
        var viewModel = new Profile()
        {
            University = university,
            Student = studentUser,
        };
        return View(viewModel);
    }
}