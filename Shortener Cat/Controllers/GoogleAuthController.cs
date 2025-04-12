using Core.Domain.Entities;
using Core.DTO;
using Core.ServicesContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shortener_Cat.Filters;

namespace Shortener_Cat.Controllers
{
    [Route("api/v1/google-auth")]
    [ApiController]
    public class GoogleAuthController : ControllerBase
    {
        private readonly IGoogleAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<GoogleAuthController> _logger;

        public GoogleAuthController(IGoogleAuthService authService, UserManager<ApplicationUser> userManager, IJwtService jwtService, ILogger<GoogleAuthController> logger)
        {
            _authService = authService;
            _userManager = userManager;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost]
        [Route("signin")]
        public async Task<IActionResult> Signin(GoogleLoginDto dto)
        {
            var payload = await _authService.VerifyToken(dto.Token);
            if (payload == null)
            {
                _logger.LogWarning($"Unsuccessful user google sign in with the token: {dto.Token}");
                return Unauthorized("Invalid Token");
            }

            ApplicationUser? user = await _userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                ApplicationUser newUser = new()
                {
                    Email = payload.Email,
                    UserName = payload.Email,
                    EmailConfirmed = true,
                };

                var creationRes = await _userManager.CreateAsync(newUser);
                var roleRes = await _userManager.AddToRoleAsync(newUser, "User");
                if (!creationRes.Succeeded || !roleRes.Succeeded)
                {
                    _logger.LogWarning($"Unsuccessful user google sign in creation with the Username: {newUser.UserName}");
                    return BadRequest();
                }
            }

            ApplicationUser userFromDb = (await _userManager.FindByEmailAsync(payload.Email))!;
            string token = _jwtService.GenerateJwtToken(userFromDb);

            _logger.LogInformation($"Successful user google sign in with the token: {dto.Token}");

            return Ok(token);
        }

        [HttpPost]
        [Route("signout")]
        [ServiceFilter(typeof(BlackListTokenFilter))]
        [Authorize]
        public async Task<IActionResult> Signout()
        {
            string auth = HttpContext.Request.Headers.Authorization.ToString();

            string token = auth.Substring("Bearer ".Length).Trim();

            await _jwtService.ExpireToken(token);

            _logger.LogInformation($"Successful jwt token expiration with the value: {token}");

            return Ok();
        }
    }
}
