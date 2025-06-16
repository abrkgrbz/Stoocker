using Stoocker.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Interfaces.Repositories
{
    public interface IWriteRepository<T> where T : BaseEntity
    {
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        void Update(T entity, CancellationToken cancellationToken = default);
        void UpdateRange(IEnumerable<T> entities);
        void Remove(T entity, CancellationToken cancellationToken = default);
        void RemoveRange(IEnumerable<T> entities);
        Task RemoveByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
