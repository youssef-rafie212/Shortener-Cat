using Core.ServicesContracts;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;

namespace Core.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IConfiguration _config;

        public GoogleAuthService(IConfiguration config)
        {
            _config = config;
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
                return payload;
            }
            catch
            {
                return null;
            }
        }
    }
}
