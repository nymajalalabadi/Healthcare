using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SnappDoctor.Application.Contracts;
using SnappDoctor.Application.DTOs.Auth;
using SnappDoctor.Core.Entities;

namespace SnappDoctor.Web.Controllers;

public class AuthController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IOtpRepository _otpRepository;
    private readonly ISmsService _smsService;
    private readonly SignInManager<User> _signInManager;

    public AuthController(
        IUserRepository userRepository,
        IOtpRepository otpRepository,
        ISmsService smsService,
        SignInManager<User> signInManager)
    {
        _userRepository = userRepository;
        _otpRepository = otpRepository;
        _smsService = smsService;
        _signInManager = signInManager;
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
                PhoneNumberConfirmed = false
            };

            var createdUser = await _userRepository.CreateAsync(user, model.Password);

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
            return RedirectToAction("Index", "Home");
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
        return RedirectToAction("Index", "Home");
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
} 