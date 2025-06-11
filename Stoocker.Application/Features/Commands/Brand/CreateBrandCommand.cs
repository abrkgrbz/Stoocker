using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Stoocker.Application.DTOs.Brand.Request;
using Stoocker.Application.DTOs.Brand.Response;
using Stoocker.Application.DTOs.Common;

namespace Stoocker.Application.Features.Commands.Brand
{
    public record CreateBrandCommand(
        string name,
        string? description,
        string? website,
        string? logoPath,
        string? contactEmail,
        string? contactPhone
        ) : IRequest<Result<CreateBrandResponse>>;

}
