using Stoocker.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Interfaces.Repositories
{
    public interface IRepository<T> where T: BaseEntity
    {
        IQueryable<T> Query();
        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default); 
    }
}
