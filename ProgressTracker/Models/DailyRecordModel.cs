namespace ProgressTracker.Models
{
    public class DailyRecordModel
    {
        public static readonly int DailyTarget = 8;
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Target { get; set; }
        public TimeSpan Break { get; set; }
        private readonly List<int> _sessionIds;
        
        public DailyRecordModel()
        {
            Date = DateTime.Now;
            Target = new TimeSpan(DailyTarget, 0, 0);
            Break = TimeSpan.Zero;
            _sessionIds = [];
        }
        
        public DailyRecordModel(int year, int month, int day)
        {
            Date = new DateTime(year, month, day);
            Target = new TimeSpan(DailyTarget, 0, 0);
            Break = TimeSpan.Zero;
            _sessionIds = [];
        }

        public List<int> SessionIds => _sessionIds;

        public void AddOneSessionId(int sessionId)
        {
            _sessionIds.Add(sessionId);
        }

        public DateTime GetDate()
        {
            return Date;
        }
        
        public void SetDate(DateTime date)
        {
            Date = date;
        }

        public TimeSpan GetTarget()
        {
            return Target;
        }

        public TimeSpan GetBreak()
        {
            return Break;
        }
        
        public void SetBreak(int breakHours, int breakMinutes)
        {
            Break = new TimeSpan(breakHours, breakMinutes, 0);
        }
    }
}