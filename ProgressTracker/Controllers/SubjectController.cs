using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProgressTracker.Services;
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
        var viewModel = await _subjectService.GetAllForUser();
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
        await _subjectService.RemoveOne(id);
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
        if (ModelState.IsValid)
        {
            await _subjectService.AddOne(viewModel);
            return RedirectToAction("Index", "Subject");
        }
        
        
        _logger.LogWarning($"Incomplete subject details to add the subject.");
        return View(viewModel);
    }
}