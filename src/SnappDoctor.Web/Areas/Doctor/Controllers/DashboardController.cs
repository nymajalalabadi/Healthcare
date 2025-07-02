using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnappDoctor.Application.Contracts;

namespace SnappDoctor.Web.Areas.Doctor.Controllers;

[Area("Doctor")]
[Authorize(Roles = "Doctor")]
public class DashboardController : Controller
{
    private readonly IConsultationRepository _consultationRepository;
    private readonly IDoctorRepository _doctorRepository;

    public DashboardController(
        IConsultationRepository consultationRepository,
        IDoctorRepository doctorRepository)
    {
        _consultationRepository = consultationRepository;
        _doctorRepository = doctorRepository;
    }

    public async Task<IActionResult> Index()
    {
        // For now, we'll use a hardcoded doctor ID
        // In a real application, you'd get this from the authenticated user
        var doctorId = 1; // This should come from user claims or session
        
        var consultations = await _consultationRepository.GetDoctorConsultationsAsync(doctorId);
        var doctor = await _doctorRepository.GetByIdAsync(doctorId);

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
        var doctorId = 1; // This should come from user claims
        var doctor = await _doctorRepository.GetByIdAsync(doctorId);
        
        if (doctor == null)
        {
            return NotFound();
        }

        return View(doctor);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAvailability(bool isAvailable)
    {
        var doctorId = 1; // This should come from user claims
        var doctor = await _doctorRepository.GetByIdAsync(doctorId);
        
        if (doctor == null)
        {
            return NotFound();
        }

        doctor.IsAvailable = isAvailable;
        doctor.UpdatedAt = DateTime.UtcNow;

        await _doctorRepository.UpdateAsync(doctor);
        
        TempData["Success"] = isAvailable ? "وضعیت شما به دردسترس تغییر یافت" : "وضعیت شما به غیردردسترس تغییر یافت";
        return RedirectToAction("Index");
    }
} 