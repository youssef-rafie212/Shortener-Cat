using Core.Domain.Entities;
using Core.ServicesContracts;
using Microsoft.AspNetCore.Mvc;
using Shortener_Cat.Helpers;
using Wangkanai.Detection.Services;

namespace Shortener_Cat.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("/")]
    public class RedirectionController : ControllerBase
    {
        private readonly IShortUrlService _shortUrlService;
        private readonly ILogger<RedirectionController> _logger;
        private readonly IShortenerService _shortenerService;
        private readonly IUrlVisitService _urlVisitService;
        private readonly IDeviceService _deviceService;

        public RedirectionController(
            IShortUrlService shortUrlService,
            ILogger<RedirectionController> logger,
            IShortenerService shortenerService,
            IUrlVisitService urlVisitService,
            IDeviceService deviceService
            )
        {
            _logger = logger;
            _shortUrlService = shortUrlService;
            _shortenerService = shortenerService;
            _urlVisitService = urlVisitService;
            _deviceService = deviceService;
        }

        [HttpGet]
        [Route("{code}")]
        public async Task<IActionResult> RedirectToOriginal(string code)
        {
            try
            {
                int idFromCode = _shortenerService.Decode(code);
                ShortUrl shortUrl = await _shortUrlService.GetById(idFromCode);

                if (!shortUrl.IsActive)
                {
                    _logger.LogError($"Failed redirection for code '{code}'");
                    return BadRequest("This URL is no longer active");
                }

                if (shortUrl.VisitCount >= shortUrl.MaxVisit)
                {
                    _logger.LogError($"Failed redirection for code '{code}'");
                    return BadRequest("This URL reached the maximum visits number");
                }

                shortUrl.VisitCount++;

                ShortUrl res = await _shortUrlService.UpdateById(shortUrl.Id, shortUrl);

                string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                if (ip == "::1") ip = "8.8.8.8";
                string? country = await UrlVisitHelpers.GetCountryNameFromIpAddress(ip);
                string? agent = HttpContext.Request.Headers["User-Agent"];
                string? referrer = HttpContext.Request.Headers["Referer"];
                string device = _deviceService.Type.ToString();
                DateTime visitedAt = DateTime.UtcNow;

                UrlVisit visit = new()
                {
                    IPAddress = ip,
                    Country = country,
                    UserAgent = agent,
                    Referrer = referrer,
                    DeviceType = device,
                    VisitedAt = visitedAt,
                    ShortUrlId = shortUrl.Id,
                };

                await _urlVisitService.AddOne(visit);

                return Redirect(res.OriginalUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed redirection for code '{code}'");
                return NotFound();
            }
        }
    }
}
