using System.ComponentModel.DataAnnotations;

namespace ProgressTracker.ViewModels.SubjectInfo;

public class Subject
{
    private static Dictionary<int, Subject> _subjects = new Dictionary<int, Subject>{
        { 1, new Subject { Id = 1, Name = "Science", Credits = 3 } },
        { 2, new Subject { Id = 2, Name = "Mathematics", Credits = 4 } },
        { 3, new Subject { Id = 3, Name = "English", Credits = 2 } },
        { 4, new Subject { Id = 4, Name = "History", Credits = 2 } },
        { 5, new Subject { Id = 5, Name = "Religion", Credits = 2 } },
        { 6, new Subject { Id = 6, Name = "Tamil", Credits = 2 } },
    };
    
    public int Id { get; set; }
    
    [StringLength(60, MinimumLength = 3)]
    [Required]
    public string? Name { get; set; }
    
    [Range(1, 10)]
    public int Credits { get; set; }
    
    public static Subject? GetOneByID(int id)
    {
        _subjects.TryGetValue(id, out Subject? subject);
        return subject;
    }

    public static Subject[] GetAll()
    {
        return _subjects.Values.ToArray();
    }
    
    public static void AddSubject(Subject subject)
    {
        _subjects[subject.Id] = subject;
    }
    
    public static bool RemoveSubject(int id)
    {
        return _subjects.Remove(id);
    }
}