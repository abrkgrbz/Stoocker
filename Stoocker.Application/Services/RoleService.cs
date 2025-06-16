using Stoocker.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Stoocker.Application.DTOs.Common;
using Stoocker.Application.DTOs.Role.Request;
using Stoocker.Application.DTOs.Role.Response;
using Stoocker.Application.DTOs.User.Response;
using Microsoft.Extensions.Logging;
using Stoocker.Application.Interfaces.Repositories;
using Stoocker.Domain.Entities;

namespace Stoocker.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RoleService> _logger;
        private readonly IMapper _mapper;
        public RoleService(IUnitOfWork unitOfWork, ILogger<RoleService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<Result<RoleResponse>> GetByIdAsync(Guid roleId, Guid tenantId, CancellationToken cancellationToken = default)
        {
            try
            {
                _unitOfWork.SetCurrentTenant(tenantId);
                var role = await _unitOfWork.Roles.GetByIdAsync(roleId, tenantId, cancellationToken);

                if (role == null)
                {
                    return Result<RoleResponse>.Failure("Rol bulunamadı.");
                }

                var response = _mapper.Map<RoleResponse>(role);
                return Result<RoleResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rol getirme sırasında hata oluştu. RoleId: {RoleId}, TenantId: {TenantId}", roleId, tenantId);
                return Result<RoleResponse>.Failure("Rol bilgileri alınırken bir hata oluştu.");
            }
        }

        public async Task<Result<PagedResult<RoleResponse>>> GetRolesAsync(Guid tenantId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                _unitOfWork.SetCurrentTenant(tenantId);
                var roles = await _unitOfWork.Roles.GetPagedAsync(
                    tenantId: tenantId,
                    page: page,
                    pageSize: pageSize,
                    cancellationToken: cancellationToken);

                var responses = _mapper.Map<PagedResult<RoleResponse>>(roles);

                return Result<PagedResult<RoleResponse>>.Success(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rol listesi getirme sırasında hata oluştu. TenantId: {TenantId}, Page: {Page}, PageSize: {PageSize}", tenantId, page, pageSize);
                return Result<PagedResult<RoleResponse>>.Failure("Rol listesi alınırken bir hata oluştu.");
            }
        }

        public async Task<Result<RoleDetailResponse>> GetRoleDetailsAsync(Guid roleId, Guid tenantId, CancellationToken cancellationToken = default)
        {
            try
            {
                _unitOfWork.SetCurrentTenant(tenantId);
                var role = await _unitOfWork.Roles.GetByIdAsync(roleId, tenantId, cancellationToken);

                if (role == null)
                {
                    return Result<RoleDetailResponse>.Failure("Rol bulunamadı.");
                }

                // Get tenant info
                var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId, cancellationToken);

                // Get users assigned to this role
                var userRoles = await _unitOfWork.UserRoles.GetByRoleIdAsync(roleId, cancellationToken);
                var userIds = userRoles.Select(ur => ur.UserId).ToList();

                var users = new List<UserRoleSummary>();
                if (userIds.Any())
                {
                    var roleUsers = await _unitOfWork.Users.GetAllByTenantAsync(tenantId, true, cancellationToken);
                    users = roleUsers.Select(u => new UserRoleSummary
                    {
                        UserId = u.Id,
                        UserName = u.UserName ?? string.Empty,
                        Email = u.Email ?? string.Empty,
                        FullName = $"{u.FirstName} {u.LastName}",
                        AssignedAt = userRoles.First(ur => ur.UserId == u.Id).AssignedAt,
                        IsActive = u.IsActive
                    }).ToList();
                }

                var response = new RoleDetailResponse
                {
                    Id = role.Id,
                    Name = role.Name ?? string.Empty,
                    Description = role.Description,
                    NormalizedName = role.NormalizedName,
                    TenantId = role.TenantId,
                    TenantName = tenant?.Name ?? string.Empty,
                    IsActive = true, // Assuming roles don't have IsActive field
                    CreatedAt = role.CreatedAt,
                    UserCount = users.Count,
                    Users = users,
                    LastAssignedAt = userRoles.OrderByDescending(ur => ur.AssignedAt).FirstOrDefault()?.AssignedAt,
                    CanBeDeleted = users.Count == 0
                };

                return Result<RoleDetailResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rol detay getirme sırasında hata oluştu. RoleId: {RoleId}, TenantId: {TenantId}", roleId, tenantId);
                return Result<RoleDetailResponse>.Failure("Rol detayları alınırken bir hata oluştu.");
            }
        }

        public async Task<Result<RoleResponse>> CreateRoleAsync(CreateRoleRequest dto, Guid tenantId, CancellationToken cancellationToken = default)
        {
            try
            {
                _unitOfWork.SetCurrentTenant(tenantId);

                // Check if role name already exists in tenant
                var existingRole = await _unitOfWork.Roles.GetByNameAsync(dto.Name, tenantId, cancellationToken);

                if (existingRole != null)
                {
                    return Result<RoleResponse>.Failure("Bu isimde bir rol zaten mevcut.");
                }

                var role = new ApplicationRole
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    NormalizedName = dto.Name.ToUpperInvariant(),
                    Description = dto.Description,
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Roles.AddAsync(role, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Yeni rol oluşturuldu. RoleId: {RoleId}, Name: {Name}, TenantId: {TenantId}",
                    role.Id, role.Name, tenantId);

                var response = _mapper.Map<RoleResponse>(role);
                return Result<RoleResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rol oluşturma sırasında hata oluştu. Name: {Name}, TenantId: {TenantId}", dto.Name, tenantId);
                return Result<RoleResponse>.Failure("Rol oluşturulurken bir hata oluştu.");
            }
        }

        public async Task<Result> UpdateRoleAsync(UpdateRoleRequest dto, Guid tenantId, CancellationToken cancellationToken = default)
        {
            try
            {
                _unitOfWork.SetCurrentTenant(tenantId);
                var role = await _unitOfWork.Roles.GetByIdAsync(dto.Id, cancellationToken);

                if (role == null)
                {
                    return Result.Failure("Rol bulunamadı.");
                }

                // Check if new name already exists (excluding current role)
                if (role.Name != dto.Name)
                {
                    var existingRole = await _unitOfWork.Roles.GetByNameAsync(dto.Name, tenantId, cancellationToken);

                    if (existingRole != null)
                    {
                        return Result.Failure("Bu isimde bir rol zaten mevcut.");
                    }
                }

                role.Name = dto.Name;
                role.NormalizedName = dto.Name.ToUpperInvariant();
                role.Description = dto.Description;

                _unitOfWork.Roles.Update(role);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Rol güncellendi. RoleId: {RoleId}, TenantId: {TenantId}", dto.Id, tenantId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rol güncelleme sırasında hata oluştu. RoleId: {RoleId}, TenantId: {TenantId}", dto.Id, tenantId);
                return Result.Failure("Rol güncellenirken bir hata oluştu.");
            }
        }

        public async Task<Result> DeleteRoleAsync(Guid roleId, Guid tenantId, CancellationToken cancellationToken = default)
        {
            try
            {
                _unitOfWork.SetCurrentTenant(tenantId);
                var role = await _unitOfWork.Roles.GetByIdAsync(roleId, tenantId, cancellationToken);

                if (role == null)
                {
                    return Result.Failure("Rol bulunamadı.");
                }

                // Check if role can be deleted
                var canDelete = await CanDeleteRoleAsync(roleId, tenantId, cancellationToken);
                if (!canDelete.Data)
                {
                    return Result.Failure("Bu role atanmış kullanıcılar bulunduğu için silinemez.");
                }

                await _unitOfWork.Roles.DeleteAsync(role.Id, tenantId, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Rol silindi. RoleId: {RoleId}, TenantId: {TenantId}", roleId, tenantId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rol silme sırasında hata oluştu. RoleId: {RoleId}, TenantId: {TenantId}", roleId, tenantId);
                return Result.Failure("Rol silinirken bir hata oluştu.");
            }
        }

        public async Task<Result<List<UserResponse>>> GetRoleUsersAsync(Guid roleId, Guid tenantId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> AssignUsersToRoleAsync(Guid roleId, List<Guid> userIds, Guid tenantId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> RemoveUsersFromRoleAsync(Guid roleId, List<Guid> userIds, Guid tenantId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<bool>> RoleExistsAsync(string roleName, Guid tenantId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<bool>> CanDeleteRoleAsync(Guid roleId, Guid tenantId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<List<RoleResponse>>> GetActiveRolesAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<int>> GetRoleUserCountAsync(Guid roleId, Guid tenantId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
