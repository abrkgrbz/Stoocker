using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Stoocker.Application.DTOs.Brand.Response;
using Stoocker.Application.DTOs.Common;

namespace Stoocker.Application.Features.Queries.Brand.GetBrands
{
    public record GetBrandsQuery(
        int PageNumber = 1,
        int PageSize = 10,
        string? SearchTerm = null,
        string? SortBy = null,
        bool IsDescending = false) : IRequest<PagedResult<GetBrandResponse>>;
}
