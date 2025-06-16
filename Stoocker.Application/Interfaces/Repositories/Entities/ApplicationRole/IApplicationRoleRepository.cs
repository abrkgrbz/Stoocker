using Stoocker.Application.DTOs.Common;
using Stoocker.Application.DTOs.Role.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Interfaces.Repositories.Entities.ApplicationRole
{
    public interface IApplicationRoleRepository
    {
        // Read Operations
        Task<Domain.Entities.ApplicationRole?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default);
        Task<Domain.Entities.ApplicationRole?> GetByNameAsync(string roleName, Guid tenantId, CancellationToken cancellationToken = default);
        Task<Domain.Entities.ApplicationRole?> GetWithUsersAsync(Guid roleId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Domain.Entities.ApplicationRole>> GetPagedAsync(Guid tenantId, int page = 1, int pageSize = 10,
            CancellationToken cancellationToken = default);
        Task<Domain.Entities.ApplicationRole> GetByIdAsync(Guid roleId, Guid tenantId,
            CancellationToken cancellationToken = default);

        // Tenant-specific Operations
        Task<IEnumerable<Domain.Entities.ApplicationRole>> GetAllByTenantAsync(Guid tenantId, bool includeSystemRoles = false, CancellationToken cancellationToken = default);
        Task<IEnumerable<Domain.Entities.ApplicationRole>> GetActiveRolesByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);

        // Query Operations
        IQueryable<Domain.Entities.ApplicationRole> Query();
        IQueryable<Domain.Entities.ApplicationRole> QueryByTenant(Guid tenantId);

        // Write Operations
        Task<Domain.Entities.ApplicationRole> AddAsync(Domain.Entities.ApplicationRole role, CancellationToken cancellationToken = default);
        void Update(Domain.Entities.ApplicationRole role);
        Task<bool> DeleteAsync(Guid roleId, Guid tenantId, CancellationToken cancellationToken = default);

        // Validation Operations
        Task<bool> IsRoleNameUniqueAsync(string roleName, Guid tenantId, Guid? excludeRoleId = null, CancellationToken cancellationToken = default);
        Task<bool> CanDeleteRoleAsync(Guid roleId, CancellationToken cancellationToken = default);

        Task<int> CountAsync(Expression<Func<Domain.Entities.ApplicationRole, bool>>? predicate = null,
            CancellationToken cancellationToken = default);
    }
}
