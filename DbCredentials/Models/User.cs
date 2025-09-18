using System.ComponentModel.DataAnnotations;
namespace DbCredentials.Models;
public class User
{
    [Key]
    public int id { get; set; }

    [Required(ErrorMessage = "First name is required")]
    [MaxLength(50)]
    public string firstName { get; set; }

    [MaxLength(50)]
    public string middleName { get; set; }

    [MaxLength(50)]
    public string lastName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string email { get; set; }

    [Required(ErrorMessage = "Phone Number is required")]
    [Phone]
    public string phoneNumber { get; set; }

    [DataType(DataType.Date)]
    public DateTime? dob { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string password { get; set; }

    // New fields
    public bool isDeleted { get; set; } = false;  // default false

    public string createdBy { get; set; } // could store email or admin id

    public DateTime createdDate { get; set; } = DateTime.UtcNow;  // default to now
}
