using SnappDoctor.Core.Enums;

namespace SnappDoctor.Core.Entities;

public class Consultation
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int DoctorId { get; set; }
    public ConsultationType Type { get; set; }
    public ConsultationStatus Status { get; set; } = ConsultationStatus.Pending;
    public DateTime ScheduledAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public string PatientSymptoms { get; set; } = string.Empty;
    public string? DoctorNotes { get; set; }
    public string? Prescription { get; set; }
    public decimal Fee { get; set; }
    public bool IsPaid { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Doctor Doctor { get; set; } = null!;
} 