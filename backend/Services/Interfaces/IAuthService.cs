using OtpEmailSystem.Models.DTOs;

namespace OtpEmailSystem.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse> RegisterAsync(RegisterRequest request);
        Task<ApiResponse> VerifyOtpAsync(string email, string otp);
        Task<ApiResponse> ResendOtpAsync(string email);
    }
}