namespace MagicalKitties.Api.Auth;

public static class AuthConstants
{
    public const string AdminUserClaimName = "admin";
    public const string TrustedUserClaimName = "trusted_user";
    public const string RefreshTokenClaimName = "refresh_token";
    
    public const string AdminUserPolicyName = "Admin";    
    public const string TrustedUserPolicyName = "TrustedUser";
    public const string RefreshTokenPolicyName = "RefreshToken";
}