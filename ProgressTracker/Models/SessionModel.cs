using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressTracker.Models
{
    public class SessionModel
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int SubjectId { get; set; }
        
        // Store TimeSpan as ticks
        public long TimeTicks { get; set; }
        
        [NotMapped]
        public TimeSpan Time
        {
            get => TimeSpan.FromTicks(TimeTicks);
            set => TimeTicks = value.Ticks;
        }
        
        public SessionModel(int subjectId, long timeTicks)
        {
            SubjectId = subjectId;
            TimeTicks = timeTicks;
        }

        public SessionModel(int subjectId, int hours, int minutes)
        {
            SubjectId = subjectId;
            SetTime(hours, minutes);
        }

        public void SetTime(int hours, int minutes)
        {
            Time = new TimeSpan(hours, minutes, 0);
        }

        public TimeSpan GetTime()
        {
            return Time;
        }
    }
}