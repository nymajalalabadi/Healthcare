using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SnappDoctor.Application.Contracts;
using SnappDoctor.Application.DTOs.User;
using SnappDoctor.Core.Entities;

namespace SnappDoctor.Web.Areas.User.Controllers;

[Area("User")]
[Authorize(Roles = "User")]
public class ProfileController : Controller
{
    private readonly IProfileService _profileService;
    private readonly UserManager<SnappDoctor.Core.Entities.User> _userManager;

    public ProfileController(IProfileService profileService, UserManager<SnappDoctor.Core.Entities.User> userManager)
    {
        _profileService = profileService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return RedirectToAction("Login", "Auth", new { area = "" });

        var profile = await _profileService.GetProfileAsync(userId);
        if (profile == null) return NotFound();

        // Get dynamic data for the profile page
        var consultations = await _profileService.GetUserConsultationsAsync(userId);
        
        // Calculate statistics
        var totalConsultations = consultations.Count;
        var completedConsultations = consultations.Count(c => c.Status == Core.Enums.ConsultationStatus.Completed);
        var pendingConsultations = consultations.Count(c => c.Status == Core.Enums.ConsultationStatus.Pending || c.Status == Core.Enums.ConsultationStatus.Confirmed);

        // Get recent activity (last 10 consultations) - only if there are actual consultations
        var recentActivity = consultations.Any() ? consultations
            .OrderByDescending(c => c.CreatedAt)
            .Take(10)
            .Select(c => new
            {
                Description = GetActivityDescription(c),
                Timestamp = c.CreatedAt,
                Type = c.Status.ToString(),
                ConsultationId = c.Id
            })
            .ToList() : null;

        // Populate ViewBag with dynamic data
        ViewBag.TotalConsultations = totalConsultations;
        ViewBag.CompletedConsultations = completedConsultations;
        ViewBag.PendingConsultations = pendingConsultations;
        ViewBag.RecentActivity = recentActivity;

        return View(profile);
    }

    private string GetActivityDescription(Core.Entities.Consultation consultation)
    {
        var doctorName = consultation.Doctor?.FullName ?? "دکتر نامشخص";
        var consultationType = GetConsultationTypeText(consultation.Type);
        
        return consultation.Status switch
        {
            Core.Enums.ConsultationStatus.Confirmed => $"نوبت {consultationType} با {doctorName} تأیید شد",
            Core.Enums.ConsultationStatus.Pending => $"درخواست {consultationType} با {doctorName} در انتظار تأیید",
            Core.Enums.ConsultationStatus.InProgress => $"مشاوره {consultationType} با {doctorName} در حال انجام",
            Core.Enums.ConsultationStatus.Completed => $"مشاوره {consultationType} با {doctorName} تکمیل شد",
            Core.Enums.ConsultationStatus.Cancelled => $"مشاوره {consultationType} با {doctorName} لغو شد",
            Core.Enums.ConsultationStatus.NoShow => $"مشاوره {consultationType} با {doctorName} لغو شد (عدم حضور)",
            _ => $"تغییر وضعیت مشاوره {consultationType} با {doctorName}"
        };
    }

    private string GetConsultationTypeText(Core.Enums.ConsultationType type)
    {
        return type switch
        {
            Core.Enums.ConsultationType.TextChat => "چت متنی",
            Core.Enums.ConsultationType.VoiceCall => "تماس صوتی",
            Core.Enums.ConsultationType.VideoCall => "تماس تصویری",
            Core.Enums.ConsultationType.InPerson => "حضوری",
            _ => "مشاوره"
        };
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return RedirectToAction("Login", "Auth", new { area = "" });

        var profile = await _profileService.GetProfileAsync(userId);
        if (profile == null) return NotFound();

        var updateDto = new UpdateProfileDto
        {
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            Email = profile.Email
        };

        return View(updateDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateProfileDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = _userManager.GetUserId(User);
        if (userId == null) return RedirectToAction("Login", "Auth", new { area = "" });

        var result = await _profileService.UpdateProfileAsync(userId, model);
        if (result)
        {
            TempData["Success"] = "پروفایل با موفقیت بروزرسانی شد";
            return RedirectToAction("Index");
        }

        TempData["Error"] = "خطا در بروزرسانی پروفایل";
        return View(model);
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = _userManager.GetUserId(User);
        if (userId == null) return RedirectToAction("Login", "Auth", new { area = "" });

        var result = await _profileService.ChangePasswordAsync(userId, model);
        if (result)
        {
            TempData["Success"] = "رمز عبور با موفقیت تغییر کرد";
            return RedirectToAction("Index");
        }

        TempData["Error"] = "رمز عبور فعلی نادرست است";
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadProfilePicture(IFormFile profilePicture)
    {
        if (profilePicture == null || profilePicture.Length == 0)
        {
            TempData["Error"] = "لطفاً یک تصویر انتخاب کنید";
            return RedirectToAction("Index");
        }

        // Validate file type
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var fileExtension = Path.GetExtension(profilePicture.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(fileExtension))
        {
            TempData["Error"] = "فرمت تصویر مجاز نیست. فرمت‌های مجاز: JPG, JPEG, PNG, GIF";
            return RedirectToAction("Index");
        }

        // Validate file size (max 5MB)
        if (profilePicture.Length > 5 * 1024 * 1024)
        {
            TempData["Error"] = "حجم تصویر نباید بیشتر از 5 مگابایت باشد";
            return RedirectToAction("Index");
        }

        var userId = _userManager.GetUserId(User);
        if (userId == null) return RedirectToAction("Login", "Auth", new { area = "" });

        // Convert IFormFile to byte array
        byte[] fileData;
        using (var memoryStream = new MemoryStream())
        {
            await profilePicture.CopyToAsync(memoryStream);
            fileData = memoryStream.ToArray();
        }

        var result = await _profileService.UploadProfilePictureAsync(userId, fileData, profilePicture.FileName);
        if (result != null)
        {
            TempData["Success"] = "تصویر پروفایل با موفقیت بروزرسانی شد";
        }
        else
        {
            TempData["Error"] = "خطا در آپلود تصویر";
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> GetProfilePicture(string? userId = null)
    {
        var targetUserId = userId ?? _userManager.GetUserId(User);
        if (targetUserId == null) return NotFound();

        var user = await _userManager.FindByIdAsync(targetUserId);
        if (user?.ProfilePicture == null) return NotFound();

        // Determine content type based on file signature
        string contentType = GetContentType(user.ProfilePicture);
        
        return File(user.ProfilePicture, contentType);
    }

    private string GetContentType(byte[] fileData)
    {
        if (fileData.Length >= 2)
        {
            // Check for JPEG
            if (fileData[0] == 0xFF && fileData[1] == 0xD8)
                return "image/jpeg";
            
            // Check for PNG
            if (fileData.Length >= 8 && fileData[0] == 0x89 && fileData[1] == 0x50 && 
                fileData[2] == 0x4E && fileData[3] == 0x47)
                return "image/png";
            
            // Check for GIF
            if (fileData.Length >= 6 && fileData[0] == 0x47 && fileData[1] == 0x49 && fileData[2] == 0x46)
                return "image/gif";
        }
        
        return "image/jpeg"; // Default fallback
    }
} 