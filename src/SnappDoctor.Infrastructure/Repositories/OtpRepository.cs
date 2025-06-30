using Microsoft.EntityFrameworkCore;
using SnappDoctor.Application.Contracts;
using SnappDoctor.Core.Entities;
using SnappDoctor.Infrastructure.Data;

namespace SnappDoctor.Infrastructure.Repositories;

public class OtpRepository : IOtpRepository
{
    private readonly ApplicationDbContext _context;

    public OtpRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OtpCode?> GetValidOtpAsync(string phoneNumber, string code, string purpose)
    {
        return await _context.OtpCodes
            .FirstOrDefaultAsync(o => 
                o.PhoneNumber == phoneNumber && 
                o.Code == code && 
                o.Purpose == purpose &&
                !o.IsUsed && 
                o.ExpiresAt > DateTime.UtcNow);
    }

    public async Task<OtpCode> CreateAsync(OtpCode otpCode)
    {
        _context.OtpCodes.Add(otpCode);
        await _context.SaveChangesAsync();
        return otpCode;
    }

    public async Task<OtpCode> UpdateAsync(OtpCode otpCode)
    {
        _context.OtpCodes.Update(otpCode);
        await _context.SaveChangesAsync();
        return otpCode;
    }

    public async Task<bool> MarkAsUsedAsync(int id)
    {
        var otp = await _context.OtpCodes.FindAsync(id);
        if (otp != null)
        {
            otp.IsUsed = true;
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteExpiredAsync()
    {
        var expiredOtps = await _context.OtpCodes
            .Where(o => o.ExpiresAt <= DateTime.UtcNow || o.IsUsed)
            .ToListAsync();

        if (expiredOtps.Any())
        {
            _context.OtpCodes.RemoveRange(expiredOtps);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }
} 