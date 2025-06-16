using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.Interfaces.Repositories.Specification;

namespace Stoocker.Application.Interfaces.Repositories.Entities.Tenant
{
    public interface ITenantSpecReadRepository: IReadRepository<Domain.Entities.Tenant>
    {
        Task<IEnumerable<Domain.Entities.Tenant>> GetPagedAsync(Guid tenantId, int page = 1, int pageSize = 10,
            CancellationToken cancellationToken = default);
    }
}
