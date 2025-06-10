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
    string GenerateToken(Account account);
    string GenerateRefreshToken();
    bool ValidateCustomToken(string accessToken);
}

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _config;
    private readonly TimeSpan _tokenLifetime;

    public JwtTokenService(IConfiguration configuration, IGlobalSettingsService globalSettingsService)
    {
        _config = configuration;

        int lifetimeHours = globalSettingsService.GetSettingAsync(WellKnownGlobalSettings.JWT_TOKEN_SECRET, 1).Result;
        _tokenLifetime = TimeSpan.FromHours(lifetimeHours);
    }

    public string GenerateToken(Account account)
    {
        string tokenSecret = _config["Jwt:Key"]!;
        JwtSecurityTokenHandler tokenHandler = new();

        byte[] key = Encoding.UTF8.GetBytes(tokenSecret);

        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, account.Email),
            new(JwtRegisteredClaimNames.Email, account.Email),
            new(AuthConstants.AdminUserClaimName, (account.AccountRole == AccountRole.admin).ToString().ToLower()),
            new(AuthConstants.TrustedUserClaimName, (account.AccountRole is AccountRole.admin or AccountRole.trusted).ToString().ToLower())
        ];

        SecurityTokenDescriptor tokenDescriptor = new()
                                                  {
                                                      Subject = new ClaimsIdentity(claims),
                                                      Expires = DateTime.UtcNow.Add(_tokenLifetime),
                                                      Issuer = _config["Jwt:Issuer"],
                                                      Audience = _config["Jwt:Audience"],
                                                      SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                                                  };

        SecurityToken newToken = tokenHandler.CreateToken(tokenDescriptor);

        string jwt = tokenHandler.WriteToken(newToken);

        return jwt;
    }

    public string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];

        using RandomNumberGenerator numberGenerator = RandomNumberGenerator.Create();
        numberGenerator.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
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

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            // just validate and make sure it's good structurally.
            tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken? _);
            return true;
        }
        catch
        {
            return false;
        }

    }
}