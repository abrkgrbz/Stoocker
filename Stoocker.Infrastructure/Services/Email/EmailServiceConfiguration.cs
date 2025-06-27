using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stoocker.Application.Interfaces.Services;
using Stoocker.Infrastructure.Services.Email;

namespace Stoocker.Infrastructure.Services
{
    public static class EmailServiceConfiguration
    {
        public static IServiceCollection AddEmailServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            services.AddScoped<IEmailService, EmailService>();
            return services;
        }
    }
}
