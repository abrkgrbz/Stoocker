using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Stoocker.Application.DTOs.Brand.Response;
using Stoocker.Application.DTOs.Common;

namespace Stoocker.Application.Features.Queries.Brand.GetBrand
{
    public record GetBrandQuery(Guid Id) : IRequest<Result<GetBrandResponse>>;
}
