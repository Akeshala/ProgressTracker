using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProgressTracker.Models;
using ProgressTracker.Services;
using ProgressTracker.ViewModels.Session;

namespace ProgressTracker.Controllers;

public class SessionController : Controller
{
    private readonly ISubjectService _subjectService;
    private readonly ISessionService _sessionService;
    public SessionController(ISubjectService subjectService, ISessionService sessionService)
    {
        _subjectService = subjectService;
        _sessionService = sessionService;
    }
    
    // GET /Session/
    public IActionResult Index()
    {
        var sessions = _sessionService.GetAll();
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
        var session = _sessionService.GetOneById(id);
        if (session == null)
        {
            return RedirectToAction("Index");
        }
        
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
    public Task<IActionResult> Edit(int id, SessionViewEditModel? viewModel)
    {
        var session = _sessionService.GetOneById(id);
        if (viewModel == null || session == null)
        {
            return Task.FromResult<IActionResult>(NotFound());
        }
        
        var hours = viewModel.Hours;
        var minutes = viewModel.Minutes;
        session.Time = new TimeSpan(hours, minutes, 0);
        session.SubjectId = viewModel.SubjectSelectedValue;
        return Task.FromResult<IActionResult>(RedirectToAction("Index", "DailyRecord"));
    }
}