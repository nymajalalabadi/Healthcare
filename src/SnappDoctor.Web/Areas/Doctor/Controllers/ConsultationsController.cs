using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnappDoctor.Application.Contracts;
using SnappDoctor.Core.Enums;

namespace SnappDoctor.Web.Areas.Doctor.Controllers;

[Area("Doctor")]
[Authorize(Roles = "Doctor")]
public class ConsultationsController : Controller
{
    private readonly IConsultationRepository _consultationRepository;

    public ConsultationsController(IConsultationRepository consultationRepository)
    {
        _consultationRepository = consultationRepository;
    }

    public async Task<IActionResult> Index(ConsultationStatus? status)
    {
        var doctorId = 1; // This should come from user claims
        var consultations = await _consultationRepository.GetDoctorConsultationsAsync(doctorId);
        
        if (status.HasValue)
        {
            consultations = consultations.Where(c => c.Status == status.Value);
            ViewBag.CurrentStatus = status.Value;
        }

        return View(consultations.OrderByDescending(c => c.ScheduledAt));
    }

    public async Task<IActionResult> Details(int id)
    {
        var doctorId = 1; // This should come from user claims
        var consultation = await _consultationRepository.GetByIdAsync(id);
        
        if (consultation == null || consultation.DoctorId != doctorId)
        {
            return NotFound();
        }

        return View(consultation);
    }

    [HttpPost]
    public async Task<IActionResult> Accept(int id)
    {
        var doctorId = 1; // This should come from user claims
        var consultation = await _consultationRepository.GetByIdAsync(id);
        
        if (consultation == null || consultation.DoctorId != doctorId)
        {
            return NotFound();
        }

        if (consultation.Status == ConsultationStatus.Pending)
        {
            consultation.Status = ConsultationStatus.Confirmed;
            consultation.UpdatedAt = DateTime.UtcNow;
            
            await _consultationRepository.UpdateAsync(consultation);
            TempData["Success"] = "مشاوره تأیید شد";
        }

        return RedirectToAction("Details", new { id });
    }

    [HttpPost]
    public async Task<IActionResult> Start(int id)
    {
        var doctorId = 1; // This should come from user claims
        var consultation = await _consultationRepository.GetByIdAsync(id);
        
        if (consultation == null || consultation.DoctorId != doctorId)
        {
            return NotFound();
        }

        if (consultation.Status == ConsultationStatus.Confirmed)
        {
            consultation.Status = ConsultationStatus.InProgress;
            consultation.StartedAt = DateTime.UtcNow;
            consultation.UpdatedAt = DateTime.UtcNow;
            
            await _consultationRepository.UpdateAsync(consultation);
            TempData["Success"] = "مشاوره شروع شد";
        }

        return RedirectToAction("Details", new { id });
    }

    [HttpPost]
    public async Task<IActionResult> Complete(int id, string doctorNotes, string prescription)
    {
        var doctorId = 1; // This should come from user claims
        var consultation = await _consultationRepository.GetByIdAsync(id);
        
        if (consultation == null || consultation.DoctorId != doctorId)
        {
            return NotFound();
        }

        if (consultation.Status == ConsultationStatus.InProgress)
        {
            consultation.Status = ConsultationStatus.Completed;
            consultation.EndedAt = DateTime.UtcNow;
            consultation.UpdatedAt = DateTime.UtcNow;
            consultation.DoctorNotes = doctorNotes;
            consultation.Prescription = prescription;
            
            await _consultationRepository.UpdateAsync(consultation);
            TempData["Success"] = "مشاوره تکمیل شد";
        }

        return RedirectToAction("Index");
    }
} 