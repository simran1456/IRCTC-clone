# 🚀 Quick Integration Guide for IRCTC Clone

## 📧 Gmail Setup (CRITICAL FIRST STEP)

1. **Enable 2-Factor Authentication** on Gmail
2. **Generate App Password**:
   - Google Account → Security → 2-Step Verification → App passwords
   - Select "Mail" → Generate 16-digit password
3. **Update appsettings.json**:
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "abcd efgh ijkl mnop",
    "EnableSsl": true
  }
}
```

## 🔧 Integration Steps

### 1. Copy Files to Your Project
```
Models/EmailVerificationOtp.cs
Services/EmailService.cs
Services/Interfaces/IEmailService.cs
Services/AuthService.cs (merge with existing)
Controllers/AuthController.cs (merge with existing)
```

### 2. Update Your DbContext
```csharp
public DbSet<EmailVerificationOtp> EmailVerificationOtps { get; set; }
```

### 3. Register Services in Program.cs
```csharp
builder.Services.AddScoped<IEmailService, EmailService>();
```

### 4. Database Migration
```bash
dotnet ef migrations add AddEmailVerificationOtp
dotnet ef database update
```

## 📡 API Usage

### Registration Flow
```javascript
// 1. Register user
POST /api/auth/register
{
  "name": "John Doe",
  "email": "john@example.com",
  "password": "Password123!",
  "phone": "1234567890",
  "age": 25
}

// 2. Verify OTP (user receives email)
POST /api/auth/verify-otp
{
  "email": "john@example.com",
  "otp": "123456"
}

// 3. Resend OTP if needed
POST /api/auth/resend-otp
{
  "email": "john@example.com"
}
```

## 🎯 Key Features
- ✅ 6-digit OTP with 10-minute expiration
- ✅ Professional HTML email templates
- ✅ Gmail SMTP integration
- ✅ Welcome email after verification
- ✅ Resend OTP functionality
- ✅ One-time use security

## 🔒 Security Notes
- OTP expires in 10 minutes
- Email confirmation required before login
- OTP marked as used after verification
- Secure Gmail App Password authentication

## 🛠️ Customization
- Change OTP expiration: `DateTime.UtcNow.AddMinutes(10)`
- Update email templates in `EmailService.cs`
- Modify branding colors and logos

## 📞 Support
- Check logs for SMTP connection issues
- Verify Gmail App Password is correct
- Ensure 2FA is enabled on Gmail account

---
*Ready to integrate! 🚂 The system is production-ready with proper error handling and logging.*