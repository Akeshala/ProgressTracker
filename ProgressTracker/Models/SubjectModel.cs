using System.ComponentModel.DataAnnotations;

namespace ProgressTracker.Models;

public class SubjectModel
{
    private static Dictionary<int, SubjectModel> _subjects = new Dictionary<int, SubjectModel>{
        { 1, new SubjectModel { Id = 1, Name = "Science", Credits = 3 } },
        { 2, new SubjectModel { Id = 2, Name = "Mathematics", Credits = 4 } },
        { 3, new SubjectModel { Id = 3, Name = "English", Credits = 2 } },
        { 4, new SubjectModel { Id = 4, Name = "History", Credits = 2 } },
        { 5, new SubjectModel { Id = 5, Name = "Religion", Credits = 2 } },
        { 6, new SubjectModel { Id = 6, Name = "Tamil", Credits = 2 } },
    };
    
    public int Id { get; set; }
    
    [StringLength(60, MinimumLength = 3)]
    [Required]
    public string? Name { get; set; }
    
    [Range(1, 10)]
    public int Credits { get; set; }
    
    public static SubjectModel? GetOneByID(int id)
    {
        _subjects.TryGetValue(id, out SubjectModel? subject);
        return subject;
    }

    public static SubjectModel[] GetAll()
    {
        return _subjects.Values.ToArray();
    }
    
    public static void AddSubject(SubjectModel subjectModel)
    {
        _subjects[subjectModel.Id] = subjectModel;
    }
    
    public static bool RemoveSubject(int id)
    {
        return _subjects.Remove(id);
    }
}