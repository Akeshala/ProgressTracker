using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProgressTracker.Models;
using ProgressTracker.ViewModels.Shared;

namespace ProgressTracker.Controllers;

public class SessionController : Controller
{
    // GET /Session/
    public IActionResult Index()
    {
        var viewModel = Session.GetAll();
        return View(viewModel);
    }
    
    // GET /Session/Create
    [HttpGet]
    public IActionResult Create()
    {
        var subjects = SubjectModel.GetAll();
        var model = new DropdownModel
        {
            Options = subjects.Select(subject => new SelectListItem
            {
                Value = subject.Id.ToString(),
                Text = subject.Name,
            }).ToList()
        };
        return View(model);
    }

    // Post: Session/Create/
    [HttpPost]
    public async Task<IActionResult> Create(DropdownModel model)
    {
        var selectedValue = model.SelectedValue;
        var subject = SubjectModel.GetOneByID(selectedValue);
        if (subject != null)
        {
            var newSession = new Session(subject.Id, 1, 30);
            Session.AddOne(newSession);
            return RedirectToAction("Index", "Session");
        }
        return View(model);
    }
}