using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Interfaces.Repositories.Entities.ApplicationUser
{
    public interface IApplicationUserRepository
    {
        // Read Operations
        Task<Domain.Entities.ApplicationUser?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Domain.Entities.ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<Domain.Entities.ApplicationUser?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);
        Task<Domain.Entities.ApplicationUser?> GetWithRolesAsync(Guid userId, CancellationToken cancellationToken = default);

        // Tenant-specific Read Operations
        Task<Domain.Entities.ApplicationUser?> GetByIdAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);
        Task<Domain.Entities.ApplicationUser?> GetByEmailAsync(string email, Guid tenantId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Domain.Entities.ApplicationUser>> GetAllByTenantAsync(Guid tenantId, bool includeInactive = false, CancellationToken cancellationToken = default);

        // Query Operations
        IQueryable<Domain.Entities.ApplicationUser> Query();
        IQueryable<Domain.Entities.ApplicationUser> QueryByTenant(Guid tenantId);

        // Write Operations
        Task<Domain.Entities.ApplicationUser> AddAsync(Domain.Entities.ApplicationUser user, CancellationToken cancellationToken = default);
        void Update(Domain.Entities.ApplicationUser user);
        Task<bool> SoftDeleteAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);

        // Validation Operations
        Task<bool> IsEmailUniqueAsync(string email, Guid tenantId, Guid? excludeUserId = null, CancellationToken cancellationToken = default);
        Task<bool> IsUserNameUniqueAsync(string userName, Guid tenantId, Guid? excludeUserId = null, CancellationToken cancellationToken = default);

        // Statistics
        Task<int> GetActiveUserCountAsync(Guid tenantId, CancellationToken cancellationToken = default);

        Task<int> CountAsync(Expression<Func<Domain.Entities.ApplicationUser, bool>>? predicate = null,
            CancellationToken cancellationToken = default);
    }
}
