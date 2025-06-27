using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Stoocker.Application.DTOs.Brand.Request;
using Stoocker.Application.DTOs.Brand.Response;
using Stoocker.Application.DTOs.Common;
using Stoocker.Application.DTOs.Tenant.Request;
using Stoocker.Application.DTOs.Tenant.Response;
using Stoocker.Application.DTOs.User.Request;
using Stoocker.Application.Interfaces.Repositories;
using Stoocker.Application.Interfaces.Services;
using Stoocker.Application.Validators.Tenant;
using Stoocker.Domain.Entities;

namespace Stoocker.Application.Features.Commands.Tenant.Create
{
    public class CreateTenantCommandHandler:IRequestHandler<CreateTenantCommand, Result<TenantResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateTenantCommandHandler> _logger;  
        private readonly IEmailService _emailService;
        private readonly IValidator<CreateTenantRequest> _validator; 
        public CreateTenantCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreateTenantCommandHandler> logger, IEmailService emailService, IValidator<CreateTenantRequest> validator  )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger; 
            _emailService = emailService;
            _validator = validator; 
        }

        public async Task<Result<TenantResponse>> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);
                var requestDto = _mapper.Map<CreateTenantRequest>(request);
                var validationResult = await _validator.ValidateAsync(requestDto);
                if (!validationResult.IsValid)
                {
                    return Result<TenantResponse>.Failure(validationResult.Errors.First().ErrorMessage);
                }

                if (!string.IsNullOrWhiteSpace(request.Domain))
                {
                    var existingTenant = await _unitOfWork.TenantSpecs
                        .GetByDomainAsync(request.Domain, cancellationToken);

                    if (existingTenant != null)
                    {
                        return Result<TenantResponse>.Failure("Bu domain zaten kullanımda");
                    }
                }

                var tenant = _mapper.Map<Domain.Entities.Tenant>(requestDto);
                await _unitOfWork.GetWriteRepository<Domain.Entities.Tenant>().AddAsync(tenant);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                 
                if (request.AdminUser != null)
                {
                    var adminUser = await CreateAdminUser(tenant.Id, request.AdminUser, cancellationToken);
                    if (!adminUser.IsSuccess)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return Result<TenantResponse>.Failure(
                            $"Admin kullanıcı oluşturulamadı: {adminUser.ErrorMessage}");
                    }
                }
                 
                if (request.InitialSettings?.CreateDefaultRoles == true)
                {
                    var rolesResult = await CreateDefaultRoles(tenant.Id, request.InitialSettings.DefaultRoleNames,
                        cancellationToken);
                    if (!rolesResult.IsSuccess)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return Result<TenantResponse>.Failure($"Roller oluşturulamadı: {rolesResult.ErrorMessage}");
                    }
                }

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                var response = _mapper.Map<TenantResponse>(tenant);

                _logger.LogInformation("Yeni tenant oluşturuldu: {TenantId} - {TenantName}", tenant.Id, tenant.Name);

                return Result<TenantResponse>.Success(response);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Tenant oluşturulurken hata oluştu");
                return Result<TenantResponse>.Failure("Tenant oluşturulurken bir hata oluştu");
            }
        }

        private async Task<Result> CreateAdminUser(Guid tenantId, AdminUserSetupCommand adminSetup, CancellationToken cancellationToken)
        {
            try
            {
                var tempPassword = GenerateTemporaryPassword();
                var adminUser = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = adminSetup.Email,
                    Email = adminSetup.Email,
                    FirstName = adminSetup.FirstName,
                    LastName = adminSetup.LastName,
                    PhoneNumber = adminSetup.PhoneNumber,
                    EmailConfirmed = true,
                    TenantId = tenantId,
                    IsActive = true, 
                    PasswordHash = tempPassword
                };

                var createResult = await _unitOfWork.Users.AddAsync(adminUser, cancellationToken);
                // Hoşgeldin e-postası gönder
                if (adminSetup.SendWelcomeEmail)
                {
                    await _emailService.SendWelcomeEmailAsync(
                        adminUser.Email,
                        adminUser.FirstName,
                        tempPassword,
                        cancellationToken);
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin kullanıcı oluşturulurken hata oluştu");
                return Result.Failure("Admin kullanıcı oluşturulurken hata oluştu");
            }
        }

        private async Task<Result> CreateDefaultRoles(Guid tenantId, List<string> roleNames, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var roleName in roleNames)
                {
                    var role = new Domain.Entities.ApplicationRole
                    {
                        Id = Guid.NewGuid(),
                        Name = roleName,
                        NormalizedName = roleName.ToUpperInvariant(),
                        TenantId = tenantId,
                        IsActive = true 
                    };

                    var result = await _unitOfWork.Roles.AddAsync(role);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    if (result is null)
                    {
                        return Result.Failure($"Rol oluşturulamadı ");
                    }
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Roller oluşturulurken hata oluştu");
                return Result.Failure("Roller oluşturulurken hata oluştu");
            }
        }

        private string GenerateTemporaryPassword()
        {
            const string chars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            var password = new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
