using System.ComponentModel.DataAnnotations;

namespace OtpEmailSystem.Models
{
    public class EmailVerificationOtp
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(6)]
        public string Otp { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddMinutes(10);
        
        public bool IsUsed { get; set; } = false;
        
        public DateTime? UsedAt { get; set; }
    }
}