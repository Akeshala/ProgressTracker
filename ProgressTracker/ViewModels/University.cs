namespace ProgressTracker.ViewModels;

public class University
{
    private static University? _instance;
    public int Id { get; set; }
    public string Name { get; set; }
    public string CourseName { get; set; }
    public string? Location { get; set; }
    
    private University()
    {
        Id = 0;
        Name = "University of Moratuwa";
        CourseName = "Electronic and Telecommunication Engineering";
        Location = "Moratuwa";
    }
    
    public static University Instance()
    {
        if (_instance == null)
        {
            _instance = new University();
        }
        return _instance;
    }
}