using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Stoocker.Application.Interfaces.Repositories.Entities.Tenant;
using Stoocker.Domain.Common;
using Stoocker.Persistence.Contexts;

namespace Stoocker.Persistence.Repositories.Entities.Tenant
{
    public class TenantWriteRepository<T> : WriteRepository<T>, ITenantWriteRepository<T>
        where T : BaseEntity, ITenantEntity
    { 
        public TenantWriteRepository(ApplicationDbContext context) : base(context)
        { 
        }

        public async Task RemoveByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
        {
            var entity = await _dbSet
                .FirstOrDefaultAsync(e => e.Id == id && e.TenantId == tenantId, cancellationToken);

            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public async Task<bool> BelongsToTenantAsync(Guid entityId, Guid tenantId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AnyAsync(e => e.Id == entityId && e.TenantId == tenantId, cancellationToken); 
        }
    }
}
