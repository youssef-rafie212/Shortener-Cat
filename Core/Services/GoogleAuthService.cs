using Core.ServicesContracts;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Core.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<GoogleAuthService> _logger;

        public GoogleAuthService(IConfiguration config, ILogger<GoogleAuthService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<GoogleJsonWebSignature.Payload?> VerifyToken(string token)
        {
            string clientId = _config["scl"]!;
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = [clientId]
            };

            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);
                _logger.LogInformation($"Successful google token verification");
                return payload;
            }
            catch
            {
                _logger.LogWarning($"Unsuccessful google token verification");
                return null;
            }
        }
    }
}
