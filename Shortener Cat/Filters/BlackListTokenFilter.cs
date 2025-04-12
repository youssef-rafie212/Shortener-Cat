using Core.ServicesContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Shortener_Cat.Filters
{
    public class BlackListTokenFilter : IAuthorizationFilter
    {
        private readonly IJwtService _jwtService;

        public BlackListTokenFilter(IJwtService jwtService)
        {
            _jwtService = jwtService;
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
                context.Result = new UnauthorizedResult();
                return;
            }
        }
    }
}