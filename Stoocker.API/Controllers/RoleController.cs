using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stoocker.Application.Features.Commands.ApplicationRole.Create;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Stoocker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : BaseApiController
    {
        
        [HttpGet("GetAllRoles")]
        public async Task<IActionResult> GetRoles()
        {  
            return Ok();
        }

        [HttpGet("GetRoleById/{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
          
            return Ok();
        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole(CreateRoleCommand command)
        {
            var result = await Mediator.Send(command);
            if (!result.IsSuccess)
                return BadRequest(result); 
            return CreatedAtAction(nameof(GetRoleById), new { id = result.Data?.Id }, result);
             
        }

        [HttpPut("UpdateRole")]
        public async Task<IActionResult> UpdateRole()
        {
            return Ok();
        }

        [HttpDelete("DeleteRole/{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            return NoContent();
        }
    }
}
