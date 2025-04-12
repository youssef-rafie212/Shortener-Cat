using Google.Apis.Auth;

namespace Core.ServicesContracts
{
    public interface IGoogleAuthService
    {
        Task<GoogleJsonWebSignature.Payload?> VerifyToken(string token);
    }
}
