using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SnappDoctor.Application.Contracts;
using SnappDoctor.Core.Entities;

namespace SnappDoctor.Web.Areas.Doctor.Controllers;

[Area("Doctor")]
[Authorize(Roles = "Doctor")]
public class ProfileController : Controller
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly UserManager<SnappDoctor.Core.Entities.User> _userManager;
    private readonly IWebHostEnvironment _environment;

    public ProfileController(
        IDoctorRepository doctorRepository,
        UserManager<SnappDoctor.Core.Entities.User> userManager,
        IWebHostEnvironment environment)
    {
        _doctorRepository = doctorRepository;
        _userManager = userManager;
        _environment = environment;
    }

    private async Task<SnappDoctor.Core.Entities.Doctor?> GetCurrentDoctorAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return null;
        
        return await _doctorRepository.GetByUserIdAsync(user.Id);
    }

    public async Task<IActionResult> Index()
    {
        var doctor = await GetCurrentDoctorAsync();
        if (doctor == null)
        {
            return RedirectToAction("Login", "Auth", new { area = "" });
        }

        return View(doctor);
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var doctor = await GetCurrentDoctorAsync();
        if (doctor == null)
        {
            return RedirectToAction("Login", "Auth", new { area = "" });
        }

        return View(doctor);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(SnappDoctor.Core.Entities.Doctor model)
    {
        var doctor = await GetCurrentDoctorAsync();
        if (doctor == null)
        {
            return RedirectToAction("Login", "Auth", new { area = "" });
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            // Update doctor fields
            doctor.FirstName = model.FirstName;
            doctor.LastName = model.LastName;
            doctor.Email = model.Email;
            doctor.Specialization = model.Specialization;
            doctor.MedicalLicenseNumber = model.MedicalLicenseNumber;
            doctor.Bio = model.Bio;
            doctor.YearsOfExperience = model.YearsOfExperience;
            doctor.ConsultationFee = model.ConsultationFee;
            
            // Update consultation types offered
            doctor.OffersVoiceCall = model.OffersVoiceCall;
            doctor.OffersVideoCall = model.OffersVideoCall;
            doctor.OffersInPersonConsultation = model.OffersInPersonConsultation;
            
            doctor.UpdatedAt = DateTime.UtcNow;

            await _doctorRepository.UpdateAsync(doctor);

            // Also update user information
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                await _userManager.UpdateAsync(user);
            }

            TempData["Success"] = "پروفایل با موفقیت بروزرسانی شد";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "خطا در بروزرسانی پروفایل: " + ex.Message);
            return View(model);
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAvailability(bool isAvailable)
    {
        var doctor = await GetCurrentDoctorAsync();
        if (doctor == null)
        {
            return RedirectToAction("Login", "Auth", new { area = "" });
        }

        doctor.IsAvailable = isAvailable;
        doctor.UpdatedAt = DateTime.UtcNow;

        await _doctorRepository.UpdateAsync(doctor);
        
        TempData["Success"] = isAvailable ? "وضعیت شما به در دسترس تغییر یافت" : "وضعیت شما به غیر قابل دسترس تغییر یافت";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> UploadProfilePicture(IFormFile profilePicture)
    {
        var doctor = await GetCurrentDoctorAsync();
        if (doctor == null)
        {
            return RedirectToAction("Login", "Auth", new { area = "" });
        }

        if (profilePicture == null || profilePicture.Length == 0)
        {
            TempData["Error"] = "لطفاً یک تصویر انتخاب کنید";
            return RedirectToAction("Index");
        }

        // Validate file type
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
        if (!allowedTypes.Contains(profilePicture.ContentType.ToLower()))
        {
            TempData["Error"] = "فقط فایل‌های JPG، PNG و GIF مجاز هستند";
            return RedirectToAction("Index");
        }

        // Validate file size (5MB max)
        if (profilePicture.Length > 5 * 1024 * 1024)
        {
            TempData["Error"] = "حجم فایل نباید بیشتر از 5 مگابایت باشد";
            return RedirectToAction("Index");
        }

        try
        {
            // Save to database as byte array
            using (var memoryStream = new MemoryStream())
            {
                await profilePicture.CopyToAsync(memoryStream);
                doctor.ProfilePicture = memoryStream.ToArray();
            }

            doctor.UpdatedAt = DateTime.UtcNow;
            await _doctorRepository.UpdateAsync(doctor);

            TempData["Success"] = "تصویر پروفایل با موفقیت بروزرسانی شد";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "خطا در آپلود تصویر: " + ex.Message;
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> GetProfilePicture()
    {
        var doctor = await GetCurrentDoctorAsync();
        if (doctor?.ProfilePicture == null)
        {
            return NotFound();
        }

        // Detect content type based on file signature
        string contentType = "image/jpeg"; // default
        if (doctor.ProfilePicture.Length > 3)
        {
            if (doctor.ProfilePicture[0] == 0x89 && doctor.ProfilePicture[1] == 0x50 && doctor.ProfilePicture[2] == 0x4E && doctor.ProfilePicture[3] == 0x47)
                contentType = "image/png";
            else if (doctor.ProfilePicture[0] == 0x47 && doctor.ProfilePicture[1] == 0x49 && doctor.ProfilePicture[2] == 0x46)
                contentType = "image/gif";
        }

        return File(doctor.ProfilePicture, contentType);
    }

    [HttpGet]
    public async Task<IActionResult> Schedule()
    {
        var doctor = await GetCurrentDoctorAsync();
        if (doctor == null)
        {
            return RedirectToAction("Login", "Auth", new { area = "" });
        }

        // This would contain schedule management logic
        // For now, return a basic view
        return View(doctor);
    }

    [HttpGet]
    public async Task<IActionResult> Analytics()
    {
        var doctor = await GetCurrentDoctorAsync();
        if (doctor == null)
        {
            return RedirectToAction("Login", "Auth", new { area = "" });
        }

        // This would contain analytics and reports
        // For now, return a basic view
        return View(doctor);
    }
} 