using System.ComponentModel.DataAnnotations;

namespace SnappDoctor.Application.DTOs.Auth;

public class LoginDto
{
    [Required(ErrorMessage = "شماره موبایل الزامی است")]
    [RegularExpression(@"^09\d{9}$", ErrorMessage = "شماره موبایل باید با 09 شروع شده و 11 رقم باشد")]
    [Display(Name = "شماره موبایل")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "رمز عبور الزامی است")]
    [Display(Name = "رمز عبور")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "مرا به خاطر بسپار")]
    public bool RememberMe { get; set; } = false;
} 