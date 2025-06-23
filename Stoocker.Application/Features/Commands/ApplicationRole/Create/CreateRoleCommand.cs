using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Stoocker.Application.DTOs.Common;
using Stoocker.Application.DTOs.Role.Response;

namespace Stoocker.Application.Features.Commands.ApplicationRole.Create
{
    public record CreateRoleCommand(
        Guid tenantId,
        string description,
        string isSystemRole,
        string name
        ) : IRequest<Result<RoleResponse>>;
}
