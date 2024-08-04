using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProgressTracker.Models;
using ProgressTracker.Services;
using ProgressTracker.ViewModels.DailyRecord;
using ProgressTracker.ViewModels.Session;

namespace ProgressTracker.Controllers;

public class DailyRecordController : Controller
{
    private readonly ISubjectService _subjectService;
    private readonly IDailyRecordService _dailyRecordService;
    private readonly ISessionService _sessionService;

    public DailyRecordController(ISubjectService subjectService, IDailyRecordService dailyRecordService,
        ISessionService sessionService)
    {
        _subjectService = subjectService;
        _dailyRecordService = dailyRecordService;
        _sessionService = sessionService;
    }

    // GET /DailyRecord/
    public IActionResult Index()
    {
        var dailyRecords = _dailyRecordService.GetAll();
        var viewModel = dailyRecords.Select(dailyRecord => new DailyRecordViewModel
        {
            DailyRecordModel = dailyRecord,
            Learned = _dailyRecordService.GetLearned(dailyRecord),
        }).ToList();
        return View(viewModel);
    }

    // GET /DailyRecord/Create
    public async Task<IActionResult> Create()
    {
        var subjects = await _subjectService.GetAllForUser();
        var model = new AddDailyRecordViewModel
        {
            SubjectOptions = subjects.Select(subject => new SelectListItem
            {
                Value = subject.Id.ToString(),
                Text = subject.Name,
            }).ToList(),
            DailyRecord = new DailyRecordModel(),
        };
        return View(model);
    }

