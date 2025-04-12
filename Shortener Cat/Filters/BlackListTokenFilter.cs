using Core.ServicesContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Shortener_Cat.Filters
{
    public class BlackListTokenFilter : IAuthorizationFilter
    {
        private readonly IJwtService _jwtService;
        private readonly ILogger<BlackListTokenFilter> _logger;

        public BlackListTokenFilter(IJwtService jwtService, ILogger<BlackListTokenFilter> logger)
        {
            _jwtService = jwtService;
            _logger = logger;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string? auth = context.HttpContext.Request.Headers["Authorization"];

            if (auth == null || !auth.StartsWith("Bearer "))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            string token = auth.Substring("Bearer ".Length).Trim();
            if (_jwtService.IsExpired(token))
            {
                _logger.LogWarning($"Unsuccessful authentication beacause of expired jwt token with the value: {token}");
                context.Result = new UnauthorizedResult();
                return;
            }
        }
    }
}