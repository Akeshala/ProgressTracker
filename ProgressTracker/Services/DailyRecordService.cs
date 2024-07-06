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

                var record1 = new DailyRecordModel(2024, 7, 1);
                record1.AddOneSessionId(1);
                record1.AddOneSessionId(2);
                record1.SetBreak(0, 45);
                AddOne(record1);

                var record2 = new DailyRecordModel(2024, 7, 2);
                record2.AddOneSessionId(3);
                record2.AddOneSessionId(4);
                record2.SetBreak(0, 30);
                AddOne(record2);

                var record3 = new DailyRecordModel(2024, 7, 3);
                record3.AddOneSessionId(5);
                record3.AddOneSessionId(6);
                record3.AddOneSessionId(7);
                record3.SetBreak(0, 10);
                AddOne(record3);
                
                var record4 = new DailyRecordModel(2024, 7, 4);
                record4.AddOneSessionId(8);
                record4.AddOneSessionId(9);
                record4.SetBreak(0, 20);
                AddOne(record4);

                var record5 = new DailyRecordModel(2024, 7, 5);
                record5.AddOneSessionId(10);
                record5.AddOneSessionId(11);
                record5.SetBreak(0, 15);
                AddOne(record5);

                var record6 = new DailyRecordModel(2024, 7, 6);
                record6.AddOneSessionId(12);
                record6.AddOneSessionId(13);
                record6.SetBreak(0, 25);
                AddOne(record6);
                
                var record7 = new DailyRecordModel(2024, 7, 7);
                record7.AddOneSessionId(14);
                record7.AddOneSessionId(15);
                record7.SetBreak(0, 27);
                AddOne(record7);
                
                var record8 = new DailyRecordModel(2024, 7, 8);
                record8.AddOneSessionId(14);
                record8.AddOneSessionId(15);
                record8.SetBreak(0, 29);
                AddOne(record8);
                
                var record9 = new DailyRecordModel(2024, 7, 9);
                record9.AddOneSessionId(14);
                record9.AddOneSessionId(15);
                record9.SetBreak(0, 23);
                AddOne(record9);
                
                var record10 = new DailyRecordModel(2024, 7, 10);
                record10.AddOneSessionId(16);
                record10.AddOneSessionId(17);
                record10.AddOneSessionId(18);
                record10.SetBreak(0, 20);
                AddOne(record10);
                
                var record11 = new DailyRecordModel(2024, 7, 11);
                record11.AddOneSessionId(19);
                record11.AddOneSessionId(20);
                record11.AddOneSessionId(21);
                record11.AddOneSessionId(22);
                record11.SetBreak(0, 30);
                AddOne(record11);        
                
                var record12 = new DailyRecordModel(2024, 7, 12);
                record12.AddOneSessionId(23);
                record12.AddOneSessionId(24);
                record12.AddOneSessionId(25);
                record12.SetBreak(0, 10);
                AddOne(record12);

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
                var existingRecord = GetRecordForDate(dailyRecord.Date);
                if (existingRecord != null)
                {
                    throw new Exception("Duplicate Records found for that day! Edit the existing record.");
                }
                
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
        
        private DailyRecordModel? GetRecordForDate(DateTime date)
        {
            var dailyRecord = _dailyRecords.Values
                .FirstOrDefault(record => record?.Date == date);
            return dailyRecord;
        }

    }
}
