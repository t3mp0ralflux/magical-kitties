using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MagicalKitties.ETE.Tests;

public class TestingAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestingAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Claim[] claims = [new Claim(ClaimTypes.Email, "test@test.com")];
        ClaimsIdentity identity = new ClaimsIdentity(claims, "Test");
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
        AuthenticationTicket ticket = new AuthenticationTicket(principal, "TestScheme");

        AuthenticateResult result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}