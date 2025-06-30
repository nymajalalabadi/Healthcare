using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using SnappDoctor.Application.Contracts;

namespace SnappDoctor.Infrastructure.Services;

public class KavenegarSmsService : ISmsService
{
    private readonly IConfiguration _configuration;
    private readonly string _apiKey;
    private readonly string _baseUrl = "https://api.kavenegar.com/v1";

    public KavenegarSmsService(IConfiguration configuration)
    {
        _configuration = configuration;
        _apiKey = _configuration["Kavenegar:ApiKey"] ?? throw new ArgumentNullException("Kavenegar API Key is not configured");
    }

    public async Task<bool> SendOtpAsync(string phoneNumber, string code)
    {
        try
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest($"/{_apiKey}/verify/lookup.json", Method.Post);
            
            request.AddParameter("receptor", phoneNumber);
            request.AddParameter("token", code);
            request.AddParameter("template", "verify"); // You need to create this template in Kavenegar panel
            
            var response = await client.ExecuteAsync(request);
            
            if (response.IsSuccessful && response.Content != null)
            {
                var result = JsonConvert.DeserializeObject<KavenegarResponse>(response.Content);
                return result?.Return?.Status == 200;
            }
            
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> SendSmsAsync(string phoneNumber, string message)
    {
        try
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest($"/{_apiKey}/sms/send.json", Method.Post);
            
            request.AddParameter("receptor", phoneNumber);
            request.AddParameter("message", message);
            request.AddParameter("sender", "1000596446"); // Your Kavenegar sender number
            
            var response = await client.ExecuteAsync(request);
            
            if (response.IsSuccessful && response.Content != null)
            {
                var result = JsonConvert.DeserializeObject<KavenegarResponse>(response.Content);
                return result?.Return?.Status == 200;
            }
            
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }
}

public class KavenegarResponse
{
    public KavenegarReturn Return { get; set; } = new();
}

public class KavenegarReturn
{
    public int Status { get; set; }
    public string Message { get; set; } = string.Empty;
} 