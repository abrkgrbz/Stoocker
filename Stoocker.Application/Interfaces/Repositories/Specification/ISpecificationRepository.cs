using Stoocker.Application.Interfaces.Repositories;
using Stoocker.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Interfaces.Repositories.Specification
{
    public interface ISpecificationRepository<T> : IReadRepository<T> where T : BaseEntity
    {
        Task<T?> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> FindAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
    }
}
