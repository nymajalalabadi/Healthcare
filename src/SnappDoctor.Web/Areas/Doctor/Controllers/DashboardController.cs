using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SnappDoctor.Application.Contracts;
using SnappDoctor.Core.Entities;

namespace SnappDoctor.Web.Areas.Doctor.Controllers;

[Area("Doctor")]
[Authorize(Roles = "Doctor")]
public class DashboardController : Controller
{
    private readonly IConsultationRepository _consultationRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly UserManager<SnappDoctor.Core.Entities.User> _userManager;

    public DashboardController(
        IConsultationRepository consultationRepository,
        IDoctorRepository doctorRepository,
        UserManager<SnappDoctor.Core.Entities.User> userManager)
    {
        _consultationRepository = consultationRepository;
        _doctorRepository = doctorRepository;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        // Get current user
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Auth", new { area = "" });
        }

        // Get doctor profile for current user
        var doctor = await _doctorRepository.GetByUserIdAsync(user.Id);
        if (doctor == null)
        {
            // If doctor profile doesn't exist, create a basic one
            var newDoctor = new SnappDoctor.Core.Entities.Doctor
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber ?? "",
                Email = null,
                Specialization = "عمومی",
                MedicalLicenseNumber = null,
                Bio = "",
                YearsOfExperience = 0,
                Rating = 0,
                ReviewCount = 0,
                IsAvailable = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            doctor = await _doctorRepository.CreateAsync(newDoctor);
        }

        var consultations = await _consultationRepository.GetDoctorConsultationsAsync(doctor.Id);

        var todayConsultations = consultations.Where(c => c.ScheduledAt.Date == DateTime.Today);
        var pendingConsultations = consultations.Where(c => c.Status == Core.Enums.ConsultationStatus.Pending);
        var completedConsultations = consultations.Where(c => c.Status == Core.Enums.ConsultationStatus.Completed);

        ViewBag.TodayConsultations = todayConsultations.Count();
        ViewBag.PendingConsultations = pendingConsultations.Count();
        ViewBag.CompletedConsultations = completedConsultations.Count();
        ViewBag.TotalEarnings = completedConsultations.Sum(c => c.Fee);
        ViewBag.RecentConsultations = consultations.OrderByDescending(c => c.CreatedAt).Take(5);

        return View(doctor);
    }

    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Auth", new { area = "" });
        }

        var doctor = await _doctorRepository.GetByUserIdAsync(user.Id);
        if (doctor == null)
        {
            return NotFound("پروفایل پزشک پیدا نشد");
        }

        return View(doctor);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAvailability(bool isAvailable)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Auth", new { area = "" });
        }

        var doctor = await _doctorRepository.GetByUserIdAsync(user.Id);
        if (doctor == null)
        {
            return NotFound("پروفایل پزشک پیدا نشد");
        }

        doctor.IsAvailable = isAvailable;
        doctor.UpdatedAt = DateTime.UtcNow;

        await _doctorRepository.UpdateAsync(doctor);
        
        TempData["Success"] = isAvailable ? "وضعیت شما به دردسترس تغییر یافت" : "وضعیت شما به غیردردسترس تغییر یافت";
        return RedirectToAction("Index");
    }
} 