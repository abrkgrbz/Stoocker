using Stoocker.Application.Interfaces.Repositories.Entities.Tenant;
using Stoocker.Domain.Common;
using Stoocker.Persistence.Contexts; 
using System.Linq.Expressions; 
using Microsoft.EntityFrameworkCore; 
using Stoocker.Persistence.Repositories.Specification;
using Stoocker.Persistence.Repositories.Entities.Brand.Specifications;

namespace Stoocker.Persistence.Repositories.Entities.Tenant
{
    public class TenantReadRepository<T> : SpecificationRepository<T>, ITenantReadRepository<T>
        where T : BaseEntity,ITenantEntity
    {
        public TenantReadRepository(ApplicationDbContext context) : base(context) { }

        public async Task<T?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(e => e.Id == id && e.TenantId == tenantId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, Guid tenantId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(e => e.TenantId == tenantId)
                .Where(predicate)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<T>> FindByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(e => e.TenantId == tenantId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, Guid tenantId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(e => e.TenantId == tenantId)
                .Where(predicate)
                .ToListAsync(cancellationToken);
        }
         
        public IQueryable<T> QueryByTenant(Guid tenantId)
        {
            return _dbSet.Where(e => e.TenantId == tenantId);
        }
        
    }
}
