using Core.Domain.Entities;
using Core.DTO;
using Core.ServicesContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shortener_Cat.Filters;
using System.Security.Claims;

namespace Shortener_Cat.Controllers.v1
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/short-url")]
    public class ShortUrlController : ControllerBase
    {
        private readonly IShortUrlService _shortUrlService;
        private readonly ILogger<ShortUrlController> _logger;

        public ShortUrlController(IShortUrlService shortUrlService, ILogger<ShortUrlController> logger)
        {
            _shortUrlService = shortUrlService;
            _logger = logger;
        }

        [HttpPost]
        [Route("generate")]
        [Authorize]
        [ServiceFilter(typeof(BlackListTokenFilter))]
        public async Task<IActionResult> Generate(GenerateShortUrlDto dto)
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                ShortUrl res = await _shortUrlService.CreateShortUrl(int.Parse(userId), dto.OriginalUrl);
                _logger.LogInformation($"Successful creation of a short url with value: {res.Value}, for original url: {dto.OriginalUrl}");
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Unsuccessful creation of a short url for original url: {dto.OriginalUrl}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("user")]
        [Authorize]
        [ServiceFilter(typeof(BlackListTokenFilter))]
        public async Task<IActionResult> GetAllForUser()
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                return Ok(await _shortUrlService.GetAllShortUrlsForUser(int.Parse(userId)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        [Authorize]
        [ServiceFilter(typeof(BlackListTokenFilter))]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                return Ok(await _shortUrlService.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize]
        [ServiceFilter(typeof(BlackListTokenFilter))]
        public async Task<IActionResult> DeleteById(int id)
        {
            try
            {
                await _shortUrlService.DeleteById(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("activate/{id:int}")]
        [Authorize]
        [ServiceFilter(typeof(BlackListTokenFilter))]
        public async Task<IActionResult> Activate(int id)
        {
            try
            {
                await _shortUrlService.Activate(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("deactivate/{id:int}")]
        [Authorize]
        [ServiceFilter(typeof(BlackListTokenFilter))]
        public async Task<IActionResult> Deactivate(int id)
        {
            try
            {
                await _shortUrlService.Deactivate(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch]
        [Route("{id:int}")]
        [Authorize]
        [ServiceFilter(typeof(BlackListTokenFilter))]
        public async Task<IActionResult> Update(int id, UpdateShortUrlDto dto)
        {
            try
            {
                ShortUrl newShortUrl = new()
                {
                    Value = dto.NewValue,
                    OriginalUrl = dto.NewOriginalUrl,
                    VisitCount = dto.NewVisitCount
                };
                return Ok(await _shortUrlService.UpdateById(id, newShortUrl));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
