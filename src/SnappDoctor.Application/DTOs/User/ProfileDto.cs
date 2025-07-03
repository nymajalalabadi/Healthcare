using System.ComponentModel.DataAnnotations;
using SnappDoctor.Core.Enums;

namespace SnappDoctor.Application.DTOs.User;

public class ProfileDto
{
    public string Id { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "نام الزامی است")]
    [Display(Name = "نام")]
    public string FirstName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "نام خانوادگی الزامی است")]
    [Display(Name = "نام خانوادگی")]
    public string LastName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "شماره موبایل الزامی است")]
    [Display(Name = "شماره موبایل")]
    [Phone(ErrorMessage = "شماره موبایل نامعتبر است")]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [Display(Name = "ایمیل")]
    [EmailAddress(ErrorMessage = "ایمیل نامعتبر است")]
    public string? Email { get; set; }
    
    [Display(Name = "آدرس تصویر پروفایل")]
    public string? ProfilePictureUrl { get; set; }
    
    [Display(Name = "تصویر پروفایل")]
    public byte[]? ProfilePicture { get; set; }
    
    public UserType UserType { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public string FullName => $"{FirstName} {LastName}";
}

public class UpdateProfileDto
{
    [Required(ErrorMessage = "نام الزامی است")]
    [Display(Name = "نام")]
    public string FirstName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "نام خانوادگی الزامی است")]
    [Display(Name = "نام خانوادگی")]
    public string LastName { get; set; } = string.Empty;
    
    [Display(Name = "ایمیل")]
    [EmailAddress(ErrorMessage = "ایمیل نامعتبر است")]
    public string? Email { get; set; }
    
    [Display(Name = "تصویر پروفایل")]
    public byte[]? ProfilePictureData { get; set; }
    
    public string? ProfilePictureFileName { get; set; }
}

public class ChangePasswordDto
{
    [Required(ErrorMessage = "رمز عبور فعلی الزامی است")]
    [Display(Name = "رمز عبور فعلی")]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "رمز عبور جدید الزامی است")]
    [Display(Name = "رمز عبور جدید")]
    [StringLength(100, ErrorMessage = "رمز عبور باید حداقل {2} کاراکتر باشد", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "تکرار رمز عبور جدید الزامی است")]
    [Display(Name = "تکرار رمز عبور جدید")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "رمز عبور جدید و تکرار آن یکسان نیستند")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
} 