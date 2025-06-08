using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Stoocker.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Extensions
{
    public static class DtoExtensions
    {
        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, PagedRequest request)
        {
            return query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize);
        }

        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, PagedRequest request, Dictionary<string, Expression<Func<T, object>>> sortExpressions)
        {
            if (string.IsNullOrEmpty(request.SortBy) || !sortExpressions.ContainsKey(request.SortBy))
                return query;

            var expression = sortExpressions[request.SortBy];

            return request.SortDescending
                ? query.OrderByDescending(expression)
                : query.OrderBy(expression);
        }

        public static async Task<PagedResult<TDto>> ToPagedResultAsync<TEntity, TDto>(
            this IQueryable<TEntity> query,
            PagedRequest request,
            IMapper mapper,
            CancellationToken cancellationToken = default)
        {
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .ApplyPaging(request)
                .ToListAsync(cancellationToken);

            var dtos = mapper.Map<List<TDto>>(items);

            return new PagedResult<TDto>(dtos, request.PageNumber, request.PageSize, totalCount);
        }
    }
}
