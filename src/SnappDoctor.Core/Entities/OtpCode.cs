namespace SnappDoctor.Core.Entities;

public class OtpCode
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Purpose { get; set; } = string.Empty; // Registration, Login, etc.
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
} 