using Microsoft.AspNetCore.Mvc;
using ProgressTracker.Models;
using ProgressTracker.Services;
using ProgressTracker.ViewModels.Subject;

namespace ProgressTracker.Controllers;

public class SubjectController : Controller
{
    private readonly ISubjectService _subjectService;
    
    public SubjectController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }
    
    // GET /Subject/
    public IActionResult Index()
    {
        var viewModel = _subjectService.GetAll();
        return View(viewModel);
    }
    
    // GET /Subject/Edit/{id}
    public IActionResult Edit(int id)
    {
        var viewModel = _subjectService.GetOneById(id);
        return View(viewModel);
    }
    
    // POST: Subject/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Edit(int id, SubjectModel subjectModel)
    {
        subjectModel.Id = id;
        _subjectService.AddOne(subjectModel);
        return Task.FromResult<IActionResult>(RedirectToAction("Index", "Subject"));
    }
    
    // Get: Subject/Delete/{id}
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        return await DeleteConfirmed(id.Value);
    }
    
    // Post: Subject/DeleteConfirmed/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> DeleteConfirmed(int id)
    {
        _subjectService.RemoveOne(id);
        return Task.FromResult<IActionResult>(RedirectToAction("Index", "Subject"));
    }
    
    // GET /Subject/Create
    [HttpGet]
    public IActionResult Create()
    {
        var model = new SubjectViewModel { };
        return View(model);
    }

    // Post: Subject/Create/
    [HttpPost]
    public Task<IActionResult> Create(SubjectViewModel? viewModel)
    {
        if (ModelState.IsValid && viewModel != null)
        {
            var subjectModel = new SubjectModel(viewModel.Name, viewModel.Credits, viewModel.LearningHours);
            _subjectService.AddOne(subjectModel);
            return Task.FromResult<IActionResult>(RedirectToAction("Index", "Subject"));
        }
        return Task.FromResult<IActionResult>(View(viewModel));
    }
}