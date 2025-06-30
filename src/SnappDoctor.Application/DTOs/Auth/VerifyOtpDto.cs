using System.ComponentModel.DataAnnotations;

namespace SnappDoctor.Application.DTOs.Auth;

public class VerifyOtpDto
{
    [Required(ErrorMessage = "شماره موبایل الزامی است")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "کد تایید الزامی است")]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "کد تایید باید 4 رقم باشد")]
    [Display(Name = "کد تایید")]
    public string Code { get; set; } = string.Empty;

    public string Purpose { get; set; } = "Registration";
} 