using Microsoft.AspNetCore.Mvc;
using ProgressTracker.ViewModels.Profile;

namespace ProgressTracker.Controllers;

public class ProfileController : Controller
{
    // GET /Profile/
    public IActionResult Index()
    {
        var studentUser = StudentViewModal.Instance();
        var university = UniversityViewModal.Instance();
        var viewModel = new ProfileViewModal()
        {
            UniversityViewModal = university,
            StudentViewModal = studentUser,
        };
        return View(viewModel);
    }
}