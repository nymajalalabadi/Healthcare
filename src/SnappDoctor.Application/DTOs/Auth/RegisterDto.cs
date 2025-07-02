using System.ComponentModel.DataAnnotations;
using SnappDoctor.Core.Enums;

namespace SnappDoctor.Application.DTOs.Auth;

public class RegisterDto
{
    [Required(ErrorMessage = "نام الزامی است")]
    [Display(Name = "نام")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "نام خانوادگی الزامی است")]
    [Display(Name = "نام خانوادگی")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "شماره موبایل الزامی است")]
    [RegularExpression(@"^09\d{9}$", ErrorMessage = "شماره موبایل باید با 09 شروع شده و 11 رقم باشد")]
    [Display(Name = "شماره موبایل")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "رمز عبور الزامی است")]
    [MinLength(6, ErrorMessage = "رمز عبور باید حداقل 6 کاراکتر باشد")]
    [Display(Name = "رمز عبور")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "تکرار رمز عبور الزامی است")]
    [Compare("Password", ErrorMessage = "رمز عبور و تکرار آن یکسان نیستند")]
    [Display(Name = "تکرار رمز عبور")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "نوع کاربری الزامی است")]
    [Display(Name = "نوع کاربری")]
    public UserType UserType { get; set; } = UserType.RegularUser;
} 