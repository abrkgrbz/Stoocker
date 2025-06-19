using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stoocker.Application.Features.Commands.Auth.Login;
using Stoocker.Application.Features.Commands.Auth.Register;
using Stoocker.Application.Interfaces.Repositories;
using Stoocker.Application.Interfaces.Services;

namespace Stoocker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        public AuthController(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// User login endpoint
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>JWT token and user information</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginCommand request)
        {
            var result = await Mediator.Send(request); 
            return Ok(result);
        }

        /// <summary>
        /// User registration endpoint
        /// </summary>
        /// <param name="request">Registration information</param>
        /// <returns>Success message</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterCommand request)
        {
            var result = await Mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            return Ok(new
            {
                UserId = _currentUserService.UserId,
                Email = _currentUserService.Email,
                UserName = _currentUserService.UserName,
                Roles = _currentUserService.Roles
            });
        }

      
    }
}
