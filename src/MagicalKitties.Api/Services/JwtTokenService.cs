using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MagicalKitties.Api.Auth;
using MagicalKitties.Application;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Services;
using Microsoft.IdentityModel.Tokens;

namespace MagicalKitties.Api.Services;

public interface IJwtTokenService
{
    string GenerateAccessToken(Account account);
    string GenerateRefreshToken(Account account);
    bool ValidateCustomToken(string accessToken);
    string? GetEmailFromToken(string jwtToken);
}

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _config;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly TimeSpan _accessTokenLifetime;
    private readonly TimeSpan _refreshTokenLifetime;
    private readonly JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();

    public JwtTokenService(IConfiguration configuration, IGlobalSettingsService globalSettingsService, IDateTimeProvider dateTimeProvider)
    {
        _config = configuration;
        this._dateTimeProvider = dateTimeProvider;

        int accessLifetimeHours = globalSettingsService.GetSettingAsync(WellKnownGlobalSettings.ACCESS_TOKEN_LIFETIME_HOURS, 1).Result;
        _accessTokenLifetime = TimeSpan.FromHours(accessLifetimeHours);

        int refreshLifeTimeDays = globalSettingsService.GetSettingAsync(WellKnownGlobalSettings.REFRESH_TOKEN_LIFETIME_DAYS, 7).Result;
        _refreshTokenLifetime = TimeSpan.FromDays(refreshLifeTimeDays);
    }

    public string GenerateAccessToken(Account account)
    {
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, account.Email),
            new(JwtRegisteredClaimNames.Email, account.Email),
            new(AuthConstants.AdminUserClaimName, (account.AccountRole == AccountRole.admin).ToString().ToLower()),
            new(AuthConstants.TrustedUserClaimName, (account.AccountRole is AccountRole.admin or AccountRole.trusted).ToString().ToLower())
        ];
        
        return this.GenerateToken(claims, _dateTimeProvider.GetUtcNow().Add(_accessTokenLifetime));
    }

    public string GenerateRefreshToken(Account account)
    {
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, account.Email),
            new(AuthConstants.RefreshTokenClaimName, "true")
        ];
        
        return this.GenerateToken(claims, _dateTimeProvider.GetUtcNow().Add(_refreshTokenLifetime));
    }

    private string GenerateToken(List<Claim> claims, DateTime expiryUtc)
    {
        string tokenSecret = _config["Jwt:Key"]!;

        byte[] key = Encoding.UTF8.GetBytes(tokenSecret);

        SecurityTokenDescriptor tokenDescriptor = new()
                                                  {
                                                      Subject = new ClaimsIdentity(claims),
                                                      Expires = expiryUtc,
                                                      Issuer = _config["Jwt:Issuer"],
                                                      Audience = _config["Jwt:Audience"],
                                                      SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                                                  };

        SecurityToken newToken = _tokenHandler.CreateToken(tokenDescriptor);

        string jwt = _tokenHandler.WriteToken(newToken);

        return jwt;
    }

    public bool ValidateCustomToken(string accessToken)
    {
        TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
                                                              {
                                                                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)),
                                                                  ValidateIssuerSigningKey = true,
                                                                  ValidIssuer = _config["Jwt:Issuer"],
                                                                  ValidAudience = _config["Jwt:Audience"],
                                                                  ValidateIssuer = true,
                                                                  ValidateAudience = true
                                                              };
        
        try
        {
            // just validate and make sure it's good structurally.
            _tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken? _);
            return true;
        }
        catch
        {
            return false;
        }

    }

    public string? GetEmailFromToken(string jwtToken)
    {
        try
        {
            JwtSecurityToken parsedToken = _tokenHandler.ReadJwtToken(jwtToken);
            return parsedToken.Subject;
        }
        catch
        {
            return null;
        }
    }
}