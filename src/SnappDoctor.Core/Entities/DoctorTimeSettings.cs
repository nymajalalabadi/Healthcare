namespace SnappDoctor.Core.Entities;

public class DoctorTimeSettings
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public int ConsultationDurationMinutes { get; set; } = 30;
    public int BreakBetweenConsultationsMinutes { get; set; } = 5;
    public int MaxDailyConsultations { get; set; } = 20;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation property
    public virtual Doctor Doctor { get; set; } = null!;
} 