using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SnappDoctor.Application.Contracts;
using SnappDoctor.Core.Entities;
using SnappDoctor.Infrastructure.Data;

namespace SnappDoctor.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public UserRepository(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }

    public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
    {
        return await _userManager.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<bool> ExistsAsync(string phoneNumber)
    {
        return await _userManager.Users
            .AnyAsync(u => u.PhoneNumber == phoneNumber);
    }

    public async Task<User> CreateAsync(User user, string password)
    {
        user.UserName = user.PhoneNumber;
        user.Email = $"{user.PhoneNumber}@snappDoctor.com"; // Temporary email
        
        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            return user;
        }
        
        throw new InvalidOperationException($"خطا در ایجاد کاربر: {string.Join(", ", result.Errors.Select(e => e.Description))}");
    }

    public async Task<User> UpdateAsync(User user)
    {
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return user;
        }
        
        throw new InvalidOperationException($"خطا در به‌روزرسانی کاربر: {string.Join(", ", result.Errors.Select(e => e.Description))}");
    }

    public async Task<bool> CheckPasswordAsync(User user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<bool> ConfirmPhoneNumberAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            user.PhoneNumberConfirmed = true;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
        return false;
    }
} 