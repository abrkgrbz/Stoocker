using Stoocker.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Interfaces.Repositories.Entities.Tenant
{
    public interface ITenantWriteRepository<T> : IWriteRepository<T> where T : BaseEntity, ITenantEntity
    {
        // Tenant kontrolü ile silme
        Task RemoveByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
        Task<bool> BelongsToTenantAsync(Guid entityId, Guid tenantId, CancellationToken cancellationToken = default);
    }

}
