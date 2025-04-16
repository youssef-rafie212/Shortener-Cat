using Core.ServicesContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shortener_Cat.Filters;

namespace Shortener_Cat.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("/")]
    public class RedirectionController : ControllerBase
    {
        private readonly IShortUrlService _shortUrlService;
        private readonly ILogger<RedirectionController> _logger;

        public RedirectionController(IShortUrlService shortUrlService, ILogger<RedirectionController> logger)
        {
            _logger = logger;
            _shortUrlService = shortUrlService;
        }

        [HttpGet]
        [Route("{code}")]
        [Authorize]
        [ServiceFilter(typeof(BlackListTokenFilter))]
        public async Task<IActionResult> Redirect(string code)
        {
            return Ok();
        }
    }
}
