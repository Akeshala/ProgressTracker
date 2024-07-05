using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProgressTracker.Models;
using ProgressTracker.Services;
using ProgressTracker.ViewModels.Session;

namespace ProgressTracker.Controllers;

public class SessionController : Controller
{
    private readonly ISubjectService _subjectService;
    public SessionController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }
    
    // GET /Session/
    public IActionResult Index()
    {
        var sessions = SessionModel.GetAll();
            
        var viewModel = sessions.Select(session =>
        {
            var subject = _subjectService.GetOneById(session.SubjectId);
            return new SessionViewModel
            {
                SubjectName = subject?.Name ?? "Unknown",
                Time = session.Time,
                Id = session.Id,
            };
        }).ToList();
        
        return View(viewModel);
    }

    // GET /Session/Edit
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var subjects = _subjectService.GetAll();
        var session = SessionModel.GetOneById(id);
        var model = new SessionViewEditModel
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
        [Bind("SubjectSelectedValue,Hours,Minutes")] SessionViewEditModel? viewModel)
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