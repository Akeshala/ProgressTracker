using ProgressTracker.Models;
using ProgressTracker.ViewModels.WeeklyReport;

namespace ProgressTracker.Services;

public interface IResultPredictorService
{
    
    List<(int subjectId, string subjectName, TimeSpan learned, TimeSpan target, string grade, double level)>
        GetPredictions(List<DailyRecordModel> dailyRecords);
}