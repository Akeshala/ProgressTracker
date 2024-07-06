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
    public DailyRecordController(ISubjectService subjectService, IDailyRecordService dailyRecordService)
    {
        _subjectService = subjectService;
        _dailyRecordService = dailyRecordService;
    }
    
    // GET /DailyRecord/
    public IActionResult Index()
    {
        var dailyRecords = _dailyRecordService.GetAll();
        var viewModel = dailyRecords.Select(dailyRecord => new DailyRecordViewModel
        {
            DailyRecordModel = dailyRecord,
            Learned = dailyRecord.GetLearned(),
        }).ToList();
        return View(viewModel);
    }

    // GET /DailyRecord/Create
    public IActionResult Create()
    {
        var subjects = _subjectService.GetAll();
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

    // Post: DailyRecord/Create/
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(AddDailyRecordViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return NotFound();
        }

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
                var subjects = _subjectService.GetAll();
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
            var subjectId = (int) subjectSelectedValue;
            var subject = _subjectService.GetOneById(subjectId);
            var subjectHours = viewModel.SubjectHours ?? 0;
            var subjectMinutes = viewModel.SubjectMinutes ?? 0;
            if (subject != null && !(subjectHours == 0 && subjectMinutes == 0))
            {
                var newSession = new SessionModel(subject.Id, subjectHours, subjectMinutes);
                if (newSession.Time + dailyRecord.Break > dailyRecord.Target)
                {
                    TempData["Error"] = "Break and Session durations can not be larger than daily target!";
                    var subjects = _subjectService.GetAll();
                    viewModel.SubjectOptions = subjects.Select(sbj => new SelectListItem
                    {
                        Value = sbj.Id.ToString(),
                        Text = sbj.Name,
                    }).ToList();
                    return View(viewModel);
                }

                SessionModel.AddOne(newSession);
                dailyRecord.AddOneSessionId(newSession.Id);
            }
        }

        _dailyRecordService.AddOne(dailyRecord);
        return RedirectToAction("Index", "DailyRecord");
    }

    // GET /DailyRecord/Edit/{id}
    public IActionResult Edit(int id)
    {
        var subjects = _subjectService.GetAll();
        var dailyRecord = _dailyRecordService.GetOneById(id);

        if (dailyRecord == null)
        {
            return NotFound();
        }
        
        var breakTime = dailyRecord.GetBreak();
        var sessionIds = dailyRecord.SessionIds;
        var model = new EditDailyRecordViewModel
        {
            DailyRecord = dailyRecord,
            Learned = dailyRecord.GetLearned(),
            BreakHours = breakTime.Hours,
            BreakMinutes = breakTime.Minutes,
            SubjectOptions = subjects.Select(subject => new SelectListItem
            {
                Value = subject.Id.ToString(),
                Text = subject.Name,
            }).ToList(),
            Sessions = SessionModel.GetMultiByIds(sessionIds).Select(session =>
            {
                var subject = _subjectService.GetOneById(session.SubjectId);
                return new SessionViewModel
                {
                    SubjectName = subject?.Name ?? "Unknown",
                    Time = session.Time,
                    Id = session.Id,
                };
            }).ToList(),
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
            return NotFound();
        }
        
        var newDailyRecord = viewModel.DailyRecord;
        var dailyRecord = _dailyRecordService.GetOneById(id);
        if (dailyRecord == null)
        {
            return NotFound();
        }

        // set new date
        dailyRecord.Date = newDailyRecord.Date;

        // edit breaks
        var breakHours = viewModel.BreakHours ?? 0;
        var breakMinutes = viewModel.BreakMinutes ?? 0;
        if (!(breakHours == 0 && breakMinutes == 0))
        {
            var breakTimeSpan = new TimeSpan(breakHours, breakMinutes, 0);
            if (breakTimeSpan + dailyRecord.GetLearned() > dailyRecord.GetTarget())
            {
                ViewData["Error"] = "Adding this Break exceeds daily Target";
                var subjects = _subjectService.GetAll();
                viewModel.SubjectOptions = subjects.Select(sbj => new SelectListItem
                {
                    Value = sbj.Id.ToString(),
                    Text = sbj.Name,
                }).ToList();
                viewModel.Sessions = SessionModel.GetMultiByIds(dailyRecord.SessionIds).Select(session =>
                {
                    var subject = _subjectService.GetOneById(session.SubjectId);
                    return new SessionViewModel
                    {
                        SubjectName = subject?.Name ?? "Unknown",
                        Time = session.Time,
                        Id = session.Id,
                    };
                }).ToList();
                viewModel.Learned = dailyRecord.GetLearned();
                viewModel.BreakHours = dailyRecord.Break.Hours;
                viewModel.BreakMinutes = dailyRecord.Break.Minutes;
                return View(viewModel);
            }
    
            dailyRecord.Break = breakTimeSpan;
        }
    
        // add session if available
        var subjectSelectedValue = viewModel.SubjectSelectedValue ?? 0;
        var subject = _subjectService.GetOneById(subjectSelectedValue);
        if (subject != null)
        {
            var subjectHours = viewModel.SubjectHours ?? 0;
            var subjectMinutes = viewModel.SubjectMinutes ?? 0;
            if (!(subjectHours == 0 && subjectMinutes == 0))
            {
                var sessionTimeSpan = new TimeSpan(subjectHours, subjectMinutes, 0);
                if (sessionTimeSpan + dailyRecord.GetRecorded() > dailyRecord.GetTarget())
                {
                    ViewData["Error"] = "Adding this Session exceeds daily Target";
                    var subjects = _subjectService.GetAll();
                    viewModel.SubjectOptions = subjects.Select(sbj => new SelectListItem
                    {
                        Value = sbj.Id.ToString(),
                        Text = sbj.Name,
                    }).ToList();
                    viewModel.Sessions = SessionModel.GetMultiByIds(dailyRecord.SessionIds).Select(session =>
                    {
                        var subject = _subjectService.GetOneById(session.SubjectId);
                        return new SessionViewModel
                        {
                            SubjectName = subject?.Name ?? "Unknown",
                            Time = session.Time,
                            Id = session.Id,
                        };
                    }).ToList();
                    viewModel.Learned = dailyRecord.GetLearned();
                    viewModel.BreakHours = dailyRecord.Break.Hours;
                    viewModel.BreakMinutes = dailyRecord.Break.Minutes;
                    return View(viewModel);
                }
                var newSession = new SessionModel(subject.Id, subjectHours, subjectMinutes);
                SessionModel.AddOne(newSession);
                dailyRecord.AddOneSessionId(newSession.Id);
            }
        }
    
        return RedirectToAction("Edit", "DailyRecord", dailyRecord.Id);
    }
}