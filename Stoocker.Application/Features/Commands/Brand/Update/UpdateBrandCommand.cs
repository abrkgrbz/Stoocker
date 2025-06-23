using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Stoocker.Application.DTOs.Brand.Request;
using Stoocker.Application.DTOs.Common;

namespace Stoocker.Application.Features.Commands.Brand.Update
{
    public sealed record UpdateBrandCommand(
        string name,
        string? description,
        string? website,
        string? logoPath,
        string? contactEmail,
        string? contactPhone,
        Guid brandId
    ) : IRequest<Result>;

}
