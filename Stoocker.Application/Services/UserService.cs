using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stoocker.Application.DTOs.Common;
using Stoocker.Application.DTOs.Role.Response;
using Stoocker.Application.DTOs.User.Request;
using Stoocker.Application.DTOs.User.Response;
using Stoocker.Application.Interfaces.Repositories;
using Stoocker.Application.Interfaces.Services;
using Stoocker.Domain.Entities;

namespace Stoocker.Application.Services
{
    public class UserService:IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly IValidator<CreateUserRequest> _createValidator; 

        public UserService(
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            ILogger<UserService> logger,
            IValidator<CreateUserRequest> createValidator)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
        }
        public Task<Result<UserResponse>> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<UserResponse>> GetByIdAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId, tenantId, cancellationToken);
                if (user == null)
                {
                    return Result<UserResponse>.Failure("User not found");
                }

                var dto = _mapper.Map<UserResponse>(user);
                return Result<UserResponse>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {UserId} for tenant {TenantId}", userId, tenantId);
                return Result<UserResponse>.Failure("An error occurred while getting the user");
            }
        }

        public async Task<Result<PagedResult<UserResponse>>> GetUsersAsync(Guid tenantId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = _unitOfWork.Users.QueryByTenant(tenantId)
                    .Where(u => u.IsActive && !u.IsDeleted)
                    .OrderBy(u => u.FirstName)
                    .ThenBy(u => u.LastName);

                var totalCount = await query.CountAsync(cancellationToken);
                var users = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                var dtos = _mapper.Map<List<UserResponse>>(users);

                var pagedResult = new PagedResult<UserResponse> (dtos,page,pageSize,totalCount);

                return Result<PagedResult<UserResponse>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users for tenant {TenantId}", tenantId);
                return Result<PagedResult<UserResponse>>.Failure("An error occurred while getting users");
            }
        }

        public Task<Result<UserDetailResponse>> GetUserDetailsAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<UserResponse>> CreateUserAsync(CreateUserRequest dto, Guid tenantId, CancellationToken cancellationToken = default)
        {
            try
            {
                // Validation
                var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
                if (!validationResult.IsValid)
                {
                    return Result<UserResponse>.Failure(validationResult.Errors.First().ErrorMessage);
                }

                // Check tenant user limit
                var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId, cancellationToken);
                if (tenant == null)
                {
                    return Result<UserResponse>.Failure("Invalid tenant");
                }

                var currentUserCount = await _unitOfWork.Users.GetActiveUserCountAsync(tenantId, cancellationToken);
                if (currentUserCount >= tenant.MaxUsers)
                {
                    return Result<UserResponse>.Failure($"User limit reached. Maximum allowed users: {tenant.MaxUsers}");
                }

                // Check email uniqueness
                var isEmailUnique = await _unitOfWork.Users.IsEmailUniqueAsync(dto.Email, tenantId, cancellationToken: cancellationToken);
                if (!isEmailUnique)
                {
                    return Result<UserResponse>.Failure("Email already exists");
                }

                // Create user
                var user = _mapper.Map<ApplicationUser>(dto);
                user.Id = Guid.NewGuid();
                user.TenantId = tenantId;
                user.UserName = dto.Email;
                user.NormalizedUserName = dto.Email.ToUpperInvariant();
                user.NormalizedEmail = dto.Email.ToUpperInvariant();
                user.CreatedAt = DateTime.UtcNow;

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                {
                    return Result<UserResponse>.Failure(result.Errors.First().Description);
                }
                 
                if (!string.IsNullOrEmpty(dto.DefaultRole))
                {
                    await _userManager.AddToRoleAsync(user, dto.DefaultRole);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Created user {UserId} for tenant {TenantId}", user.Id, tenantId);

                var userDto = _mapper.Map<UserResponse>(user);
                return Result<UserResponse>.Success(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user for tenant {TenantId}", tenantId);
                return Result<UserResponse>.Failure("An error occurred while creating the user");
            }
        }

        public Task<Result> UpdateUserAsync(UpdateUserRequest dto, Guid tenantId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteUserAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> AssignRolesToUserAsync(Guid userId, List<Guid> roleIds, Guid tenantId, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId, tenantId, cancellationToken);
                if (user == null)
                {
                    return Result.Failure("User not found");
                }
                 
                await _unitOfWork.UserRoles.RemoveAllUserRolesAsync(userId, cancellationToken);
                 
                foreach (var roleId in roleIds)
                {
                    var role = await _unitOfWork.Roles.GetByIdAsync(roleId, cancellationToken);
                    if (role == null || role.TenantId != tenantId)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return Result.Failure($"Invalid role: {roleId}");
                    }

                    var userRole = new ApplicationUserRole
                    {
                        UserId = userId,
                        RoleId = roleId,
                        AssignedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    await _unitOfWork.UserRoles.AddAsync(userRole, cancellationToken);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Assigned {Count} roles to user {UserId}", roleIds.Count, userId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Error assigning roles to user {UserId}", userId);
                return Result.Failure("An error occurred while assigning roles");
            }
        }

        public Task<Result<List<RoleResponse>>> GetUserRolesAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result> ResetPasswordAsync(Guid userId, string newPassword, Guid tenantId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
