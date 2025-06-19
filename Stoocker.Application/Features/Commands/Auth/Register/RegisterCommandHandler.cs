using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity.Data;
using Stoocker.Application.DTOs.Common;
using Stoocker.Application.Interfaces.Services;

namespace Stoocker.Application.Features.Commands.Auth.Register
{
    public class RegisterCommandHandler:IRequestHandler<RegisterCommand,Result>
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        public RegisterCommandHandler(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
        { 
            try
            {
                var result = await _authService.RegisterAsync(request.Email, request.Password, request.UserName,
                    request.FirstName, request.LastName, request.PhoneNumber);
                return result;
            }
            catch (Exception e)
            {
               return Result.Failure("AAn error occurred during exception!");
            }
        }
    }
}
