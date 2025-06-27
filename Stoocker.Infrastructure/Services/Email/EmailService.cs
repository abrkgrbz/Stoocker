using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Stoocker.Application.Interfaces.Services;

namespace Stoocker.Infrastructure.Services.Email
{
    public class EmailService:IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;
        private readonly IEmailTemplateService _emailTemplateService;
        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger, IEmailTemplateService emailTemplateService)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
            _emailTemplateService = emailTemplateService;
        }

        public async Task SendEmailVerificationAsync(string email, string verificationCode)
        {
            var subject = "Email Doğrulama - Stoocker";

            var templateModel = new
            {
                VerificationCode = verificationCode,
                ExpiryMinutes = 15
            };
            var htmlBody = await _emailTemplateService.GetTemplateAsync("EmailVerification", templateModel);
            await SendEmailAsync(email, subject, htmlBody);
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetCode)
        {
            var subject = "Şifre Sıfırlama - Stoocker";

            var templateModel = new
            {
                ResetCode = resetCode,
                ExpiryMinutes = 30
            };

            var htmlBody = await _emailTemplateService.GetTemplateAsync("PasswordReset", templateModel);
            await SendEmailAsync(email, subject, htmlBody);
        }

        public async Task SendWelcomeEmailAsync(string email, string fullName, string tempPassword, CancellationToken cancellationToken)
        {
            var subject = "Stoocker'a Hoşgeldiniz - Stoocker";

            var templateModel = new
            {
                TempPassword=tempPassword,
                FullName=fullName,
                Email=email,
                ExpiryMinutes = 30
            };

            var htmlBody = await _emailTemplateService.GetTemplateAsync("WelcomeEmail", templateModel);
            await SendEmailAsync(email, subject, htmlBody);
        }

        private async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
                message.To.Add(new MailboxAddress(to, to));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlBody
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port,
                    _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

                await client.AuthenticateAsync(_emailSettings.UserName, _emailSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Email successfully sent to {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email to {to}");
                throw new ApplicationException($"Email gönderilemedi: {ex.Message}", ex);
            }
        }
    }
}
