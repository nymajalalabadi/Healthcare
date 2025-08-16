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

        // Calculate total spent from completed consultations
        var totalSpent = consultations.Where(c => c.Status == Core.Enums.ConsultationStatus.Completed)
                                   .Sum(c => c.Fee ?? 0m);

        // Get consultations for last 30 days for trend analysis
        var thirtyDaysAgo = DateTime.Now.AddDays(-30);
        var recentConsultations = consultations.Where(c => c.CreatedAt >= thirtyDaysAgo).ToList();

        // Calculate weekly trend data (last 7 days)
        var weeklyData = new List<int>();
        var weekLabels = new List<string>();
        for (int i = 6; i >= 0; i--)
        {
            var date = DateTime.Now.AddDays(-i);
            var dayConsultations = recentConsultations.Count(c => c.CreatedAt.Date == date.Date);
            weeklyData.Add(dayConsultations);
            weekLabels.Add(date.ToString("dddd", new System.Globalization.CultureInfo("fa-IR")));
        }

        // Calculate consultation types distribution
        var consultationTypes = consultations
            .Where(c => c.Status == Core.Enums.ConsultationStatus.Completed)
            .GroupBy(c => c.Type)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToList();

        // Get monthly spending data (last 6 months)
        var monthlySpending = new List<object>();
        var persianMonths = new[] { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" };
        
        for (int i = 5; i >= 0; i--)
        {
            var monthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-i);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);
            var monthSpending = consultations
                .Where(c => c.Status == Core.Enums.ConsultationStatus.Completed && 
                           c.CreatedAt >= monthStart && c.CreatedAt <= monthEnd)
                .Sum(c => c.Fee ?? 0m);
            
            monthlySpending.Add(new { 
                Month = persianMonths[monthStart.Month - 1], 
                Spending = monthSpending,
                MonthIndex = monthStart.Month
            });
        }

        // Get top doctors based on user's consultations
        var topDoctors = consultations
            .Where(c => c.Status == Core.Enums.ConsultationStatus.Completed && c.Doctor != null)
            .GroupBy(c => c.Doctor)
            .Select(g => new { 
                Doctor = g.Key, 
                ConsultationCount = g.Count(),
                TotalRating = g.Sum(c => c.Doctor?.Rating ?? 0),
                AverageRating = g.Average(c => c.Doctor?.Rating ?? 0)
            })
            .OrderByDescending(x => x.AverageRating)
            .ThenByDescending(x => x.ConsultationCount)
            .Take(5)
            .Select(x => new { 
                FullName = x.Doctor?.FullName ?? "نامشخص",
                Specialization = x.Doctor?.Specialization ?? "تخصص نامشخص",
                Rating = Math.Round(x.AverageRating, 1),
                ReviewCount = x.ConsultationCount
            })
            .ToList();

        // Get recent activity from actual consultations
        var recentActivity = consultations
            .OrderByDescending(c => c.CreatedAt)
            .Take(10)
            .Select(c => new
            {
                Description = GetActivityDescription(c),
                Timestamp = c.CreatedAt,
                Type = c.Status.ToString(),
                ConsultationId = c.Id
            })
            .ToList();

        // Calculate growth percentages (compare with previous period)
        var previousPeriodStart = DateTime.Now.AddDays(-60);
        var previousPeriodEnd = DateTime.Now.AddDays(-30);
        var currentPeriodStart = DateTime.Now.AddDays(-30);
        
        var previousPeriodConsultations = consultations.Count(c => c.CreatedAt >= previousPeriodStart && c.CreatedAt <= previousPeriodEnd);
        var currentPeriodConsultations = consultations.Count(c => c.CreatedAt >= currentPeriodStart);
        
        var consultationGrowth = previousPeriodConsultations > 0 
            ? Math.Round(((double)(currentPeriodConsultations - previousPeriodConsultations) / previousPeriodConsultations) * 100, 1)
            : 0.0;

        var previousPeriodSpending = consultations
            .Where(c => c.Status == Core.Enums.ConsultationStatus.Completed && 
                       c.CreatedAt >= previousPeriodStart && c.CreatedAt <= previousPeriodEnd)
            .Sum(c => c.Fee ?? 0m);
        var currentPeriodSpending = consultations
            .Where(c => c.Status == Core.Enums.ConsultationStatus.Completed && 
                       c.CreatedAt >= currentPeriodStart)
            .Sum(c => c.Fee ?? 0m);
        
        var spendingGrowth = previousPeriodSpending > 0 
            ? Math.Round(((double)(currentPeriodSpending - previousPeriodSpending) / (double)previousPeriodSpending) * 100, 1)
            : 0.0;

        ViewBag.TotalConsultations = totalConsultations;
        ViewBag.CompletedConsultations = completedConsultations;
        ViewBag.PendingConsultations = pendingConsultations;
        ViewBag.TotalSpent = totalSpent;
        
        // Analytics data
        ViewBag.WeeklyData = weeklyData;
        ViewBag.WeekLabels = weekLabels;
        ViewBag.ConsultationTypes = consultationTypes;
        ViewBag.MonthlySpending = monthlySpending;
        ViewBag.TopDoctors = topDoctors;
        ViewBag.RecentActivity = recentActivity;
        ViewBag.ConsultationGrowth = consultationGrowth;
        ViewBag.SpendingGrowth = spendingGrowth;

        return View(user);
    }

    private string GetActivityDescription(Core.Entities.Consultation consultation)
    {
        return consultation.Status switch
        {
            Core.Enums.ConsultationStatus.Pending => $"نوبت جدید با دکتر {consultation.Doctor?.FullName ?? "نامشخص"} رزرو شد",
            Core.Enums.ConsultationStatus.Confirmed => $"نوبت با دکتر {consultation.Doctor?.FullName ?? "نامشخص"} تأیید شد",
            Core.Enums.ConsultationStatus.Completed => $"مشاوره با دکتر {consultation.Doctor?.FullName ?? "نامشخص"} تکمیل شد",
            Core.Enums.ConsultationStatus.Cancelled => $"نوبت با دکتر {consultation.Doctor?.FullName ?? "نامشخص"} لغو شد",
            _ => $"وضعیت مشاوره تغییر کرد: {consultation.Status}"
        };
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
            .Take(10)
            .ToList();

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