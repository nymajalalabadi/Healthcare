namespace SnappDoctor.Application.Contracts;

public interface ISmsService
{
    Task<bool> SendOtpAsync(string phoneNumber, string code);
    Task<bool> SendSmsAsync(string phoneNumber, string message);
} 