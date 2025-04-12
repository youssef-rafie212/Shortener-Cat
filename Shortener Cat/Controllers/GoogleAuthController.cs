using Core.Domain.Entities;
using Core.DTO;
using Core.ServicesContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Shortener_Cat.Controllers
{
    [Route("api/v1/google-auth")]
    [ApiController]
    public class GoogleAuthController : ControllerBase
    {
        private readonly IGoogleAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;

        public GoogleAuthController(IGoogleAuthService authService, UserManager<ApplicationUser> userManager, IJwtService jwtService)
        {
            _authService = authService;
            _userManager = userManager;
            _jwtService = jwtService;
        }

        [HttpPost]
        [Route("signin")]
        public async Task<IActionResult> Signin(GoogleLoginDto dto)
        {
            var payload = await _authService.VerifyToken(dto.Token);
            if (payload == null)
            {
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

                var res = await _userManager.CreateAsync(newUser);
                if (!res.Succeeded)
                {
                    return BadRequest(res.Errors);
                }
            }

            ApplicationUser userFromDb = (await _userManager.FindByEmailAsync(payload.Email))!;
            string token = _jwtService.GenerateJwtToken(userFromDb);
            return Ok(token);
        }
    }
}
