  
namespace Stoocker.Application.Interfaces.Repositories.Entities.ApplicationUserRole
{
    public interface IApplicationUserRoleRepository
    {
        // Read Operations
        Task<Domain.Entities.ApplicationUserRole?> GetAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Domain.Entities.ApplicationUserRole>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Domain.Entities.ApplicationUserRole>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Domain.Entities.ApplicationUserRole>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        // Include Operations
        Task<IEnumerable<Domain.Entities.ApplicationUserRole>> GetByUserIdWithRoleAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Domain.Entities.ApplicationUserRole>> GetByRoleIdWithUserAsync(Guid roleId, CancellationToken cancellationToken = default);

        // Write Operations
        Task<Domain.Entities.ApplicationUserRole> AddAsync(Domain.Entities.ApplicationUserRole userRole, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<Domain.Entities.ApplicationUserRole> userRoles, CancellationToken cancellationToken = default);
        void Update(Domain.Entities.ApplicationUserRole userRole);
        Task<bool> RemoveAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
        Task RemoveAllUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
        Task RemoveAllRoleUsersAsync(Guid roleId, CancellationToken cancellationToken = default);

        // Validation Operations
        Task<bool> UserHasRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
        Task<bool> UserHasRoleAsync(Guid userId, string roleName, Guid tenantId, CancellationToken cancellationToken = default);
        Task<int> GetUserCountInRoleAsync(Guid roleId, CancellationToken cancellationToken = default);

        // Query Operations
        IQueryable<Domain.Entities.ApplicationUserRole> Query();
    }
}
