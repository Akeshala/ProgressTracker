using ProgressTracker.Models;

namespace ProgressTracker.Services
{
    public interface IDailyRecordService
    {
        List<DailyRecordModel> GetAll();
        DailyRecordModel? GetOneById(int id);
        void RemoveOne(int id);
        void AddOne(DailyRecordModel dailyRecord);
        List<DailyRecordModel> GetAllInRange(DateTime startDate, DateTime endDate);
        Task<TimeSpan> GetLearned(DailyRecordModel dailyRecord);
        Task<TimeSpan> GetUntracked(DailyRecordModel dailyRecord);
        Task<TimeSpan> GetTracked(DailyRecordModel dailyRecord);
        Task<TimeSpan> GetRecorded(DailyRecordModel dailyRecord);
    }
}