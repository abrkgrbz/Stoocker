using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Stoocker.API.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Policy = "SuperAdmin")]
    public class BaseAdminController : ControllerBase
    {
        private IMediator _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
        protected readonly ILogger<BaseAdminController> _logger;

        protected BaseAdminController(ILogger<BaseAdminController> logger)
        {
            _logger = logger;
        }
    }
}
