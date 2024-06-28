namespace ProgressTracker.ViewModels.Profile;

public class UniversityViewModal
{
    private static UniversityViewModal? _instance;
    public int Id { get; set; }
    public string Name { get; set; }
    public string CourseName { get; set; }
    public string? Location { get; set; }
    
    private UniversityViewModal()
    {
        Id = 0;
        Name = "University of Moratuwa";
        CourseName = "Electronic and Telecommunication Engineering";
        Location = "Moratuwa";
    }
    
    public static UniversityViewModal Instance()
    {
        if (_instance == null)
        {
            _instance = new UniversityViewModal();
        }
        return _instance;
    }
}