using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressTracker.Models
{
    public class DailyRecordModel
    {
        public static readonly int DailyTarget = 8;
        
        [Key]
        public int Id { get; set; }
        
        // [Required]
        public int UserId { get; set; }
        
        // [Required]
        public DateTime Date { get; set; }
        
        public long TargetTicks { get; set; }
        public long BreakTicks { get; set; }
        
        [NotMapped]
        public TimeSpan Target
        {
            get => TimeSpan.FromTicks(TargetTicks);
            set => TargetTicks = value.Ticks;
        }

        [NotMapped]
        public TimeSpan Break
        {
            get => TimeSpan.FromTicks(BreakTicks);
            set => BreakTicks = value.Ticks;
        }
        
        public string SessionIdsString { get; set; } = string.Empty;

        [NotMapped]
        public List<int> SessionIds
        {
            get => string.IsNullOrEmpty(SessionIdsString) 
                ? new List<int>() 
                : SessionIdsString.Split(',').Select(int.Parse).ToList();
            set => SessionIdsString = string.Join(",", value);
        }
        
        public DailyRecordModel(int userId, long targetTicks, long breakTicks, string sessionIds, DateTime date)
        {
            UserId = userId;
            TargetTicks = targetTicks;
            BreakTicks = breakTicks;
            SessionIdsString = sessionIds;
            Date = date;
        }
        
        public DailyRecordModel()
        {
            Date = DateTime.Now;
            Target = new TimeSpan(DailyTarget, 0, 0);
            Break = TimeSpan.Zero;
            SessionIds = [];
        }
        
        public DailyRecordModel(int year, int month, int day)
        {
            Date = new DateTime(year, month, day);
            Target = new TimeSpan(DailyTarget, 0, 0);
            Break = TimeSpan.Zero;
            SessionIds = [];
        }

        public void AddOneSessionId(int sessionId)
        {
            if (string.IsNullOrEmpty(SessionIdsString))
            {
                SessionIdsString = sessionId.ToString();
            }
            else
            {
                SessionIdsString += "," + sessionId;
            }
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