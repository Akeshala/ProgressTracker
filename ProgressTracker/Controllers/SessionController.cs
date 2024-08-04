using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProgressTracker.Models;
using ProgressTracker.Services;
using ProgressTracker.ViewModels;
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
    public async Task<IActionResult> Index()
    {
        var sessions = _sessionService.GetAll();
        var viewModelTasks = sessions.Select(async session =>
        {
            var subject = await _subjectService.GetOneById(session.SubjectId);
            return new SessionViewModel
            {
                SubjectName = subject?.Name ?? "Unknown",
                Time = session.Time,
                Id = session.Id,
            };
        });

        var viewModel = await Task.WhenAll(viewModelTasks);

        return View(viewModel);
    }


    // GET /Session/Edit
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        // Extract user ID from cookies
        string? userId = HttpContext.Request.Cookies["userId"];
        if (userId == null)
        {
            return Unauthorized();
        }
        
        var subjects = await _subjectService.GetAllForUser(int.Parse(userId));
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
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}