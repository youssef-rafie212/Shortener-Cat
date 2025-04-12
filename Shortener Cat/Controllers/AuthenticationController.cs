using Core.Domain.Entities;
using Core.DTO;
using Core.ServicesContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shortener_Cat.Filters;

namespace Shortener_Cat.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IJwtService jwtService,
            ILogger<AuthenticationController> logger
            )
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> Signup(SignupDto dto)
        {
            ApplicationUser user = new()
            {
                UserName = dto.Username,
                Email = dto.Email,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
                bool userRoleExists = _roleManager.RoleExistsAsync("User").Result;
                if (!userRoleExists)
                {
                    await _roleManager.CreateAsync(new() { Name = "User" });
                }

                await _userManager.AddToRoleAsync(user, "User");
                string token = _jwtService.GenerateJwtToken(user);

                _logger.LogInformation($"Successful creation of a user with the Username: {user.UserName}");

                return Ok(new
                {
                    Token = token,
                });
            }
            else
            {
                List<string> errors = [];
                foreach (var err in result.Errors)
                {
                    errors.Add(err.Description);
                }

                _logger.LogWarning($"Unsuccessful creation of a user with the Username: {user.UserName}");

                return BadRequest(new
                {
                    Errors = errors
                });
            }
        }

        [HttpPost]
        [Route("sginin")]
        public async Task<IActionResult> Signin(SigninDto dto)
        {
            //TODO: use sign in manager
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user != null)
            {
                bool result = await _userManager.CheckPasswordAsync(user, dto.Password);
                if (result)
                {
                    string token = _jwtService.GenerateJwtToken(user);

                    _logger.LogInformation($"Successful user sign in with the Username: {user.UserName}");

                    return Ok(new
                    {
                        Token = token,
                    });
                }
            }

            _logger.LogWarning($"Unuccessful user sign in with the Username: {user.UserName}");

            return Unauthorized("Invalid Credentials.");
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
