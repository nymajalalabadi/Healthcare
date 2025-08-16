using SnappDoctor.Application.DTOs.User;
using SnappDoctor.Application.DTOs.Doctor;

namespace SnappDoctor.Application.Contracts;

public interface IProfileService
{
    Task<ProfileDto?> GetProfileAsync(string userId);
    Task<bool> UpdateProfileAsync(string userId, UpdateProfileDto updateDto);
    Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);
    Task<string?> UploadProfilePictureAsync(string userId, byte[] fileData, string fileName);
    Task<DoctorAnalyticsViewModel> GetDoctorAnalyticsAsync(int doctorId);
} 