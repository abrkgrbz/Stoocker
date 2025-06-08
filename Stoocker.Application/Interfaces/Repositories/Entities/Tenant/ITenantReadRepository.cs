using Stoocker.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Interfaces.Repositories.Entities.Tenant
{
    public interface ITenantReadRepository<T> : IReadRepository<T> where T : BaseEntity, ITenantEntity
    {
        // Tenant filtrelemeli metodlar
        Task<T?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, Guid tenantId, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> FindByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, Guid tenantId, CancellationToken cancellationToken = default);
        IQueryable<T> QueryByTenant(Guid tenantId);
    }
}
