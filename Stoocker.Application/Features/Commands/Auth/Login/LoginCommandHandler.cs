using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Stoocker.Application.DTOs.Auth.Response;
using Stoocker.Application.DTOs.Common;
using Stoocker.Application.Interfaces.Services;

namespace Stoocker.Application.Features.Commands.Auth.Login
{
    public class LoginCommandHandler:IRequestHandler<LoginCommand, Result<LoginResponse>>
    {
        private IAuthService _authService;

        public LoginCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user= await _authService.LoginAsync(request.Email, request.Password);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            return user;
        }
    }
}
