using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProgressTracker.Models;
using ProgressTracker.ViewModels.DailyRecord;
using System.Collections.Generic;
using System.Linq;
using ProgressTracker.ViewModels.Session;

namespace ProgressTracker.Controllers;

public class DailyRecordController : Controller
{
    // GET /DailyRecord/
    public IActionResult Index()
    {
        var dailyRecords = DailyRecordModel.GetAll();
        var viewModel = dailyRecords.Select(dailyRecord => new DailyRecordViewModel
        {
            Id = dailyRecord.Id,
            Date = dailyRecord.Date,
            Target = dailyRecord.Target,
            Break = dailyRecord.Break,
            Covered = SessionModel.GetMultiByIds(dailyRecord.SessionIds)
                .Aggregate(TimeSpan.Zero, (ac, session) => ac + session.Time),
        }).ToList();
        return View(viewModel);
    }

    // GET /DailyRecord/Create
    [HttpGet]
    public IActionResult Create()
    {
        var subjects = SubjectModel.GetAll();
        var model = new AddDailyRecordViewModel
        {
            DailyTarget = DailyRecordModel.DailyTarget,
            SubjectOptions = subjects.Select(subject => new SelectListItem
            {
                Value = subject.Id.ToString(),
                Text = subject.Name,
            }).ToList(),
        };
        return View(model);
    }

    // Post: DailyRecord/Create/
    [HttpPost]
    public async Task<IActionResult> Create(AddDailyRecordViewModel viewModel)
    {
        var year = viewModel.Year;
        var month = viewModel.Month;
        var day = viewModel.Day;

        // early return if date is not selected
        if (year == 0 || month == 0 || day == 0) return View(viewModel);

        // create record with date
        var dailyRecord = new DailyRecordModel(year, month, day);

        // add breaks
        var breakHours = viewModel.BreakHours;
        var breakMinutes = viewModel.BreakMinutes;
        if (!(breakHours == 0 && breakMinutes == 0))
        {
            dailyRecord.Break = new TimeSpan(breakHours, breakMinutes, 0);
        }

        // add session if available
        var subjectSelectedValue = viewModel.SubjectSelectedValue;
        var subject = SubjectModel.GetOneByID(subjectSelectedValue);
        var subjectHours = viewModel.SubjectHours;
        var subjectMinutes = viewModel.SubjectMinutes;
        if (subject != null && !(subjectHours == 0 && subjectMinutes == 0))
        {
            var newSession = new SessionModel(subject.Id, subjectHours, subjectMinutes);
            SessionModel.AddOne(newSession);
            dailyRecord.AddOneSessionId(newSession.Id);
        }

        DailyRecordModel.AddOne(dailyRecord);
        return RedirectToAction("Edit", "DailyRecord", new { id = dailyRecord.Id });
    }

    // GET /DailyRecord/Edit/{id}
    public IActionResult Edit(int id)
    {
        var subjects = SubjectModel.GetAll();
        var dailyRecord = DailyRecordModel.GetOneById(id);

        var date = dailyRecord.GetDate();
        var breakTime = dailyRecord.GetBreak();
        var targetTime = dailyRecord.GetTarget();
        var sessionIds = dailyRecord.SessionIds;
        var model = new EditDailyRecordViewModel
        {
            DailyTarget = targetTime.Hours,
            Year = date.Year,
            Month = date.Month,
            Day = date.Day,
            BreakHours = breakTime.Hours,
            BreakMinutes = breakTime.Minutes,
            SubjectOptions = subjects.Select(subject => new SelectListItem
            {
                Value = subject.Id.ToString(),
                Text = subject.Name,
            }).ToList(),
            Sessions = SessionModel.GetMultiByIds(sessionIds).Select(session => new SessionViewModel
            {
                SubjectName = SubjectModel.GetOneByID(session.SubjectId).Name,
                Time = session.Time,
                Id = session.Id,
            }).ToList(),
            Covered = SessionModel.GetMultiByIds(dailyRecord.SessionIds)
                .Aggregate(TimeSpan.Zero, (ac, session) => ac + session.Time),
        };
        return View(model);
    }

    // POST: DailyRecord/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("DailyTarget,Year,Month,Day,BreakHours,BreakMinutes,SubjectSelectedValue,SubjectHours,SubjectMinutes")]
        EditDailyRecordViewModel? viewModel
    )
    {
        var dailyRecord = DailyRecordModel.GetOneById(id);
        if (viewModel == null || dailyRecord == null)
        {
            return NotFound();
        }

        // edit breaks
        dailyRecord.Break = new TimeSpan(viewModel.BreakHours, viewModel.BreakMinutes, 0);
        dailyRecord.Date = new DateTime(viewModel.Year, viewModel.Month, viewModel.Day);
        dailyRecord.Date = new DateTime(viewModel.Year, viewModel.Month, viewModel.Day);

        // add session if available
        var subjectSelectedValue = viewModel.SubjectSelectedValue;
        var subject = SubjectModel.GetOneByID(subjectSelectedValue);
        var subjectHours = viewModel.SubjectHours;
        var subjectMinutes = viewModel.SubjectMinutes;
        if (subject != null && !(subjectHours == 0 && subjectMinutes == 0))
        {
            var newSession = new SessionModel(subject.Id, subjectHours, subjectMinutes);
            SessionModel.AddOne(newSession);
            dailyRecord.AddOneSessionId(newSession.Id);
        }

        return RedirectToAction("Edit", "DailyRecord", id);
    }
}