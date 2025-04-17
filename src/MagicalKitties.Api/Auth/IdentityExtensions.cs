using System.Security.Claims;

namespace MagicalKitties.Api.Auth;

public static class IdentityExtensions
{
    public static string? GetUserEmail(this HttpContext? context)
    {
        
        Claim? email = context?.User.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Email);

        return email?.Value;
    }
}