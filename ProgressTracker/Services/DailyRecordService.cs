using ProgressTracker.Data;
using ProgressTracker.Models;

namespace ProgressTracker.Services
{
    public class DailyRecordService : IDailyRecordService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DailyRecordService> _logger;
        private readonly ISessionService _sessionService;

        public DailyRecordService(ISessionService sessionService, ILogger<DailyRecordService> logger, AppDbContext context)
        {
            _sessionService = sessionService;
            _logger = logger;
            _context = context;
        }

        public List<DailyRecordModel> GetAll()
        {
            try
            {
                return _context.DailyRecords.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all daily records.");
                throw;
            }
        }

        public List<DailyRecordModel> GetAllByUser(int userId)
        {
            return GetAll().Where(record => record.UserId == userId).ToList();
        }

        public async Task<DailyRecordModel?> GetOneById(int id)
        {
            try
            {
                return await _context.DailyRecords.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving daily record with ID {id}.");
                throw;
            }
        }

        public async Task AddOne(DailyRecordModel? dailyRecord)
        {
            if (dailyRecord != null)
            {
                try
                {
                    var existingRecord = GetRecordForDate(dailyRecord.Date);
                    if (existingRecord != null)
                    {
                        throw new Exception("Duplicate records found for that day! Edit the existing record.");
                    }

                    await _context.DailyRecords.AddAsync(dailyRecord);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding daily record.");
                    throw;
                }
            }
        }

        public async Task<bool> RemoveOne(int id)
        {
            try
            {
                var dailyRecord = await _context.DailyRecords.FindAsync(id);
                if (dailyRecord == null)
                {
                    return false;
                }

                _context.DailyRecords.Remove(dailyRecord);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing daily record with ID {id}.");
                throw;
            }
        }

        public List<DailyRecordModel> GetAllInRange(DateTime startDate, DateTime endDate)
        {
            return GetAll()
                .Where(record => record.Date >= startDate && record.Date <= endDate)
                .ToList();
        }

        public List<DailyRecordModel> GetAllInRangeByUser(DateTime startDate, DateTime endDate, int userId)
        {
            return GetAll()
                .Where(record => record.Date >= startDate && record.Date <= endDate && record.UserId == userId)
                .ToList();
        }

        public async Task<TimeSpan> GetLearned(DailyRecordModel dailyRecord)
        {
            return await CalculateTotalTime(dailyRecord);
        }

        public async Task<TimeSpan> GetUntracked(DailyRecordModel dailyRecord)
        {
            var learnedTime = await CalculateTotalTime(dailyRecord);
            return dailyRecord.GetTarget() - learnedTime + dailyRecord.Break;
        }

        public async Task<TimeSpan> GetTracked(DailyRecordModel dailyRecord)
        {
            return await CalculateTotalTime(dailyRecord) + dailyRecord.Break;
        }

        public async Task<TimeSpan> GetRecorded(DailyRecordModel dailyRecord)
        {
            return await CalculateTotalTime(dailyRecord) + dailyRecord.Break;
        }

        private async Task<TimeSpan> CalculateTotalTime(DailyRecordModel dailyRecord)
        {
            try
            {
                var sessionIterator = await _sessionService.GetMultiByIds(dailyRecord.SessionIds);
                return sessionIterator.Aggregate(TimeSpan.Zero, (ac, session) => ac + session.Time);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total session time.");
                throw;
            }
        }

        private DailyRecordModel? GetRecordForDate(DateTime date)
        {
            return GetAll().FirstOrDefault(record => record.Date == date);
        }
    }
}
