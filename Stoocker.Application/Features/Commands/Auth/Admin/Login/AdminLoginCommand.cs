using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Stoocker.Application.DTOs.Auth.Response;
using Stoocker.Application.DTOs.Common;

namespace Stoocker.Application.Features.Commands.Auth.Admin.Login
{
    public class AdminLoginCommand:IRequest<Result<AdminLoginResponse>>
    {
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public bool RememberMe { get; init; }
    }
}
