# OTP Email Verification System 📧

A minimal .NET 8 Web API system for email verification using Gmail SMTP and 6-digit OTP codes.

## 🚀 Features

- **User Registration** with email verification
- **6-digit OTP** generation and validation
- **Gmail SMTP** integration for email sending
- **Professional HTML emails** with OTP and welcome templates
- **10-minute OTP expiration** for security
- **Resend OTP** functionality
- **Entity Framework Core** with SQL Server

## 📋 Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB or full version)
- Gmail account with App Password

## ⚙️ Setup

### 1. Gmail Configuration
1. Enable 2-Factor Authentication on your Gmail account
2. Generate an App Password:
   - Go to Google Account Settings → Security → 2-Step Verification → App passwords
   - Generate password for "Mail"
3. Update `appsettings.json`:
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "your-16-digit-app-password",
    "EnableSsl": true
  }
}
```

### 2. Database Setup
```bash
# Install EF Core tools (if not installed)
dotnet tool install --global dotnet-ef

# Create migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update
```

### 3. Run the Application
```bash
dotnet restore
dotnet run
```

## 📡 API Endpoints

### Register User
```http
POST /api/auth/register
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john@example.com",
  "password": "Password123!",
  "phone": "1234567890",
  "age": 25,
  "dateOfBirth": "1999-01-01",
  "gender": "Male"
}
```

### Verify OTP
```http
POST /api/auth/verify-otp
Content-Type: application/json

{
  "email": "john@example.com",
  "otp": "123456"
}
```

### Resend OTP
```http
POST /api/auth/resend-otp
Content-Type: application/json

{
  "email": "john@example.com"
}
```

## 🔧 Integration Guide

### 1. Add to Existing Project

Copy these files to your project:
- `Models/EmailVerificationOtp.cs`
- `Services/EmailService.cs` & `Services/Interfaces/IEmailService.cs`
- `Controllers/AuthController.cs` (merge with existing)
- Update your `DbContext` to include `EmailVerificationOtps`

### 2. Register Services in Program.cs
```csharp
builder.Services.AddScoped<IEmailService, EmailService>();
```

### 3. Update appsettings.json
Add the `EmailSettings` section with your Gmail credentials.

### 4. Database Migration
```bash
dotnet ef migrations add AddEmailVerificationOtp
dotnet ef database update
```

## 🎯 How It Works

1. **User Registration**: User submits registration form
2. **OTP Generation**: System generates 6-digit OTP with 10-minute expiration
3. **Email Sending**: Professional HTML email sent via Gmail SMTP
4. **OTP Verification**: User enters OTP to verify email
5. **Account Activation**: Email confirmed, welcome email sent

## 🔒 Security Features

- OTP expires in 10 minutes
- One-time use OTP (marked as used after verification)
- Email confirmation required before login
- Secure Gmail SMTP with App Password
- Input validation and error handling

## 📧 Email Templates

- **OTP Email**: Professional template with company branding
- **Welcome Email**: Sent after successful verification
- **HTML Format**: Responsive design with inline CSS

## 🛠️ Customization

- Change OTP expiration time in `AuthService.cs`
- Modify email templates in `EmailService.cs`
- Update branding and colors in HTML templates
- Add additional validation rules as needed

## 📝 Notes

- Uses Entity Framework Core with SQL Server
- Compatible with .NET 8
- Includes Swagger documentation
- CORS enabled for frontend integration
- Comprehensive logging for debugging

---
*Ready to integrate into your IRCTC clone or any .NET application!* 🚂