using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Stoocker.Application.DTOs.Brand.Request;
using Stoocker.Application.DTOs.Tenant.Request;
using Stoocker.Application.Features.Commands.Tenant.Create;

namespace Stoocker.Application.Validators.Tenant
{
    public class CreateTenantValidator : AbstractValidator<CreateTenantRequest>
    {

        public CreateTenantValidator()
        {
            RuleFor(x => x.Name)
                  .NotEmpty().WithMessage("Tenant adı zorunludur")
                  .MaximumLength(200).WithMessage("Tenant adı maksimum 200 karakter olabilir");

            RuleFor(x => x.Domain)
                .MaximumLength(100).WithMessage("Domain maksimum 100 karakter olabilir")
                .Matches(@"^[a-zA-Z0-9][a-zA-Z0-9-]{0,61}[a-zA-Z0-9](\.[a-zA-Z0-9][a-zA-Z0-9-]{0,61}[a-zA-Z0-9])*$")
                .WithMessage("Geçerli bir domain formatı giriniz")
                .When(x => !string.IsNullOrWhiteSpace(x.Domain));

            RuleFor(x => x.ContactEmail)
                .EmailAddress().WithMessage("Geçerli bir email adresi giriniz")
                .MaximumLength(200).WithMessage("Email maksimum 200 karakter olabilir")
                .When(x => !string.IsNullOrWhiteSpace(x.ContactEmail));

            RuleFor(x => x.ContactPhone)
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Geçerli bir telefon numarası giriniz")
                .MaximumLength(20).WithMessage("Telefon numarası maksimum 20 karakter olabilir")
                .When(x => !string.IsNullOrWhiteSpace(x.ContactPhone));

            RuleFor(x => x.PrimaryColor)
                .Matches(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")
                .WithMessage("Geçerli bir hex color kodu giriniz");

            // Admin user validation
            When(x => x.AdminUser != null, () =>
            {
                RuleFor(x => x.AdminUser!.Email)
                    .NotEmpty().WithMessage("Admin email zorunludur")
                    .EmailAddress().WithMessage("Geçerli bir email adresi giriniz");

                RuleFor(x => x.AdminUser!.FirstName)
                    .NotEmpty().WithMessage("Admin adı zorunludur")
                    .MaximumLength(100).WithMessage("Ad maksimum 100 karakter olabilir");

                RuleFor(x => x.AdminUser!.LastName)
                    .NotEmpty().WithMessage("Admin soyadı zorunludur")
                    .MaximumLength(100).WithMessage("Soyad maksimum 100 karakter olabilir");

                RuleFor(x => x.AdminUser!.PhoneNumber)
                    .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Geçerli bir telefon numarası giriniz")
                    .When(x => !string.IsNullOrWhiteSpace(x.AdminUser?.PhoneNumber));
            });

            // Initial settings validation
            When(x => x.InitialSettings != null, () =>
            {
                RuleFor(x => x.InitialSettings!.TimeZone)
                    .NotEmpty().WithMessage("Zaman dilimi zorunludur");

                RuleFor(x => x.InitialSettings!.Language)
                    .NotEmpty().WithMessage("Dil zorunludur")
                    .Matches(@"^[a-z]{2}-[A-Z]{2}$").WithMessage("Geçerli bir dil kodu giriniz (örn: tr-TR)");

                RuleFor(x => x.InitialSettings!.Currency)
                    .NotEmpty().WithMessage("Para birimi zorunludur")
                    .Length(3).WithMessage("Para birimi 3 karakter olmalıdır");

                RuleFor(x => x.InitialSettings!.DefaultRoleNames)
                    .NotEmpty().WithMessage("En az bir varsayılan rol belirtilmelidir")
                    .When(x => x.InitialSettings!.CreateDefaultRoles);
            });
        }
    }
}


