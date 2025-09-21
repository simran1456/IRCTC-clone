using System.Net;
using System.Net.Mail;
using OtpEmailSystem.Services.Interfaces;

namespace OtpEmailSystem.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendOtpEmailAsync(string toEmail, string otp, string userName)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var smtpServer = emailSettings["SmtpServer"];
                var smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587");
                var senderEmail = emailSettings["SenderEmail"];
                var senderPassword = emailSettings["SenderPassword"];
                var enableSsl = bool.Parse(emailSettings["EnableSsl"] ?? "true");

                _logger.LogInformation("Sending OTP email to {Email}", toEmail);

                using var client = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = enableSsl
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail!, "IRCTC Clone"),
                    Subject = "Verify Your Email - IRCTC Clone",
                    Body = GenerateOtpEmailBody(userName, otp),
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);
                await client.SendMailAsync(mailMessage);
                
                _logger.LogInformation("OTP email sent successfully to {Email}", toEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send OTP email to {Email}", toEmail);
                return false;
            }
        }

        public async Task<bool> SendWelcomeEmailAsync(string toEmail, string userName)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var smtpServer = emailSettings["SmtpServer"];
                var smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587");
                var senderEmail = emailSettings["SenderEmail"];
                var senderPassword = emailSettings["SenderPassword"];
                var enableSsl = bool.Parse(emailSettings["EnableSsl"] ?? "true");

                using var client = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = enableSsl
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail!, "IRCTC Clone"),
                    Subject = "Welcome to IRCTC Clone!",
                    Body = GenerateWelcomeEmailBody(userName),
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);
                await client.SendMailAsync(mailMessage);
                
                _logger.LogInformation("Welcome email sent successfully to {Email}", toEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send welcome email to {Email}", toEmail);
                return false;
            }
        }

        private static string GenerateOtpEmailBody(string userName, string otp)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5; }}
                        .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
                        .header {{ text-align: center; margin-bottom: 30px; }}
                        .logo {{ font-size: 24px; font-weight: bold; color: #ff6600; }}
                        .otp-box {{ background-color: #f8f9fa; padding: 20px; border-radius: 8px; text-align: center; margin: 20px 0; }}
                        .otp {{ font-size: 32px; font-weight: bold; color: #ff6600; letter-spacing: 5px; }}
                        .footer {{ margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; text-align: center; color: #666; font-size: 14px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <div class='logo'>🚂 IRCTC Clone</div>
                        </div>
                        
                        <h2>Email Verification Required</h2>
                        <p>Hello {userName},</p>
                        <p>Thank you for registering with IRCTC Clone! To complete your registration, please verify your email address using the OTP below:</p>
                        
                        <div class='otp-box'>
                            <div class='otp'>{otp}</div>
                            <p><strong>This OTP will expire in 10 minutes</strong></p>
                        </div>
                        
                        <p>If you didn't create an account with IRCTC Clone, please ignore this email.</p>
                        
                        <div class='footer'>
                            <p>Best regards,<br>The IRCTC Clone Team</p>
                            <p>This is an automated email. Please do not reply.</p>
                        </div>
                    </div>
                </body>
                </html>";
        }

        private static string GenerateWelcomeEmailBody(string userName)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5; }}
                        .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
                        .header {{ text-align: center; margin-bottom: 30px; }}
                        .logo {{ font-size: 24px; font-weight: bold; color: #ff6600; }}
                        .welcome-box {{ background-color: #e8f5e8; padding: 20px; border-radius: 8px; text-align: center; margin: 20px 0; }}
                        .footer {{ margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; text-align: center; color: #666; font-size: 14px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <div class='logo'>🚂 IRCTC Clone</div>
                        </div>
                        
                        <div class='welcome-box'>
                            <h2>🎉 Welcome to IRCTC Clone!</h2>
                        </div>
                        
                        <p>Hello {userName},</p>
                        <p>Your email has been successfully verified! You can now enjoy all the features of IRCTC Clone:</p>
                        
                        <ul>
                            <li>🚂 Book train tickets</li>
                            <li>📱 Manage your bookings</li>
                            <li>🔔 Get real-time notifications</li>
                        </ul>
                        
                        <p>Start your journey by logging in and exploring our services!</p>
                        
                        <div class='footer'>
                            <p>Happy travels,<br>The IRCTC Clone Team</p>
                        </div>
                    </div>
                </body>
                </html>";
        }
    }
}