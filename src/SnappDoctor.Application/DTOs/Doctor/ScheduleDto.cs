using System.ComponentModel.DataAnnotations;

namespace SnappDoctor.Application.DTOs.Doctor;

public class DoctorScheduleDto
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public bool IsActive { get; set; }
    
    [Required(ErrorMessage = "زمان شروع الزامی است")]
    public TimeOnly StartTime { get; set; }
    
    [Required(ErrorMessage = "زمان پایان الزامی است")]
    public TimeOnly EndTime { get; set; }
}

public class DoctorBreakTimeDto
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    
    [Required(ErrorMessage = "نوع استراحت الزامی است")]
    public string BreakType { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "عنوان الزامی است")]
    public string Title { get; set; } = string.Empty;
    
    public bool IsActive { get; set; }
    
    [Required(ErrorMessage = "زمان شروع الزامی است")]
    public TimeOnly StartTime { get; set; }
    
    [Required(ErrorMessage = "زمان پایان الزامی است")]
    public TimeOnly EndTime { get; set; }
}

public class DoctorTimeSettingsDto
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    
    [Range(10, 120, ErrorMessage = "مدت مشاوره باید بین 10 تا 120 دقیقه باشد")]
    public int ConsultationDurationMinutes { get; set; } = 30;
    
    [Range(0, 60, ErrorMessage = "فاصله بین مشاوره‌ها باید بین 0 تا 60 دقیقه باشد")]
    public int BreakBetweenConsultationsMinutes { get; set; } = 5;
    
    [Range(1, 100, ErrorMessage = "حداکثر مشاوره روزانه باید بین 1 تا 100 باشد")]
    public int MaxDailyConsultations { get; set; } = 20;
}

public class UpdateScheduleDto
{
    public List<DoctorScheduleDto> Schedules { get; set; } = new();
    public List<DoctorBreakTimeDto> BreakTimes { get; set; } = new();
    public DoctorTimeSettingsDto TimeSettings { get; set; } = new();
    public bool OffersVoiceCall { get; set; }
    public bool OffersVideoCall { get; set; }
    public bool OffersInPersonConsultation { get; set; }
    public decimal ConsultationFee { get; set; }
} 