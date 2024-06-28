namespace ProgressTracker.ViewModels.Profile;

public class StudentViewModal
{
    private static StudentViewModal? _instance;
    
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Email { get; set; }
    public int? Age { get; set; }
    
    private StudentViewModal()
    {
        Id = 0;
        Name = "Akeshala";
        Email = "chavinduakeshala@gmail.com";
        Age = 27;
    }
    
    public static StudentViewModal Instance()
    {
        if (_instance == null)
        {
            _instance = new StudentViewModal();
        }
        return _instance;
    }
}