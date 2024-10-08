using System.ComponentModel.DataAnnotations;

namespace ProgressTracker.ViewModels.Login;

public class RegisterViewModel
{
    [Required]
    public string FirstName { get; set; }
    
    [Required]
    public string LastName { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [Required]
    public string University { get; set; }
}
