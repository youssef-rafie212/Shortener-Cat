using Core.Domain.Entities;
using Core.DTO;
using Core.ServicesContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shortener_Cat.Filters;
using System.Security.Claims;

namespace Shortener_Cat.Controllers.v1
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/authentication")]
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
        [Route("signin")]
        public async Task<IActionResult> Signin(SigninDto dto)
        {
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

            _logger.LogWarning($"Unuccessful user sign in with the email: {dto.Email}");

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

        [HttpGet]
        [Route("email-confirm-token")]
        [ServiceFilter(typeof(BlackListTokenFilter))]
        [Authorize]
        public async Task<IActionResult> GetConfirmToken()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            _logger.LogInformation($"Successfully generated email confirmation token for user with username: {user.UserName}");

            return Ok(new { Token = token });
        }

        [HttpPost]
        [Route("confirm-email")]
        [ServiceFilter(typeof(BlackListTokenFilter))]
        [Authorize]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto dto)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            var res = await _userManager.ConfirmEmailAsync(user, dto.Token);

            if (!res.Succeeded)
            {
                _logger.LogWarning($"unsuccessful email confirmation for user with username: {user.UserName}");
                return BadRequest(res.Errors);
            }

            _logger.LogInformation($"Successful email confirmation for user with username: {user.UserName}");
            return Ok("Email confirmed successfully");
        }

        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            if (!int.TryParse(dto.UserId, out int _)) return BadRequest("ID must be in the form of an integer");
            ApplicationUser? user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null) return BadRequest("User not found");

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            _logger.LogInformation($"Successfully generated password reset token for user with username: {user.UserName}");

            return Ok(new { Token = token });
        }

        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            if (!int.TryParse(dto.UserId, out int _)) return BadRequest("ID must be in the form of an integer");
            ApplicationUser? user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null) return BadRequest("User not found");

            var res = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

            if (!res.Succeeded)
            {
                _logger.LogWarning($"unsuccessful password reset for user with username: {user.UserName}");
                return BadRequest(res.Errors);
            }

            _logger.LogInformation($"Successful password reset for user with username: {user.UserName}");
            return Ok("Password changed successfully");
        }
    }
}
