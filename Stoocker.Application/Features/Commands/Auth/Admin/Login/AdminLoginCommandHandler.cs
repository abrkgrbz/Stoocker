using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Stoocker.Application.DTOs.Auth.Response;
using Stoocker.Application.DTOs.Common;
using Stoocker.Application.Interfaces.Repositories;
using Stoocker.Application.Interfaces.Services;
using Stoocker.Domain.Entities;

namespace Stoocker.Application.Features.Commands.Auth.Admin.Login
{
    public class AdminLoginCommandHandler : IRequestHandler<AdminLoginCommand, Result<AdminLoginResponse>>
    {
       private readonly IAuthService _authService; 
       private readonly ILogger<AdminLoginCommandHandler> _logger;
        public AdminLoginCommandHandler(IAuthService authService, ILogger<AdminLoginCommandHandler> logger)
        {
            _authService = authService;
            _logger = logger;
        }


       public async Task<Result<AdminLoginResponse>> Handle(AdminLoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.LoginAsync(request.Email,request.Password);
                var userData = user.Data.User;
                if (user == null)
                {
                    return Result<AdminLoginResponse>.Failure("Geçersiz kullanıcı adı veya şifre");
                }
                 
                if (!userData.Roles.Any(r => r == "SuperAdmin"))
                {
                    return Result<AdminLoginResponse>.Failure("Bu alana erişim yetkiniz yok");
                }

                var response = new AdminLoginResponse()
                {
                    AdminId = userData.Id, 
                    FullName = userData.FullName, 
                    Email = userData.Email,
                    Token = user.Data.AccessToken
                };
                return Result<AdminLoginResponse>.Success(response);
            }
            catch(Exception ex)
            { 
                 _logger.LogError(ex, "Error occurred during admin login");
                return Result<AdminLoginResponse>.Failure("Bir hata oluştu. Lütfen tekrar deneyin.");
            }
        }
 
    }
}