    // Get: DailyRecord/Delete/{id}
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        return await DeleteConfirmed(id.Value);
    }

    // Post: DailyRecord/Delete/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> DeleteConfirmed(int id)
    {
        _dailyRecordService.RemoveOne(id);
        TempData["Message"] = "Daily Record deleted successfully.";
        return Task.FromResult<IActionResult>(RedirectToAction("Index", "DailyRecord"));
    }

    // Post: DailyRecord/Create/
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AddDailyRecordViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return NotFound();
        }

        try
        {
            var dailyRecord = viewModel.DailyRecord;

            // validate and set break time
            var breakHours = viewModel.BreakHours ?? 0;
            var breakMinutes = viewModel.BreakMinutes ?? 0;
            if (!(breakHours == 0 && breakMinutes == 0))
            {
                var breakTimeSpan = new TimeSpan(breakHours!, breakMinutes!, 0);
                if (breakTimeSpan > dailyRecord.GetTarget())
                {
                    TempData["Error"] = "Break can not be larger than Target!";
                    var subjects = await _subjectService.GetAllForUser();
                    viewModel.SubjectOptions = subjects.Select(subject => new SelectListItem
                    {
                        Value = subject.Id.ToString(),
                        Text = subject.Name,
                    }).ToList();
                    return View(viewModel);
                }

                dailyRecord.Break = breakTimeSpan;
            }

            // add a session if available
            var subjectSelectedValue = viewModel.SubjectSelectedValue;
            if (subjectSelectedValue != null)
            {
                var subjectId = (int)subjectSelectedValue;
                var subject = _subjectService.GetOneById(subjectId);
                var subjectHours = viewModel.SubjectHours ?? 0;
                var subjectMinutes = viewModel.SubjectMinutes ?? 0;
                if (subject != null && !(subjectHours == 0 && subjectMinutes == 0))
                {
                    var newSession = new SessionModel(subject.Id, subjectHours, subjectMinutes);
                    if (newSession.Time + dailyRecord.Break > dailyRecord.Target)
                    {
                        TempData["Error"] = "Break and Session durations can not be larger than daily target!";
                        var subjects = await _subjectService.GetAllForUser();
                        viewModel.SubjectOptions = subjects.Select(sbj => new SelectListItem
                        {
                            Value = sbj.Id.ToString(),
                            Text = sbj.Name,
                        }).ToList();
                        return View(viewModel);
                    }

                    _sessionService.AddOne(newSession);
                    dailyRecord.AddOneSessionId(newSession.Id);
                }
            }

            _dailyRecordService.AddOne(dailyRecord);
            TempData["Message"] = "Record created successfully!";
        }
        catch (Exception ex)
        {
            TempData["Message"] = ex.Message;
        }

        return RedirectToAction("Index", "DailyRecord");
    }

    // GET /DailyRecord/Edit/{id}
    public async Task<IActionResult> Edit(int id)
    {
        var subjects = await _subjectService.GetAllForUser();
        var dailyRecord = _dailyRecordService.GetOneById(id);

        if (dailyRecord == null)
        {
            return NotFound();
        }

        var breakTime = dailyRecord.GetBreak();
        var sessionIds = dailyRecord.SessionIds;
        var sessionsTasks = _sessionService.GetMultiByIds(sessionIds).Select(async session =>
        {
            var subject = await _subjectService.GetOneById(session.SubjectId);
            return new SessionViewModel
            {
                SubjectName = subject?.Name ?? "Unknown",
                Time = session.Time,
                Id = session.Id,
            };
        });
        var sessions = await Task.WhenAll(sessionsTasks);
        var model = new EditDailyRecordViewModel
        {
            DailyRecord = dailyRecord,
            Learned = _dailyRecordService.GetLearned(dailyRecord),
            BreakHours = breakTime.Hours,
            BreakMinutes = breakTime.Minutes,
            SubjectOptions = subjects.Select(subject => new SelectListItem
            {
                Value = subject.Id.ToString(),
                Text = subject.Name,
            }).ToList(),
            Sessions = sessions,
        };
        return View(model);
    }

    // POST: DailyRecord/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditDailyRecordViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return await Task.FromResult<IActionResult>(NotFound());
        }

        var newDailyRecord = viewModel.DailyRecord;
        var dailyRecord = _dailyRecordService.GetOneById(id);
        if (dailyRecord == null)
        {
            return await Task.FromResult<IActionResult>(NotFound());
        }

        // set new date
        dailyRecord.Date = newDailyRecord.Date;

        // edit breaks
        var breakHours = viewModel.BreakHours ?? 0;
        var breakMinutes = viewModel.BreakMinutes ?? 0;
        if (!(breakHours == 0 && breakMinutes == 0))
        {
            var breakTimeSpan = new TimeSpan(breakHours, breakMinutes, 0);
            if (breakTimeSpan + _dailyRecordService.GetLearned(dailyRecord) > dailyRecord.GetTarget())
            {
                ViewData["Error"] = "Adding this Break exceeds daily Target";
                var subjects = await _subjectService.GetAllForUser();
                viewModel.SubjectOptions = subjects.Select(sbj => new SelectListItem
                {
                    Value = sbj.Id.ToString(),
                    Text = sbj.Name,
                }).ToList();
                var sessionViewModelTasks = _sessionService.GetMultiByIds(dailyRecord.SessionIds).Select(
                    async session =>
                    {
                        var subject = await _subjectService.GetOneById(session.SubjectId);
                        return new SessionViewModel
                        {
                            SubjectName = subject?.Name ?? "Unknown",
                            Time = session.Time,
                            Id = session.Id,
                        };
                    });
                viewModel.Sessions = await Task.WhenAll(sessionViewModelTasks);
                viewModel.Learned = _dailyRecordService.GetLearned(dailyRecord);
                viewModel.BreakHours = dailyRecord.Break.Hours;
                viewModel.BreakMinutes = dailyRecord.Break.Minutes;
                return await Task.FromResult<IActionResult>(View(viewModel));
            }

            dailyRecord.Break = breakTimeSpan;
        }

        // add session if available
        var subjectSelectedValue = viewModel.SubjectSelectedValue ?? 0;
        var subject = await _subjectService.GetOneById(subjectSelectedValue);
        if (subject != null)
        {
            var subjectHours = viewModel.SubjectHours ?? 0;
            var subjectMinutes = viewModel.SubjectMinutes ?? 0;
            if (!(subjectHours == 0 && subjectMinutes == 0))
            {
                var sessionTimeSpan = new TimeSpan(subjectHours, subjectMinutes, 0);
                if (sessionTimeSpan + _dailyRecordService.GetRecorded(dailyRecord) > dailyRecord.GetTarget())
                {
                    ViewData["Error"] = "Adding this Session exceeds daily Target";
                    var subjects = await _subjectService.GetAllForUser();
                    viewModel.SubjectOptions = subjects.Select(sbj => new SelectListItem
                    {
                        Value = sbj.Id.ToString(),
                        Text = sbj.Name,
                    }).ToList();
                    var sessionViewModelTasks = _sessionService.GetMultiByIds(dailyRecord.SessionIds).Select(
                        async session =>
                        {
                            var sessionSubject = await _subjectService.GetOneById(session.SubjectId);
                            return new SessionViewModel
                            {
                                SubjectName = sessionSubject?.Name ?? "Unknown",
                                Time = session.Time,
                                Id = session.Id,
                            };
                        }).ToList();
                    viewModel.Sessions = await Task.WhenAll(sessionViewModelTasks);
                    viewModel.Learned = _dailyRecordService.GetLearned(dailyRecord);
                    viewModel.BreakHours = dailyRecord.Break.Hours;
                    viewModel.BreakMinutes = dailyRecord.Break.Minutes;
                    return await Task.FromResult<IActionResult>(View(viewModel));
                }

                var newSession = new SessionModel(subject.Id, subjectHours, subjectMinutes);
                _sessionService.AddOne(newSession);
                dailyRecord.AddOneSessionId(newSession.Id);
            }
        }

        return await Task.FromResult<IActionResult>(RedirectToAction("Edit", "DailyRecord", dailyRecord.Id));
    }
}