using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Stoocker.Application.Interfaces.Repositories.Specification;
using Stoocker.Domain.Common;
using Stoocker.Persistence.Contexts;

namespace Stoocker.Persistence.Repositories
{
    public class SpecificationRepository<T> : ReadRepository<T>, ISpecificationRepository<T>
        where T : BaseEntity
    {
        public SpecificationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<T?> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            var queryWithSpec = ApplySpecification(spec);
            return await queryWithSpec.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<T>> FindAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            var queryWithSpec = ApplySpecification(spec);
            return await queryWithSpec.ToListAsync(cancellationToken);
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(Query(), spec);
        }
    }
}
