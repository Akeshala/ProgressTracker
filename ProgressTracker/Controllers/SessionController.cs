using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProgressTracker.Models;
using ProgressTracker.ViewModels.Session;

namespace ProgressTracker.Controllers;

public class SessionController : Controller
{
    // GET /Session/
    public IActionResult Index()
    {
        var viewModel = SessionModel.GetAll();
        return View(viewModel);
    }

    // GET /Session/Edit
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var subjects = SubjectModel.GetAll();
        var session = SessionModel.GetOneById(id);
        var model = new SessionViewModel
        {
            SubjectOptions = subjects.Select(subject => new SelectListItem
            {
                Value = subject.Id.ToString(),
                Text = subject.Name,
            }).ToList(),
            Hours = session.GetTime().Hours,
            Minutes = session.GetTime().Minutes,
            SubjectSelectedValue = session.SubjectId,
        };
        return View(model);
    }

    // POST: Subject/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("SubjectSelectedValue,Hours,Minutes")] SessionViewModel? viewModel)
    {
        var session = SessionModel.GetOneById(id);
        if (viewModel == null || session == null)
        {
            return NotFound();
        }
        
        var hours = viewModel.Hours;
        var minutes = viewModel.Minutes;
        session.Time = new TimeSpan(hours, minutes, 0);
        session.SubjectId = viewModel.SubjectSelectedValue;
        return RedirectToAction("Index", "DailyRecord");
    }
}