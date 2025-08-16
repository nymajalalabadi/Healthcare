using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SnappDoctor.Application.Contracts;
using SnappDoctor.Core.Entities;

namespace SnappDoctor.Web.Areas.User.Controllers;

[Area("User")]
[Authorize]
public class DashboardController : Controller
{
    private readonly UserManager<SnappDoctor.Core.Entities.User> _userManager;
    private readonly IConsultationRepository _consultationRepository;
    private readonly IDoctorRepository _doctorRepository;

    public DashboardController(
        UserManager<SnappDoctor.Core.Entities.User> userManager,
        IConsultationRepository consultationRepository,
        IDoctorRepository doctorRepository)
    {
        _userManager = userManager;
        _consultationRepository = consultationRepository;
        _doctorRepository = doctorRepository;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Auth", new { area = "" });
        }

        // Get user's consultation statistics
        var consultations = await _consultationRepository.GetUserConsultationsAsync(user.Id);
        var totalConsultations = consultations.Count();
        var completedConsultations = consultations.Count(c => c.Status == Core.Enums.ConsultationStatus.Completed);
        var pendingConsultations = consultations.Count(c => c.Status == Core.Enums.ConsultationStatus.Pending);

        ViewBag.TotalConsultations = totalConsultations;
        ViewBag.CompletedConsultations = completedConsultations;
        ViewBag.PendingConsultations = pendingConsultations;
        ViewBag.RecentConsultations = consultations.OrderByDescending(c => c.CreatedAt).Take(5);

