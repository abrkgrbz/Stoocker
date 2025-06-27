using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmailVerificationAsync(string email, string verificationCode);
        Task SendPasswordResetEmailAsync(string email, string resetCode);
        Task SendWelcomeEmailAsync(string email, string fullName,string tempPassword,CancellationToken cancellationToken);
    }
}
