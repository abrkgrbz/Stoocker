using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stoocker.Application.DTOs.Auth.Request;
using Stoocker.Application.Features.Commands.Auth.Admin.Login;

namespace Stoocker.API.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/admin/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : BaseAdminController
    {
        protected AuthController(ILogger<BaseAdminController> logger) : base(logger)
        {
        }

        /// <summary>
        /// Super Admin girişi
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> AdminLogin([FromBody] AdminLoginCommand request)
        {
            try
            { 
                var result = await Mediator.Send(request);

                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Failed admin login attempt for {Email}", request.Email);
                    return Unauthorized(new { message = result.ErrorMessage });
                }

                _logger.LogInformation("Successful admin login for {Email}", request.Email);

                // HttpOnly cookie ile token gönder (daha güvenli)
                Response.Cookies.Append("admin-auth-token", result.Data.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = request.RememberMe ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddHours(8)
                });

                return Ok(new
                {
                    message = "Giriş başarılı",
                    user = new
                    {
                        id = result.Data.AdminId,
                        email = result.Data.Email,
                        fullName = result.Data.FullName,
                        role = result.Data.Role
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin login error");
                return StatusCode(500, new { message = "Beklenmeyen bir hata oluştu" });
            }
        }

        /// <summary>
        /// Admin çıkışı
        /// </summary>
        [HttpPost("logout")]
        [Authorize(Policy = "SuperAdmin")]
        public IActionResult AdminLogout()
        {
            Response.Cookies.Delete("admin-auth-token");
            return Ok(new { message = "Çıkış başarılı" });
        }
 
    }
     
}
 
