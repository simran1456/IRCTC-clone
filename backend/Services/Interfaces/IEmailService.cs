namespace OtpEmailSystem.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendOtpEmailAsync(string toEmail, string otp, string userName);
        Task<bool> SendWelcomeEmailAsync(string toEmail, string userName);
    }
}