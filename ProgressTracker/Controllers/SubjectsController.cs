using Microsoft.AspNetCore.Mvc;
using ProgressTracker.ViewModels.SubjectInfo;

namespace ProgressTracker.Controllers;

public class SubjectsController : Controller
{
    // GET /Subject/
    public IActionResult Index()
    {
        var viewModel = Subject.GetAll();
        return View(viewModel);
    }
    
    // GET /Subject/Edit/{id}
    public IActionResult Edit(int id)
    {
        var viewModel = Subject.GetOneByID(id);
        return View(viewModel);
    }
    
    // POST: Subject/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Credits")] Subject subject)
    {
        if (id != subject.Id)
        {
            return NotFound();
        }
        Subject.AddSubject(subject);
        return View(subject);
    }
    
    // POST: Subject/Delete/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        Subject.RemoveSubject(id);
        return RedirectToAction("Index", "Subjects");
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
        Subject.RemoveSubject(id);
        return RedirectToAction(nameof(Index));
    }
    
    // GET /Subject/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // Post: Subject/Create/
    [HttpPost]
    public async Task<IActionResult> Create(Subject subject)
    {
        if (ModelState.IsValid)
        {
            Subject.AddSubject(subject);
            return RedirectToAction("Index", "Subjects");
        }
        return View(subject);
    }
}