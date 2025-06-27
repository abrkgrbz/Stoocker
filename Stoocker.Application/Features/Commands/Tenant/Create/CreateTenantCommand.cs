using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Stoocker.Application.DTOs.Common;
using Stoocker.Application.DTOs.Tenant.Response;

namespace Stoocker.Application.Features.Commands.Tenant.Create
{
    public sealed record CreateTenantCommand : IRequest<Result<TenantResponse>>
    {
        public string Name { get; init; } = string.Empty;
        public string? Domain { get; init; }
        public string? ContactEmail { get; init; }
        public string? ContactPhone { get; init; }
        public string PrimaryColor { get; init; } = "#3498db";
        public bool IsActive { get; init; } = true;
        public AdminUserSetupCommand? AdminUser { get; init; }
        public InitialTenantSettingsCommand? InitialSettings { get; init; }
    }

    public sealed record AdminUserSetupCommand
    {
        public string Email { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string? PhoneNumber { get; init; }
        public bool SendWelcomeEmail { get; init; } = true;
    }

    public sealed record InitialTenantSettingsCommand
    {
        public string TimeZone { get; init; } = "Europe/Istanbul";
        public string Language { get; init; } = "tr-TR";
        public string Currency { get; init; } = "TRY";
        public bool CreateDefaultRoles { get; init; } = true;
        public List<string> DefaultRoleNames { get; init; } = new() { "Admin", "User", "Manager" };
    }
}
