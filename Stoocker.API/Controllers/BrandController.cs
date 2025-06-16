using Microsoft.AspNetCore.Mvc;
using Stoocker.Application.Features.Commands.Brand.Create;
using Stoocker.Application.Features.Queries.Brand.GetBrand;
using Stoocker.Application.Features.Queries.Brand.GetBrands;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Stoocker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : BaseApiController
    {
        [HttpGet("GetBrand")]
        public async Task<IActionResult> GetBrand([FromQuery] GetBrandQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("GetBrands")]
        public async Task<IActionResult> GetBrands([FromQuery] GetBrandsQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("CreateBrand")]
        public async Task<IActionResult> CreateBrand([FromBody] CreateBrandCommand command)
        {
            var result = await Mediator.Send(command); 
            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetBrands), new { id = result.Data?.Id }, result);
        }
    }
}
