using SnappDoctor.Core.Entities;

namespace SnappDoctor.Application.Contracts;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByPhoneNumberAsync(string phoneNumber);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsAsync(string phoneNumber);
    Task<User> CreateAsync(User user, string password);
    Task<User> UpdateAsync(User user);
    Task<bool> CheckPasswordAsync(User user, string password);
    Task<bool> ConfirmPhoneNumberAsync(string userId);
} 