        return View(user);
    }

    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Auth", new { area = "" });
        }

        // Get user's consultation statistics for profile page
        var consultations = await _consultationRepository.GetUserConsultationsAsync(user.Id);
        var totalConsultations = consultations.Count();
        var completedConsultations = consultations.Count(c => c.Status == Core.Enums.ConsultationStatus.Completed);
        var pendingConsultations = consultations.Count(c => c.Status == Core.Enums.ConsultationStatus.Pending);

        ViewBag.TotalConsultations = totalConsultations;
        ViewBag.CompletedConsultations = completedConsultations;
        ViewBag.PendingConsultations = pendingConsultations;

        // Get recent activity (placeholder data for now)
        ViewBag.RecentActivity = new List<object>
        {
            new { Description = "مشاوره با دکتر احمدی تکمیل شد", Timestamp = DateTime.Now.AddDays(-1) },
            new { Description = "نوبت جدید با دکتر محمدی رزرو شد", Timestamp = DateTime.Now.AddDays(-2) },
            new { Description = "پروفایل بروزرسانی شد", Timestamp = DateTime.Now.AddDays(-3) }
        };

        return View(user);
    }

    public async Task<IActionResult> Analytics()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Auth", new { area = "" });
        }

        // Get user's consultation statistics
        var consultations = await _consultationRepository.GetUserConsultationsAsync(user.Id);
        var totalConsultations = consultations.Count();
        var completedConsultations = consultations.Count(c => c.Status == Core.Enums.ConsultationStatus.Completed);
        var pendingConsultations = consultations.Count(c => c.Status == Core.Enums.ConsultationStatus.Pending);

        // Calculate total spent (placeholder calculation)
        var totalSpent = consultations.Where(c => c.Status == Core.Enums.ConsultationStatus.Completed)
                                   .Sum(c => c.Fee);

        ViewBag.TotalConsultations = totalConsultations;
        ViewBag.CompletedConsultations = completedConsultations;
        ViewBag.PendingConsultations = pendingConsultations;
        ViewBag.TotalSpent = totalSpent;

        // Get top doctors (placeholder data for now)
        var topDoctors = await _doctorRepository.GetAllAsync();
        ViewBag.TopDoctors = topDoctors.Take(5).ToList();

        // Get recent activity (placeholder data for now)
        ViewBag.RecentActivity = new List<object>
        {
            new { Description = "مشاوره با دکتر احمدی تکمیل شد", Timestamp = DateTime.Now.AddDays(-1) },
            new { Description = "نوبت جدید با دکتر محمدی رزرو شد", Timestamp = DateTime.Now.AddDays(-2) },
            new { Description = "پروفایل بروزرسانی شد", Timestamp = DateTime.Now.AddDays(-3) },
            new { Description = "مشاوره با دکتر رضایی لغو شد", Timestamp = DateTime.Now.AddDays(-4) },
            new { Description = "نوبت با دکتر کریمی تأیید شد", Timestamp = DateTime.Now.AddDays(-5) }
        };

        return View(user);
    }

    public async Task<IActionResult> Schedule()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Auth", new { area = "" });
        }

        // Get user's scheduled consultations
        var consultations = await _consultationRepository.GetUserConsultationsAsync(user.Id);
        var upcomingConsultations = consultations.Where(c => 
            c.Status == Core.Enums.ConsultationStatus.Pending || 
            c.Status == Core.Enums.ConsultationStatus.Confirmed)
            .OrderBy(c => c.ScheduledAt)
            .Take(10);

        ViewBag.UpcomingConsultations = upcomingConsultations;

        // Calculate schedule statistics
        var totalScheduled = consultations.Count(c => 
            c.Status == Core.Enums.ConsultationStatus.Pending || 
            c.Status == Core.Enums.ConsultationStatus.Confirmed);
        var completedScheduled = consultations.Count(c => c.Status == Core.Enums.ConsultationStatus.Completed);
        var upcomingScheduled = consultations.Count(c => 
            c.Status == Core.Enums.ConsultationStatus.Pending || 
            c.Status == Core.Enums.ConsultationStatus.Confirmed);
        var cancelledScheduled = consultations.Count(c => c.Status == Core.Enums.ConsultationStatus.Cancelled);

        ViewBag.TotalScheduled = totalScheduled;
        ViewBag.CompletedScheduled = completedScheduled;
        ViewBag.UpcomingScheduled = upcomingScheduled;
        ViewBag.CancelledScheduled = cancelledScheduled;

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProfile(string firstName, string lastName)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Auth", new { area = "" });
        }

        user.FirstName = firstName;
        user.LastName = lastName;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            TempData["Success"] = "پروفایل با موفقیت بروزرسانی شد";
        }
        else
        {
            TempData["Error"] = "خطا در بروزرسانی پروفایل";
        }

        return RedirectToAction("Profile");
    }

    [HttpPost]
    public async Task<IActionResult> AddSchedule(string title, DateTime scheduleDate, string scheduleTime, string description, string scheduleType)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Auth", new { area = "" });
        }

        // Here you would typically save the schedule to a database
        // For now, we'll just return a success message
        
        TempData["Success"] = "برنامه جدید با موفقیت اضافه شد";
        return RedirectToAction("Schedule");
    }

    [HttpPost]
    public async Task<IActionResult> AddReminder(string title, DateTime reminderDate, string reminderTime, string description)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Auth", new { area = "" });
        }

        // Here you would typically save the reminder to a database
        // For now, we'll just return a success message
        
        TempData["Success"] = "یادآوری جدید با موفقیت اضافه شد";
        return RedirectToAction("Schedule");
    }

    [HttpPost]
    public async Task<IActionResult> UploadProfilePicture(IFormFile profilePicture)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Auth", new { area = "" });
        }

        if (profilePicture == null || profilePicture.Length == 0)
        {
            TempData["Error"] = "لطفاً یک تصویر انتخاب کنید";
            return RedirectToAction("Profile");
        }

        // Validate file type
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var fileExtension = Path.GetExtension(profilePicture.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(fileExtension))
        {
            TempData["Error"] = "فرمت تصویر مجاز نیست. فرمت‌های مجاز: JPG, JPEG, PNG, GIF";
            return RedirectToAction("Profile");
        }

        // Validate file size (max 5MB)
        if (profilePicture.Length > 5 * 1024 * 1024)
        {
            TempData["Error"] = "حجم تصویر نباید بیشتر از 5 مگابایت باشد";
            return RedirectToAction("Profile");
        }

        // Convert IFormFile to byte array
        byte[] fileData;
        using (var memoryStream = new MemoryStream())
        {
            await profilePicture.CopyToAsync(memoryStream);
            fileData = memoryStream.ToArray();
        }

        // Update user's profile picture
        user.ProfilePicture = fileData;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            TempData["Success"] = "تصویر پروفایل با موفقیت بروزرسانی شد";
        }
        else
        {
            TempData["Error"] = "خطا در آپلود تصویر";
        }

        return RedirectToAction("Profile");
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