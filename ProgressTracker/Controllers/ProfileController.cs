using Microsoft.AspNetCore.Mvc;
using ProgressTracker.ViewModels;

namespace ProgressTracker.Controllers;

public class ProfileController : Controller
{
    // GET /Profile/
    public IActionResult Index()
    {
        var user = new User()
        {
            Id = 1,
            Name = "Akeshala",
            Email = "chavinduakeshala@gmail.com",
            Age = 27,
        };
        
        var university = new University()
        {
            Id = 1,
            Name = "University of Moratuwa",
            CourseName = "Electronic and Telecommunication Engineering",
            Location = "Moratuwa",
        };
        var viewModel = new Profile()
        {
            University = university,
            User = user,
        };
        return View(viewModel);
    }
}