using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Stoocker.Application.DTOs.User.Request;
using Stoocker.Application.Interfaces.Repositories;

namespace Stoocker.Application.Validators.User
{
    public class CreateUserValidator:AbstractValidator<CreateUserRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MustAsync(BeUniqueEmail).WithMessage("Email already exists");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters")
                .Matches(@"^[a-zA-ZğüşöçıİĞÜŞÖÇ\s]+$").WithMessage("First name can only contain letters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters")
                .Matches(@"^[a-zA-ZğüşöçıİĞÜŞÖÇ\s]+$").WithMessage("Last name can only contain letters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number")
                .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$").When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("Invalid phone number format");

            RuleFor(x => x.TimeZone)
                .Must(BeValidTimeZone).When(x => !string.IsNullOrEmpty(x.TimeZone))
                .WithMessage("Invalid timezone");

            RuleFor(x => x.RoleIds)
                .MustAsync(BeValidRoles).When(x => x.RoleIds != null && x.RoleIds.Any())
                .WithMessage("One or more invalid roles");
        }

        private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
        {
            // Bu metod service layer'da tenant bilgisi ile kontrol edilmeli
            var tenantId = _unitOfWork.GetCurrentTenant().Value;
            var isUniqueEmail = await _unitOfWork.Users.IsEmailUniqueAsync(email, tenantId, null, cancellationToken);
            return isUniqueEmail;
        }

        private bool BeValidTimeZone(string? timeZone)
        {
            try
            {
                TimeZoneInfo.FindSystemTimeZoneById(timeZone ?? "");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> BeValidRoles(List<Guid>? roleIds, CancellationToken cancellationToken)
        {
            // Bu metod service layer'da tenant bilgisi ile kontrol edilmeli
            return true;
        }
    }
}

