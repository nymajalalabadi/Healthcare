using Microsoft.AspNetCore.Identity;
using SnappDoctor.Application.Contracts;
using SnappDoctor.Application.DTOs.User;
using SnappDoctor.Core.Entities;
using SnappDoctor.Application.DTOs.Doctor;
using SnappDoctor.Core.Enums;

namespace SnappDoctor.Infrastructure.Services;

public class ProfileService : IProfileService
{
    private readonly UserManager<User> _userManager;
    private readonly IConsultationRepository _consultationRepository;
    private readonly string _uploadsPath;

    public ProfileService(UserManager<User> userManager, IConsultationRepository consultationRepository)
    {
        _userManager = userManager;
        _consultationRepository = consultationRepository;
        _uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
    }

    public async Task<ProfileDto?> GetProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;

        return new ProfileDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Email = user.Email,
            ProfilePictureUrl = user.ProfilePictureUrl,
            ProfilePicture = user.ProfilePicture,
            UserType = user.UserType,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task<bool> UpdateProfileAsync(string userId, UpdateProfileDto updateDto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        user.FirstName = updateDto.FirstName;
        user.LastName = updateDto.LastName;
        user.Email = updateDto.Email;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
        return result.Succeeded;
    }

    public async Task<string?> UploadProfilePictureAsync(string userId, byte[] fileData, string fileName)
    {
        if (fileData == null || fileData.Length == 0) return null;

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;

        try
        {
            // Create uploads directory if it doesn't exist
            if (!Directory.Exists(_uploadsPath))
            {
                Directory.CreateDirectory(_uploadsPath);
            }

            // Generate unique filename
            var fileExtension = Path.GetExtension(fileName);
            var newFileName = $"{userId}_{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(_uploadsPath, newFileName);

            // Delete old profile picture if exists
            if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var oldFilePath = Path.Combine(wwwrootPath, user.ProfilePictureUrl.TrimStart('/'));
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }
            }

            // Save new file
            await File.WriteAllBytesAsync(filePath, fileData);

            // Update user profile picture URL and binary data
            var profilePictureUrl = $"/uploads/profiles/{newFileName}";
            user.ProfilePictureUrl = profilePictureUrl;
            user.ProfilePicture = fileData; // Store binary data in database as well
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded ? profilePictureUrl : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<DoctorAnalyticsViewModel> GetDoctorAnalyticsAsync(int doctorId)
    {
        var consultations = await _consultationRepository.GetDoctorConsultationsAsync(doctorId);
        
        var viewModel = new DoctorAnalyticsViewModel
        {
            TotalRevenue = consultations.Where(c => c.Status == ConsultationStatus.Completed).Sum(c => c.Fee ?? 0m),
            TotalConsultations = consultations.Count(),
            TotalPatients = consultations.Select(c => c.UserId).Distinct().Count(),
            // Assuming average daily consultations calculation might need a date range, for now just a placeholder
            AverageDailyConsultations = consultations.Any() ? (double)consultations.Count() / 30 : 0, // Placeholder for 30 days
            
            ChatConsultations = consultations.Count(c => c.Type == ConsultationType.TextChat),
            VoiceCallConsultations = consultations.Count(c => c.Type == ConsultationType.VoiceCall),
            VideoCallConsultations = consultations.Count(c => c.Type == ConsultationType.VideoCall),
            InPersonConsultations = consultations.Count(c => c.Type == ConsultationType.InPerson),

            // Patient Satisfaction (placeholder - assuming a review system would provide this)
            PatientSatisfactionRating = 4.8, // Placeholder
            TotalRatings = 124, // Placeholder
            
            RecentActivities = new List<RecentActivityDto>()
        };

        // Populate Recent Activities (example data based on consultation status)
        foreach (var consultation in consultations.OrderByDescending(c => c.CreatedAt).Take(3))
        {
            var activity = new RecentActivityDto
            {
                Description = $"مشاوره با {consultation.User?.FirstName} {consultation.User?.LastName} ",
                TimeAgo = (DateTime.UtcNow - consultation.CreatedAt).TotalHours < 24 ? 
                            $"{(int)(DateTime.UtcNow - consultation.CreatedAt).TotalHours} ساعت پیش" :
                            $"{(int)(DateTime.UtcNow - consultation.CreatedAt).TotalDays} روز پیش",
                Type = "Consultation"
            };

            if (consultation.Status == ConsultationStatus.Completed)
            {
                activity.Type = "ConsultationCompleted";
                activity.Description += "تکمیل شد";
                activity.Value = $"+{(consultation.Fee ?? 0).ToString("N0")} تومان";
            }
            else if (consultation.Status == ConsultationStatus.Pending)
            {
                activity.Type = "NewConsultation";
                activity.Description += "در انتظار تأیید";
                activity.Value = "در انتظار تأیید";
            }
            // Add other statuses/types as needed
            viewModel.RecentActivities.Add(activity);
        }

        return viewModel;
    }
} 