using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Persistence.Helpers;
using Stoocker.Persistence.Repositories.Specification;

namespace Stoocker.Persistence.Repositories.Entities.Brand.Specifications
{
    public class BrandSearchSpecification:BaseSpecification<Domain.Entities.Brand>
    {
        public BrandSearchSpecification(Guid tenantId, string searchTerm, int page, int pageSize)
        { 
            Expression<Func<Domain.Entities.Brand, bool>> criteria = b => b.TenantId == tenantId;
             
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchLower = searchTerm.ToLower();
                Expression<Func<Domain.Entities.Brand, bool>> searchCriteria = b =>
                    b.Name != null && b.Name.ToLower().Contains(searchLower);
                 
                criteria = CombineWithAnd(criteria, searchCriteria);
            }

            Criteria = criteria;

            // Pagination (page 1-based)
            if (page > 0 && pageSize > 0)
            {
                ApplyPaging((page - 1) * pageSize, pageSize);
            }

            // Default ordering
            ApplyOrderBy(b => b.Name);
        }

        private Expression<Func<Domain.Entities.Brand, bool>> CombineWithAnd(
            Expression<Func<Domain.Entities.Brand, bool>> first,
            Expression<Func<Domain.Entities.Brand, bool>> second)
        {
            var parameter = Expression.Parameter(typeof(Domain.Entities.Brand), "b");

            var firstBody = new ParameterReplacer(first.Parameters[0], parameter).Visit(first.Body);
            var secondBody = new ParameterReplacer(second.Parameters[0], parameter).Visit(second.Body);

            var combined = Expression.AndAlso(firstBody, secondBody);

            return Expression.Lambda<Func<Domain.Entities.Brand, bool>>(combined, parameter);
        }
    }
}
