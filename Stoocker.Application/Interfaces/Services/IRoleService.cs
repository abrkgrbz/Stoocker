using Stoocker.Application.DTOs.Common;
using Stoocker.Application.DTOs.Role.Request;
using Stoocker.Application.DTOs.Role.Response;
using Stoocker.Application.DTOs.User.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Interfaces.Services
{
    public interface IRoleService

    {   // Temel CRUD operasyonları
        Task<Result<RoleResponse>> GetByIdAsync(Guid roleId, Guid tenantId, CancellationToken cancellationToken = default);
        Task<Result<PagedResult<RoleResponse>>> GetRolesAsync(Guid tenantId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<Result<RoleDetailResponse>> GetRoleDetailsAsync(Guid roleId, Guid tenantId, CancellationToken cancellationToken = default);
        Task<Result<RoleResponse>> CreateRoleAsync(CreateRoleRequest dto, Guid tenantId, CancellationToken cancellationToken = default);
        Task<Result> UpdateRoleAsync(UpdateRoleRequest dto, Guid tenantId, CancellationToken cancellationToken = default);
        Task<Result> DeleteRoleAsync(Guid roleId, Guid tenantId, CancellationToken cancellationToken = default);

        // Role-User ilişkileri
        Task<Result<List<UserResponse>>> GetRoleUsersAsync(Guid roleId, Guid tenantId, CancellationToken cancellationToken = default);
        Task<Result> AssignUsersToRoleAsync(Guid roleId, List<Guid> userIds, Guid tenantId, CancellationToken cancellationToken = default);
        Task<Result> RemoveUsersFromRoleAsync(Guid roleId, List<Guid> userIds, Guid tenantId, CancellationToken cancellationToken = default);

        // Yardımcı metodlar
        Task<Result<bool>> RoleExistsAsync(string roleName, Guid tenantId, CancellationToken cancellationToken = default);
        Task<Result<bool>> CanDeleteRoleAsync(Guid roleId, Guid tenantId, CancellationToken cancellationToken = default);
        Task<Result<List<RoleResponse>>> GetActiveRolesAsync(Guid tenantId, CancellationToken cancellationToken = default);
        Task<Result<int>> GetRoleUserCountAsync(Guid roleId, Guid tenantId, CancellationToken cancellationToken = default);

    }
}
