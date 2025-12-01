using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Testing.Common;

public static class Utilities
{
    public static ControllerContext CreateControllerContext(string email)
    {
        ControllerContext result = new()
                                   {
                                       HttpContext = new DefaultHttpContext
                                                     {
                                                         User = new ClaimsPrincipal()
                                                     }
                                   };

        result.HttpContext.User.AddIdentity(new ClaimsIdentity([new Claim(ClaimTypes.Email, email)]));

        return result;
    }
}