using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stoocker.Application.DTOs.Common;
using Stoocker.Application.DTOs.Tenant.Response;
using Stoocker.Application.Features.Commands.Tenant.Create;
using Swashbuckle.AspNetCore.Annotations;

namespace Stoocker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TenantController : BaseApiController
    {
        private readonly ILogger<TenantController> _logger;

        public TenantController(ILogger<TenantController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Yeni tenant oluşturur
        /// </summary>
        [HttpPost]
        [SwaggerOperation(
            Summary = "Yeni tenant oluştur",
            Description = "Yeni bir tenant ve opsiyonel olarak admin kullanıcısı oluşturur",
            OperationId = "CreateTenant"
        )]
        [SwaggerResponse(200, "Tenant başarıyla oluşturuldu", typeof(TenantResponse))]
        [SwaggerResponse(400, "Geçersiz istek")]
        [SwaggerResponse(409, "Domain zaten kullanımda")]
        public async Task<IActionResult> CreateTenant([FromBody] CreateTenantCommand command)
        {
            try
            {
                var result = await Mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return BadRequest(new { message = result.ErrorMessage });
                }

                return Ok(new
                {
                    message = "Tenant başarıyla oluşturuldu",
                    data = result.Data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tenant oluşturulurken hata oluştu");
                return StatusCode(500, new { message = "Beklenmeyen bir hata oluştu" });
            }
        }
    }
}
