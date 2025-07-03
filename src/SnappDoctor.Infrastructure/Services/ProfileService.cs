using Microsoft.AspNetCore.Identity;
using SnappDoctor.Application.Contracts;
using SnappDoctor.Application.DTOs.User;
using SnappDoctor.Core.Entities;

namespace SnappDoctor.Infrastructure.Services;

public class ProfileService : IProfileService
{
    private readonly UserManager<User> _userManager;
    private readonly string _uploadsPath;

    public ProfileService(UserManager<User> userManager)
    {
        _userManager = userManager;
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
} 