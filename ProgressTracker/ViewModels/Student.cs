namespace ProgressTracker.ViewModels;

public class Student
{
    private static Student? _instance;
    
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Email { get; set; }
    public int? Age { get; set; }
    
    private Student()
    {
        Id = 0;
        Name = "Akeshala";
        Email = "chavinduakeshala@gmail.com";
        Age = 27;
    }
    
    public static Student Instance()
    {
        if (_instance == null)
        {
            _instance = new Student();
        }
        return _instance;
    }
}