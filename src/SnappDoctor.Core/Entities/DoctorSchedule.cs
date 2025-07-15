namespace SnappDoctor.Core.Entities;

public class DoctorSchedule
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public bool IsActive { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation property
    public virtual Doctor Doctor { get; set; } = null!;
} 