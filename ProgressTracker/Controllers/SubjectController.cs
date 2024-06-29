using Microsoft.AspNetCore.Mvc;
using ProgressTracker.Models;
using ProgressTracker.ViewModels.Subject;

namespace ProgressTracker.Controllers;

public class SubjectController : Controller
{
    // GET /Subject/
    public IActionResult Index()
    {
        var viewModel = SubjectModel.GetAll();
        return View(viewModel);
    }
    
    // GET /Subject/Edit/{id}
    public IActionResult Edit(int id)
    {
        var viewModel = SubjectModel.GetOneByID(id);
        return View(viewModel);
    }
    
    // POST: Subject/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Credits")] SubjectModel subjectModel)
    {
        if (id != subjectModel.Id)
        {
            return NotFound();
        }
        SubjectModel.AddSubject(subjectModel);
        return RedirectToAction("Index", "Subject");
    }
    
    // POST: Subject/Delete/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        SubjectModel.RemoveSubject(id);
        return RedirectToAction("Index", "Subject");
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
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        SubjectModel.RemoveSubject(id);
        return RedirectToAction(nameof(Index));
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
    public async Task<IActionResult> Create(SubjectViewModel? viewModel)
    {
        if (ModelState.IsValid && viewModel != null)
        {
            var subjectModel = new SubjectModel(viewModel.Name, viewModel.Credits);
            SubjectModel.AddSubject(subjectModel);
            return RedirectToAction("Index", "Subject");
        }
        return View(viewModel);
    }
}