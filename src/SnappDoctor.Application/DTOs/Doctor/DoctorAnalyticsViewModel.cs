namespace SnappDoctor.Application.DTOs.Doctor;

public class DoctorAnalyticsViewModel
{
    public decimal TotalRevenue { get; set; }
    public int TotalPatients { get; set; }
    public int TotalConsultations { get; set; }
    public double AverageDailyConsultations { get; set; }
    public double PatientSatisfactionRating { get; set; }
    public int TotalRatings { get; set; }

    public int ChatConsultations { get; set; }
    public int VoiceCallConsultations { get; set; }
    public int VideoCallConsultations { get; set; }
    public int InPersonConsultations { get; set; }

    public List<RecentActivityDto> RecentActivities { get; set; } = new List<RecentActivityDto>();
}

public class RecentActivityDto
{
    public string Type { get; set; } // e.g., "ConsultationCompleted", "NewConsultation", "NewReview"
    public string Description { get; set; }
    public string TimeAgo { get; set; }
    public string Value { get; set; } // e.g., "+150,000 تومان", "در انتظار تأیید", "۵ ستاره"
}
