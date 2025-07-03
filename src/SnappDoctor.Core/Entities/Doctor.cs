namespace SnappDoctor.Core.Entities;

public class Doctor
{
    public int Id { get; set; }
    public string? UserId { get; set; } // Foreign key to AspNetUsers
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Specialization { get; set; } = string.Empty;
    public string? MedicalLicenseNumber { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public byte[]? ProfilePicture { get; set; }
    public string Bio { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public decimal ConsultationFee { get; set; } = 150000; // Default consultation fee in Toman
    
    // Consultation Types Offered
    public bool OffersVoiceCall { get; set; } = true;
    public bool OffersVideoCall { get; set; } = true;
    public bool OffersInPersonConsultation { get; set; } = false;
    
    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }
    public bool IsAvailable { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual User? User { get; set; }
    public virtual ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
} 