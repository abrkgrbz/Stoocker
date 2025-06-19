using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Stoocker.Application.DTOs.Auth.Response;
using Stoocker.Application.DTOs.Common;

namespace Stoocker.Application.Features.Commands.Auth.Login
{
    public class LoginCommand:IRequest<Result<LoginResponse>>
    {
        public string Email { get; set; }
        public string Password { get; set; } 
    }
}
