using Microsoft.AspNetCore.Mvc;
using OtpEmailSystem.Models.DTOs;
using OtpEmailSystem.Services.Interfaces;

namespace OtpEmailSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// User registration with OTP email verification
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse>> Register([FromBody] RegisterRequest request)
        {
            _logger.LogInformation("Registration request received for email: {Email}", request?.Email);
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                _logger.LogWarning("Registration validation failed: {Errors}", string.Join(", ", errors));
                return BadRequest(ApiResponse.ErrorResult("Validation failed", errors));
            }

            var result = await _authService.RegisterAsync(request);

            if (!result.Success)
            {
                _logger.LogWarning("Registration failed for email {Email}: {Message}", request.Email, result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Registration successful for email: {Email}", request.Email);
            return Ok(result);
        }

        /// <summary>
        /// Verify OTP for email confirmation
        /// </summary>
        [HttpPost("verify-otp")]
        public async Task<ActionResult<ApiResponse>> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            _logger.LogInformation("OTP verification request for email: {Email}", request?.Email);
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse.ErrorResult("Validation failed", errors));
            }

            var result = await _authService.VerifyOtpAsync(request.Email, request.Otp);

            if (!result.Success)
            {
                _logger.LogWarning("OTP verification failed for email {Email}: {Message}", request.Email, result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("OTP verification successful for email: {Email}", request.Email);
            return Ok(result);
        }

        /// <summary>
        /// Resend OTP for email verification
        /// </summary>
        [HttpPost("resend-otp")]
        public async Task<ActionResult<ApiResponse>> ResendOtp([FromBody] ResendOtpRequest request)
        {
            _logger.LogInformation("OTP resend request for email: {Email}", request?.Email);
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse.ErrorResult("Validation failed", errors));
            }

            var result = await _authService.ResendOtpAsync(request.Email);

            if (!result.Success)
            {
                _logger.LogWarning("OTP resend failed for email {Email}: {Message}", request.Email, result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("OTP resend successful for email: {Email}", request.Email);
            return Ok(result);
        }
    }

    public class VerifyOtpRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string Otp { get; set; } = string.Empty;
    }

    public class ResendOtpRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}