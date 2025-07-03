using Microsoft.AspNetCore.Identity;
using SnappDoctor.Core.Enums;

namespace SnappDoctor.Core.Entities;

public class User : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; } = true;

    public string? ProfilePictureUrl { get; set; }
    
    public byte[]? ProfilePicture { get; set; }

    public UserType UserType { get; set; } = UserType.RegularUser;
    
    // Navigation properties
    public virtual Doctor? Doctor { get; set; }
    public virtual ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
    public virtual ICollection<OtpCode> OtpCodes { get; set; } = new List<OtpCode>();
} 