using SnappDoctor.Core.Entities;

namespace SnappDoctor.Application.DTOs.Doctor;

public class ScheduleViewModel
{
    public SnappDoctor.Core.Entities.Doctor Doctor { get; set; } = null!;
    public List<DoctorScheduleDto> Schedules { get; set; } = new();
    public List<DoctorBreakTimeDto> BreakTimes { get; set; } = new();
    public DoctorTimeSettingsDto TimeSettings { get; set; } = new();
} 