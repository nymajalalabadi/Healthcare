using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SnappDoctor.Application.Contracts;
using SnappDoctor.Application.DTOs.Auth;
using SnappDoctor.Core.Entities;
using SnappDoctor.Core.Enums;

namespace SnappDoctor.Web.Controllers;

public class AuthController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IOtpRepository _otpRepository;
    private readonly ISmsService _smsService;
    private readonly IDoctorRepository _doctorRepository;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthController(
        IUserRepository userRepository,
        IOtpRepository otpRepository,
        ISmsService smsService,
        IDoctorRepository doctorRepository,
        SignInManager<User> signInManager,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userRepository = userRepository;
        _otpRepository = otpRepository;
        _smsService = smsService;
        _doctorRepository = doctorRepository;
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterDto());
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Check if user already exists
        if (await _userRepository.ExistsAsync(model.PhoneNumber))
        {
            ModelState.AddModelError("PhoneNumber", "کاربری با این شماره موبایل قبلاً ثبت‌نام کرده است");
            return View(model);
        }

        try
        {
            // Create user
            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                PhoneNumberConfirmed = false,
                UserType = model.UserType
            };

            var createdUser = await _userRepository.CreateAsync(user, model.Password);

            // Assign role based on user type
            await EnsureRolesExist();
            var roleName = model.UserType == UserType.Doctor ? "Doctor" : "User";
            await _userManager.AddToRoleAsync(createdUser, roleName);

            // If user is registering as a doctor, create Doctor record
            if (model.UserType == UserType.Doctor)
            {
                try
                {
                    var doctor = new Doctor
                    {
                        UserId = createdUser.Id, // Link to the created user
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        PhoneNumber = model.PhoneNumber,
                        Email = null, // Will be updated later in profile
                        Specialization = "عمومی", // Default specialization
                        MedicalLicenseNumber = null, // Will be updated later
                        Bio = "",
                        YearsOfExperience = 0,
                        Rating = 0,
                        ReviewCount = 0,
                        IsAvailable = false, // Doctor needs to complete profile first
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    var createdDoctor = await _doctorRepository.CreateAsync(doctor);
                    
                    // Verify both User and Doctor were created successfully
                    if (createdDoctor == null)
                    {
                        throw new InvalidOperationException("ایجاد حساب پزشک با مشکل مواجه شد.");
                    }
                }
                catch (Exception doctorEx)
                {
                    // If doctor creation fails, we should still let the user complete registration
                    // but log the error for debugging
                    Console.WriteLine($"خطا در ایجاد رکورد پزشک: {doctorEx.Message}");
                    // The user account is still created, doctor profile can be completed later
                }
            }

            // Generate and send OTP
            var otpCode = new Random().Next(1000, 9999).ToString();
            var otp = new OtpCode
            {
                UserId = createdUser.Id,
                PhoneNumber = model.PhoneNumber,
                Code = otpCode,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                Purpose = "Registration"
            };

            await _otpRepository.CreateAsync(otp);
            await _smsService.SendOtpAsync(model.PhoneNumber, otpCode);

            TempData["PhoneNumber"] = model.PhoneNumber;
            TempData["UserType"] = model.UserType.ToString();
            TempData["Message"] = "ثبت‌نام با موفقیت انجام شد. کد تایید به شماره موبایل شما ارسال شده است.";
            
            return RedirectToAction("VerifyOtp");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginDto());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userRepository.GetByPhoneNumberAsync(model.PhoneNumber);
        if (user == null)
        {
            ModelState.AddModelError("", "شماره موبایل یا رمز عبور اشتباه است");
            return View(model);
        }

        if (!user.PhoneNumberConfirmed)
        {
            ModelState.AddModelError("", "شماره موبایل شما هنوز تایید نشده است");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            user.UserName!, model.Password, model.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            // Redirect based on user role
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Contains("Doctor"))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Doctor" });
            }
            else
            {
                return RedirectToAction("Index", "Dashboard", new { area = "User" });
            }
        }

        ModelState.AddModelError("", "شماره موبایل یا رمز عبور اشتباه است");
        return View(model);
    }

    [HttpGet]
    public IActionResult VerifyOtp()
    {
        var phoneNumber = TempData["PhoneNumber"]?.ToString();
        if (string.IsNullOrEmpty(phoneNumber))
        {
            return RedirectToAction("Register");
        }

        var model = new VerifyOtpDto { PhoneNumber = phoneNumber };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> VerifyOtp(VerifyOtpDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var validOtp = await _otpRepository.GetValidOtpAsync(
            model.PhoneNumber, model.Code, model.Purpose);

        if (validOtp == null)
        {
            ModelState.AddModelError("Code", "کد تایید نامعتبر یا منقضی شده است");
            return View(model);
        }

        // Mark OTP as used
        await _otpRepository.MarkAsUsedAsync(validOtp.Id);

        // Confirm user phone number
        await _userRepository.ConfirmPhoneNumberAsync(validOtp.UserId);

        // Sign in user
        var user = await _userRepository.GetByIdAsync(validOtp.UserId);
        if (user != null)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
        }

        TempData["Message"] = "شماره موبایل با موفقیت تایید شد. خوش آمدید!";
        
        // Redirect based on user role
        if (user != null)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Contains("Doctor"))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Doctor" });
            }
            else
            {
                return RedirectToAction("Index", "Dashboard", new { area = "User" });
            }
        }
        
        return RedirectToAction("Index", "Dashboard", new { area = "User" });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> ResendOtp(string phoneNumber, string purpose = "Registration")
    {
        if (string.IsNullOrEmpty(phoneNumber))
        {
            return Json(new { success = false, message = "شماره موبایل معتبر نیست" });
        }

        try
        {
            var user = await _userRepository.GetByPhoneNumberAsync(phoneNumber);
            if (user == null)
            {
                return Json(new { success = false, message = "کاربر یافت نشد" });
            }

            // Generate new OTP
            var otpCode = new Random().Next(1000, 9999).ToString();
            var otp = new OtpCode
            {
                UserId = user.Id,
                PhoneNumber = phoneNumber,
                Code = otpCode,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                Purpose = purpose
            };

            await _otpRepository.CreateAsync(otp);
            var smsSent = await _smsService.SendOtpAsync(phoneNumber, otpCode);

            if (smsSent)
            {
                return Json(new { success = true, message = "کد تایید مجدداً ارسال شد" });
            }
            else
            {
                return Json(new { success = false, message = "خطا در ارسال پیامک" });
            }
        }
        catch (Exception)
        {
            return Json(new { success = false, message = "خطای سیستمی رخ داده است" });
        }
    }

    private async Task EnsureRolesExist()
    {
        string[] roleNames = { "User", "Doctor" };

        foreach (var roleName in roleNames)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
} 