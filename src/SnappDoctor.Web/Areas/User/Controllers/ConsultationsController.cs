using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SnappDoctor.Application.Contracts;
using SnappDoctor.Core.Enums;

namespace SnappDoctor.Web.Areas.User.Controllers;

[Area("User")]
[Authorize]
public class ConsultationsController : Controller
{
    private readonly IConsultationRepository _consultationRepository;
    private readonly UserManager<SnappDoctor.Core.Entities.User> _userManager;

    public ConsultationsController(
        IConsultationRepository consultationRepository,
        UserManager<SnappDoctor.Core.Entities.User> userManager)
    {
        _consultationRepository = consultationRepository;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Auth", new { area = "" });
        }

        var consultations = await _consultationRepository.GetUserConsultationsAsync(user.Id);
        return View(consultations.OrderByDescending(c => c.CreatedAt));
    }

    public async Task<IActionResult> Details(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Auth", new { area = "" });
        }

        var consultation = await _consultationRepository.GetByIdAsync(id);
        if (consultation == null || consultation.UserId != user.Id)
        {
            return NotFound();
        }

        return View(consultation);
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Auth", new { area = "" });
        }

        var consultation = await _consultationRepository.GetByIdAsync(id);
        if (consultation == null || consultation.UserId != user.Id)
        {
            return NotFound();
        }

        if (consultation.Status == ConsultationStatus.Pending)
        {
            consultation.Status = ConsultationStatus.Cancelled;
            consultation.UpdatedAt = DateTime.UtcNow;
            
            await _consultationRepository.UpdateAsync(consultation);
            TempData["Success"] = "مشاوره با موفقیت لغو شد";
        }
        else
        {
            TempData["Error"] = "امکان لغو این مشاوره وجود ندارد";
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> StartConsultation(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Auth", new { area = "" });
        }

        var consultation = await _consultationRepository.GetByIdAsync(id);
        if (consultation == null || consultation.UserId != user.Id)
        {
            return NotFound();
        }

        if (consultation.Status == Core.Enums.ConsultationStatus.Confirmed || 
            consultation.Status == Core.Enums.ConsultationStatus.InProgress)
        {
            // Here you would typically start the consultation (e.g., redirect to video call, chat, etc.)
            consultation.Status = Core.Enums.ConsultationStatus.InProgress;
            consultation.UpdatedAt = DateTime.UtcNow;
            
            await _consultationRepository.UpdateAsync(consultation);
            TempData["Success"] = "مشاوره شروع شد";
        }
        else
        {
            TempData["Error"] = "امکان شروع این مشاوره وجود ندارد";
        }

        return RedirectToAction("Details", new { id = id });
    }

    [HttpPost]
    public async Task<IActionResult> CompleteConsultation(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Auth", new { area = "" });
        }

        var consultation = await _consultationRepository.GetByIdAsync(id);
        if (consultation == null || consultation.UserId != user.Id)
        {
            return NotFound();
        }

        if (consultation.Status == Core.Enums.ConsultationStatus.InProgress)
        {
            consultation.Status = Core.Enums.ConsultationStatus.Completed;
            consultation.UpdatedAt = DateTime.UtcNow;
            
            await _consultationRepository.UpdateAsync(consultation);
            TempData["Success"] = "مشاوره تکمیل شد";
        }
        else
        {
            TempData["Error"] = "امکان تکمیل این مشاوره وجود ندارد";
        }

        return RedirectToAction("Details", new { id = id });
    }
} 