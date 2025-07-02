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
} 