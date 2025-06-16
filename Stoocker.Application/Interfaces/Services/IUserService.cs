
using Stoocker.Application.DTOs.Common;
using Stoocker.Application.DTOs.Role.Response;
using Stoocker.Application.DTOs.User.Request;
using Stoocker.Application.DTOs.User.Response;
using Stoocker.Domain.Entities;

namespace Stoocker.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<Result<UserResponse>> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Result<UserResponse>> GetByIdAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);
        Task<Result<PagedResult<UserResponse>>> GetUsersAsync(Guid tenantId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<Result<UserDetailResponse>> GetUserDetailsAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);
        Task<Result<UserResponse>> CreateUserAsync(CreateUserRequest dto, Guid tenantId, CancellationToken cancellationToken = default);
        Task<Result> UpdateUserAsync(UpdateUserRequest dto, Guid tenantId, CancellationToken cancellationToken = default);
        Task<Result> DeleteUserAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);
        Task<Result> AssignRolesToUserAsync(Guid userId, List<Guid> roleIds, Guid tenantId, CancellationToken cancellationToken = default);
        Task<Result<List<RoleResponse>>> GetUserRolesAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);
        Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
        Task<Result> ResetPasswordAsync(Guid userId, string newPassword, Guid tenantId, CancellationToken cancellationToken = default);
        Task<Result<ApplicationUser>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<Result<ApplicationUser>> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    }
}
