using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SnappDoctor.Application.Contracts;
using SnappDoctor.Core.Entities;
using SnappDoctor.Core.Enums;

namespace SnappDoctor.Web.Areas.User.Controllers;

[Area("User")]
[Authorize]
public class DoctorsController : Controller
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IConsultationRepository _consultationRepository;
    private readonly UserManager<SnappDoctor.Core.Entities.User> _userManager;

    public DoctorsController(
        IDoctorRepository doctorRepository,
        IConsultationRepository consultationRepository,
        UserManager<SnappDoctor.Core.Entities.User> userManager)
    {
        _doctorRepository = doctorRepository;
        _consultationRepository = consultationRepository;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? specialization)
    {
        IEnumerable<SnappDoctor.Core.Entities.Doctor> doctors;
        
        if (!string.IsNullOrEmpty(specialization))
        {
            doctors = await _doctorRepository.GetBySpecializationAsync(specialization);
            ViewBag.CurrentSpecialization = specialization;
        }
        else
        {
            doctors = await _doctorRepository.GetAvailableDoctorsAsync();
        }

        // Get unique specializations for filter
        var allDoctors = await _doctorRepository.GetAllAsync();
        ViewBag.Specializations = allDoctors.Select(d => d.Specialization).Distinct().ToList();

        return View(doctors);
    }

    public async Task<IActionResult> Details(int id)
    {
        var doctor = await _doctorRepository.GetByIdAsync(id);
        if (doctor == null)
        {
            return NotFound();
        }

        return View(doctor);
    }

    [HttpPost]
    public async Task<IActionResult> BookConsultation(int doctorId, ConsultationType type, string symptoms)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Auth", new { area = "" });
        }

        var doctor = await _doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null || !doctor.IsAvailable)
        {
            TempData["Error"] = "پزشک انتخابی در دسترس نیست";
            return RedirectToAction("Details", new { id = doctorId });
        }

        var consultation = new Consultation
        {
            UserId = user.Id,
            DoctorId = doctorId,
            Type = type,
            PatientSymptoms = symptoms,
            Status = ConsultationStatus.Pending,
            ScheduledAt = DateTime.UtcNow.AddHours(1), // Default to 1 hour from now
            Fee = GetConsultationFee(type)
        };

        var createdConsultation = await _consultationRepository.CreateAsync(consultation);
        
        TempData["Success"] = "درخواست مشاوره با موفقیت ثبت شد";
        return RedirectToAction("Index", "Consultations");
    }

    private decimal GetConsultationFee(ConsultationType type)
    {
        return type switch
        {
            ConsultationType.TextChat => 50000,
            ConsultationType.VoiceCall => 80000,
            ConsultationType.VideoCall => 120000,
            ConsultationType.InPerson => 200000,
            _ => 50000
        };
    }

    [HttpGet]
    public async Task<IActionResult> GetDoctorProfilePicture(int doctorId)
    {
        var doctor = await _doctorRepository.GetByIdAsync(doctorId);
        if (doctor?.ProfilePicture == null) return NotFound();

        // Determine content type based on file signature
        string contentType = GetContentType(doctor.ProfilePicture);
        
        return File(doctor.ProfilePicture, contentType);
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