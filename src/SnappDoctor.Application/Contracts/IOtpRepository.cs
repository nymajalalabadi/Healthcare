using SnappDoctor.Core.Entities;

namespace SnappDoctor.Application.Contracts;

public interface IOtpRepository
{
    Task<OtpCode?> GetValidOtpAsync(string phoneNumber, string code, string purpose);
    Task<OtpCode> CreateAsync(OtpCode otpCode);
    Task<OtpCode> UpdateAsync(OtpCode otpCode);
    Task<bool> MarkAsUsedAsync(int id);
    Task<bool> DeleteExpiredAsync();
} 