namespace ProgressTracker.Models
{
    public class SessionModel
    {
        public int SubjectId { get; set; }
        public int Id { get; set; }
        public TimeSpan Time { get; set; }

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