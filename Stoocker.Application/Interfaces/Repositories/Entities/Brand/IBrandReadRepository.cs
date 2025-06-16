using Stoocker.Application.Interfaces.Repositories.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Interfaces.Repositories.Entities.Brand
{
    public interface IBrandReadRepository : IReadRepository<Domain.Entities.Brand>
    {
        Task<IEnumerable<Domain.Entities.Brand>> GetBrandsByTenantIdAsync(Guid tenantId, string searchTerm, int page = 0, int pageSize = 10, CancellationToken cancellationToken = default);

        Task<Domain.Entities.Brand> GetBrandsById(Guid Id,Guid tenantId, CancellationToken cancellationToken = default);
    }
}
