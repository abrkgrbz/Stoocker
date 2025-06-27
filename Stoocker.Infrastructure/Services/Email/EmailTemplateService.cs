using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Infrastructure.Services.Email
{
    public interface IEmailTemplateService
    {
        Task<string> GetTemplateAsync(string templateName, object model);
    }

    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IWebHostEnvironment _environment;

        public EmailTemplateService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> GetTemplateAsync(string templateName, object model)
        {
            var templatePath = Path.Combine(_environment.ContentRootPath,
                "EmailTemplates", $"{templateName}.html");

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Email template '{templateName}' not found at {templatePath}");
            }

            var template = await File.ReadAllTextAsync(templatePath);

            // Model properties ile template'deki placeholder'ları değiştir
            foreach (var prop in model.GetType().GetProperties())
            {
                var placeholder = $"{{{{{prop.Name}}}}}";
                var value = prop.GetValue(model)?.ToString() ?? string.Empty;
                template = template.Replace(placeholder, value);
            }

            return template;
        }
    }
}
