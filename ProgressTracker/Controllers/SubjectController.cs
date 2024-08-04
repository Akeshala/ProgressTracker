using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProgressTracker.Services;
using ProgressTracker.ViewModels;
using ProgressTracker.ViewModels.Subject;

namespace ProgressTracker.Controllers;

public class SubjectController : Controller
{
    private readonly ISubjectService _subjectService;
    private readonly ILogger<SubjectController> _logger;

    public SubjectController(ISubjectService subjectService, ILogger<SubjectController> logger)
    {
        _logger = logger;
        _subjectService = subjectService;
    }

    // GET /Subject/
    public async Task<IActionResult> Index()
    {
        // Extract user ID from cookies
        string? userId = HttpContext.Request.Cookies["userId"];
        if (userId == null)
        {
            return Unauthorized();
        }
        
        var viewModel = await _subjectService.GetAllForUser(int.Parse(userId));
        return View(viewModel);
    }

    // Get: Subject/Delete/{id}
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            _logger.LogWarning($"ID not available to delete.");
            return NotFound();
        }

        return await DeleteConfirmed(id.Value);
    }

    // Post: Subject/DeleteConfirmed/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        // Extract user ID from cookies
        string? userId = HttpContext.Request.Cookies["userId"];
        if (userId == null)
        {
            return Unauthorized();
        }
        
        await _subjectService.RemoveOne(id, int.Parse(userId));
        return RedirectToAction("Index", "Subject");
    }

    // GET /Subject/Add
    [HttpGet]
    public async Task<IActionResult> Add()
    {
        var model = new SubjectAddModel { };
        var subjects = await _subjectService.GetAll();
        var selectListItems = subjects.Select(s => new SelectListItem
        {
            Value = s.Id.ToString(),
            Text = s.Name
        }).ToList();
        ViewData["SubjectOptions"] = selectListItems;
        return View(model);
    }

    // Post: Subject/Add/
    [HttpPost]
    public async Task<IActionResult> Add(SubjectAddModel viewModel)
    {
        // Extract user ID from cookies
        string? userId = HttpContext.Request.Cookies["userId"];
        if (userId == null)
        {
            return Unauthorized();
        }
        
        if (ModelState.IsValid)
        {
            viewModel.UserId = int.Parse(userId);
            await _subjectService.AddOne(viewModel);
            return RedirectToAction("Index", "Subject");
        }
        
        
        _logger.LogWarning($"Incomplete subject details to add the subject.");
        return View(viewModel);
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}