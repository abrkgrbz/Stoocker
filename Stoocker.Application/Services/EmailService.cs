using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.Interfaces.Services;

namespace Stoocker.Application.Services
{
    public class EmailService:IEmailService
    {
        public async Task SendEmailVerificationAsync(string email, string verificationCode)
        {
            throw new NotImplementedException();
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetCode)
        {
            throw new NotImplementedException();
        }
    }
}
