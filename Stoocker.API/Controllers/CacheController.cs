using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stoocker.Application.Interfaces.Services.Caching;

namespace Stoocker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CacheController : ControllerBase
    {
        private readonly ICacheService _cacheService;
        private readonly ICacheStatistics _cacheStatistics;

        public CacheController(ICacheService cacheService, ICacheStatistics cacheStatistics)
        {
            _cacheService = cacheService;
            _cacheStatistics = cacheStatistics;
        }

        [HttpGet("statistics")]
        public IActionResult GetStatistics()
        {
            var stats = _cacheStatistics.GetSnapshot();
            return Ok(stats);
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCache()
        {
            if (_cacheService is IMemoryCacheService memoryCacheService)
            {
                memoryCacheService.Clear();
                return Ok(new { message = "Memory cache cleared successfully" });
            }

            return BadRequest(new { message = "Clear operation is only supported for memory cache" });
        }

        [HttpDelete("key/{key}")]
        public async Task<IActionResult> RemoveKey(string key)
        {
            await _cacheService.RemoveAsync(key);
            return Ok(new { message = $"Key '{key}' removed from cache" });
        }

        [HttpDelete("pattern/{pattern}")]
        public async Task<IActionResult> RemoveByPattern(string pattern)
        {
            await _cacheService.RemoveByPatternAsync(pattern);
            return Ok(new { message = $"Keys matching pattern '{pattern}' removed from cache" });
        }
    }
}
