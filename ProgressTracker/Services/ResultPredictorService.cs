using ProgressTracker.Models;
using ProgressTracker.Utils;

namespace ProgressTracker.Services;

public class ResultPredictorService : IResultPredictorService
{
    private readonly ISessionService _sessionService;
    private readonly ISubjectService _subjectService;

    public ResultPredictorService(ISessionService sessionService, ISubjectService subjectService)
    {
        _sessionService = sessionService;
        _subjectService = subjectService;
    }

    public async
        Task<List<(int subjectId, string subjectName, TimeSpan learned, TimeSpan target, string grade, double level)>>
        GetPredictions(List<DailyRecordModel> dailyRecords)
    {
        var learnedTimeBySubjectId = await GetLearnedTimeBySubjectIds(dailyRecords);
        var subjectReportsTasks = learnedTimeBySubjectId.Select(async kvp =>
        {
            var subject = await _subjectService.GetOneById(kvp.Key);
            var target = new TimeSpan(subject?.LearningHours ?? 0, 0, 0);
            var (grade, studyRatio) = ResultsLib.GetStudyRatio(kvp.Value, target);
            return
            (
                kvp.Key, // SubjectId
                subject?.Name ?? "Unknown Subject",
                kvp.Value, // Learned
                target,
                grade,
                Math.Round(studyRatio * 100, 2)
            );
        }).ToList();
        var subjectReports = await Task.WhenAll(subjectReportsTasks);

        return subjectReports.ToList();
    }

    private async Task<Dictionary<int, TimeSpan>> GetLearnedTimeBySubjectIds(List<DailyRecordModel> dailyRecords)
    {
        if (dailyRecords.Count == 0)
        {
            return new Dictionary<int, TimeSpan>();
        }

        var learnedTimeBySubjectId = new Dictionary<int, TimeSpan>();

        foreach (var dailyRecord in dailyRecords)
        {
            var sessionIds = dailyRecord.SessionIds;
            var sessions = await _sessionService.GetMultiByIds(sessionIds);

            foreach (var session in sessions)
            {
                var subjectId = session.SubjectId;
                var time = session.Time;

                if (!learnedTimeBySubjectId.TryAdd(subjectId, time))
                {
                    learnedTimeBySubjectId[subjectId] += time;
                }
            }
        }

        return learnedTimeBySubjectId;
    }
}