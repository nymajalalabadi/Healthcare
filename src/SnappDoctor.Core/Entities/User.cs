using Microsoft.AspNetCore.Identity;

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
    
    // Navigation properties
    public virtual ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
    public virtual ICollection<OtpCode> OtpCodes { get; set; } = new List<OtpCode>();
} 