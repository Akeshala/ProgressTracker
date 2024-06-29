using System.ComponentModel.DataAnnotations;

namespace ProgressTracker.Models;

public class SubjectModel
{
    private static Dictionary<int, SubjectModel> _subjects = new Dictionary<int, SubjectModel>();
    private static int _nextId = 0;
    
    public int Id { get; private set; }

    [StringLength(60, MinimumLength = 3)]
    [Required]
    public string? Name { get; set; }

    [Range(1, 10)]
    public int Credits { get; set; }
    
    public SubjectModel(string name, int credits)
    {
        Id = GenerateId();
        Name = name;
        Credits = credits;
    }
    
    private static int GenerateId()
    {
        return _nextId++;
    }

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