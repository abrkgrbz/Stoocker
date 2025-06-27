using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stoocker.Application.DTOs.Common;
using Stoocker.Application.DTOs.Tenant.Response;
using Swashbuckle.AspNetCore.Annotations;

namespace Stoocker.API.Areas.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : BaseAdminController
    {
        protected TenantsController(ILogger<BaseAdminController> logger) : base(logger)
        {
        }
        [HttpGet]
        [SwaggerOperation(
            Summary = "Tenant listesi",
            Description = "Sistemdeki tüm tenant'ları sayfalanmış olarak getirir",
            OperationId = "GetAllTenants",
            Tags = new[] { "Tenant Management" }
        )]
        [SwaggerResponse(200, "Başarılı", typeof(PagedResult<TenantResponse>))]
        [SwaggerResponse(401, "Yetkisiz erişim")]
        [SwaggerResponse(403, "Erişim reddedildi")]
        [ProducesResponseType(typeof(PagedResult<TenantResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllTenants(
            [FromQuery, SwaggerParameter("Sayfa numarası", Required = false)] int page = 1,
            [FromQuery, SwaggerParameter("Sayfa başına kayıt sayısı", Required = false)] int pageSize = 10)
        {
            return Ok();
        }
    }
}
