using ProgressTracker.Models;

namespace ProgressTracker.Services
{
    public interface IDailyRecordService
    {
        List<DailyRecordModel> GetAll();
        Task<DailyRecordModel?> GetOneById(int id);
        Task<bool> RemoveOne(int id);
        Task AddOne(DailyRecordModel dailyRecord);
        List<DailyRecordModel> GetAllInRange(DateTime startDate, DateTime endDate);
        Task<TimeSpan> GetLearned(DailyRecordModel dailyRecord);
        Task<TimeSpan> GetUntracked(DailyRecordModel dailyRecord);
        Task<TimeSpan> GetTracked(DailyRecordModel dailyRecord);
        Task<TimeSpan> GetRecorded(DailyRecordModel dailyRecord);
        List<DailyRecordModel> GetAllByUser(int userId);
        List<DailyRecordModel> GetAllInRangeByUser(DateTime startDate, DateTime endDate, int userId);
    }
}