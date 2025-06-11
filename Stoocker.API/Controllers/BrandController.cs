using Microsoft.AspNetCore.Mvc;
using Stoocker.Application.Features.Commands.Brand;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Stoocker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : BaseApiController
    {
        

        [HttpGet("GetBrand/{id}")]
        public async Task<IActionResult> GetBrand(int id)
        { 

            return Ok();
        }

        [HttpPost("CreateBrand")]
        public async Task<IActionResult> CreateBrand([FromBody] CreateBrandCommand command)
        {
            var result = await Mediator.Send(command); 
            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetBrand), new { id = result.Data?.Id }, result);
        }
    }
}
