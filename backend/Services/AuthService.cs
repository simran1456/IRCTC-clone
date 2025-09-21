using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OtpEmailSystem.Data;
using OtpEmailSystem.Models;
using OtpEmailSystem.Models.DTOs;
using OtpEmailSystem.Services.Interfaces;

namespace OtpEmailSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AuthService> _logger;
        private readonly IEmailService _emailService;
        private readonly AppDbContext _context;

        public AuthService(
            UserManager<IdentityUser> userManager,
            ILogger<AuthService> logger,
            IEmailService emailService,
            AppDbContext context)
        {
            _userManager = userManager;
            _logger = logger;
            _emailService = emailService;
            _context = context;
        }

        public async Task<ApiResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                _logger.LogInformation("Starting registration for user {Email}", request.Email);
                
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("Registration failed: User {Email} already exists", request.Email);
                    return ApiResponse.ErrorResult("User with this email already exists");
                }

                var user = new IdentityUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    EmailConfirmed = false // Require email verification
                };

                _logger.LogInformation("Creating user {Email} with Identity", request.Email);
                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    _logger.LogWarning("User creation failed for {Email}: {Errors}", request.Email, string.Join(", ", errors));
                    return ApiResponse.ErrorResult("Registration failed", errors);
                }

                // Generate OTP and save to database
                var otp = GenerateOtp();
                var otpRecord = new EmailVerificationOtp
                {
                    Email = request.Email,
                    Otp = otp,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                    IsUsed = false
                };

                _context.EmailVerificationOtps.Add(otpRecord);
                await _context.SaveChangesAsync();

                // Send OTP email
                var emailSent = await _emailService.SendOtpEmailAsync(request.Email, otp, request.Name);
                if (!emailSent)
                {
                    _logger.LogWarning("Failed to send OTP email to {Email}", request.Email);
                }

                _logger.LogInformation("User {Email} registered successfully", request.Email);
                return ApiResponse.SuccessResult("Registration successful! Please check your email for the verification OTP.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for user {Email}", request.Email);
                return ApiResponse.ErrorResult("An error occurred during registration");
            }
        }

        public async Task<ApiResponse> VerifyOtpAsync(string email, string otp)
        {
            try
            {
                _logger.LogInformation("Verifying OTP for email {Email}", email);

                var otpRecord = await _context.EmailVerificationOtps
                    .Where(o => o.Email == email && o.Otp == otp && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
                    .OrderByDescending(o => o.CreatedAt)
                    .FirstOrDefaultAsync();

                if (otpRecord == null)
                {
                    _logger.LogWarning("Invalid or expired OTP for email {Email}", email);
                    return ApiResponse.ErrorResult("Invalid or expired OTP");
                }

                // Mark OTP as used
                otpRecord.IsUsed = true;
                otpRecord.UsedAt = DateTime.UtcNow;

                // Confirm user email
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);
                }

                await _context.SaveChangesAsync();

                // Send welcome email
                await _emailService.SendWelcomeEmailAsync(email, email.Split('@')[0]);

                _logger.LogInformation("Email verified successfully for {Email}", email);
                return ApiResponse.SuccessResult("Email verified successfully! You can now login.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during OTP verification for email {Email}", email);
                return ApiResponse.ErrorResult("An error occurred during verification");
            }
        }

        public async Task<ApiResponse> ResendOtpAsync(string email)
        {
            try
            {
                _logger.LogInformation("Resending OTP for email {Email}", email);

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return ApiResponse.ErrorResult("User not found");
                }

                if (user.EmailConfirmed)
                {
                    return ApiResponse.ErrorResult("Email is already verified");
                }

                // Generate new OTP
                var otp = GenerateOtp();
                var otpRecord = new EmailVerificationOtp
                {
                    Email = email,
                    Otp = otp,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                    IsUsed = false
                };

                _context.EmailVerificationOtps.Add(otpRecord);
                await _context.SaveChangesAsync();

                // Send OTP email
                var emailSent = await _emailService.SendOtpEmailAsync(email, otp, email.Split('@')[0]);
                if (!emailSent)
                {
                    _logger.LogWarning("Failed to resend OTP email to {Email}", email);
                    return ApiResponse.ErrorResult("Failed to send OTP email");
                }

                _logger.LogInformation("OTP resent successfully to {Email}", email);
                return ApiResponse.SuccessResult("OTP sent successfully! Please check your email.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during OTP resend for email {Email}", email);
                return ApiResponse.ErrorResult("An error occurred while resending OTP");
            }
        }

        private static string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}