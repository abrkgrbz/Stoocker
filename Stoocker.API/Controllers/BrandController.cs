using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stoocker.API.Attributes;
using Stoocker.Application.Features.Commands.Brand.Create;
using Stoocker.Application.Features.Queries.Brand.GetBrand;
using Stoocker.Application.Features.Queries.Brand.GetBrands;
using Stoocker.Domain.Constants;


namespace Stoocker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BrandController : BaseApiController
    {
        [HttpGet("GetBrand")]
        [RequirePermission(PermissionConstants.BrandView)]
        public async Task<IActionResult> GetBrand([FromQuery] GetBrandQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("GetBrands")]
        [RequirePermission(PermissionConstants.BrandView)]
        public async Task<IActionResult> GetBrands([FromQuery] GetBrandsQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("CreateBrand")]
        [RequirePermission(PermissionConstants.BrandCreate)]
        public async Task<IActionResult> CreateBrand([FromBody] CreateBrandCommand command)
        {
            var result = await Mediator.Send(command); 
            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetBrands), new { id = result.Data?.Id }, result);
        }
    }
}
