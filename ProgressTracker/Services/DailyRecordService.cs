using System.Runtime.InteropServices.JavaScript;
using ProgressTracker.Models;

namespace ProgressTracker.Services
{
    public class DailyRecordService : IDailyRecordService
    {
        private static readonly Dictionary<int, DailyRecordModel?> _dailyRecords = new Dictionary<int, DailyRecordModel?>();

        private static int _nextId = 0;
        private static readonly object _lock = new object();
        private static bool _initialized = false;

        public DailyRecordService()
        {
            InitializeDailyRecords();
        }

        private void InitializeDailyRecords()
        {
            if (_initialized) return;

            lock (_lock)
            {
                if (_initialized) return;

                var record1 = new DailyRecordModel(2024, 6, 1);
                record1.AddOneSessionId(1);
                record1.AddOneSessionId(2);
                record1.SetBreak(0, 45);
                AddOne(record1);

                var record2 = new DailyRecordModel(2024, 6, 2);
                record2.AddOneSessionId(3);
                record2.AddOneSessionId(4);
                record2.SetBreak(0, 30);
                AddOne(record2);

                var record3 = new DailyRecordModel(2024, 6, 3);
                record3.AddOneSessionId(5);
                record3.AddOneSessionId(6);
                record3.AddOneSessionId(7);
                record3.SetBreak(0, 10);
                AddOne(record3);

                _initialized = true;
            }
        }

        public IEnumerable<DailyRecordModel?> GetAll()
        {
            return _dailyRecords.Values.ToArray();
        }

        public DailyRecordModel? GetOneById(int id)
        {
            if (_dailyRecords.TryGetValue(id, out DailyRecordModel? dailyRecord))
            {
                return dailyRecord;
            }
            return null;
        }

        public void AddOne(DailyRecordModel? dailyRecord)
        {
            if (dailyRecord != null)
            {
                if (dailyRecord.Id == 0)
                {
                    dailyRecord.Id = GenerateUniqueId();
                    _dailyRecords[dailyRecord.Id] = dailyRecord;
                }
                else
                {
                    _dailyRecords[dailyRecord.Id] = dailyRecord;
                }
            }
        }

        private int GenerateUniqueId()
        {
            lock (_lock)
            {
                return ++_nextId;
            }
        }

        public void RemoveOne(int id)
        {
            lock (_lock)
            {
                _dailyRecords.Remove(id);
            }
        }

        public IEnumerable<DailyRecordModel?> GetAllInRange(DateTime startDate, DateTime endDate)
        {
            var dailyRecords = _dailyRecords.Values
                .Where(record => record?.Date >= startDate && record?.Date <= endDate)
                .ToArray();
            return dailyRecords;
        }
    }
}
