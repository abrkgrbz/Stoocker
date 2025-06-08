using Stoocker.Application.Interfaces.Repositories.Specification;
using Stoocker.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Stoocker.Persistence
{
    public static class SpecificationEvaluator<T> where T : BaseEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
        {
            var query = inputQuery;
             
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }
             
            query = specification.Includes
                .Aggregate(query, (current, include) => current.Include(include));
             
            query = specification.IncludeStrings
                .Aggregate(query, (current, include) => current.Include(include));
             
            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            else if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }
             
            if (specification.IsPagingEnabled && specification.Skip.HasValue && specification.Take.HasValue)
            {
                query = query.Skip(specification.Skip.Value).Take(specification.Take.Value);
            }

            return query;
        }
    }
}
