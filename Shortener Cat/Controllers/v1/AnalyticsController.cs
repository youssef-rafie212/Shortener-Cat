using Core.ServicesContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shortener_Cat.Filters;
using System.Security.Claims;

namespace Shortener_Cat.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/analytics")]
    [ApiVersion("1.0")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet]
        [Route("max-visits-by-country/{urlId:int}")]
        [Authorize]
        [ServiceFilter(typeof(BlackListTokenFilter))]
        public async Task<IActionResult> MaxVisitsByCountry(int urlId)
        {
            var res = await _analyticsService.GetMaxVisitsByCountry(urlId);
            if (res == null) return BadRequest("Url does not exist or has no vists yet");
            return Ok(res);
        }

        [HttpGet]
        [Route("max-visits-by-device/{urlId:int}")]
        [Authorize]
        [ServiceFilter(typeof(BlackListTokenFilter))]
        public async Task<IActionResult> GetMaxVisitsByDevice(int urlId)
        {
            var res = await _analyticsService.GetMaxVisitsByDevice(urlId);
            if (res == null) return BadRequest("Url does not exist or has no vists yet");
            return Ok(res);
        }

        [HttpGet]
        [Route("url-with-max-visits-for-user")]
        [Authorize]
        [ServiceFilter(typeof(BlackListTokenFilter))]
        public async Task<IActionResult> GetUrlWithMaxVisits()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var res = await _analyticsService.GetMaxVisitsForUserUrls(int.Parse(userId));
            if (res == null) return BadRequest("User has no URLs or URLs have no visits yet");
            return Ok(res);
        }
    }
}
