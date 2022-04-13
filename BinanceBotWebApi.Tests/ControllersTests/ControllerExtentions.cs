using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BinanceBotWebApi.Tests.ControllersTests;

public static class ControllerExtentions
{
    public static void AddUser(this ControllerBase controller)
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim("Id", "1"),
        }, "mock"));
    
        controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };
    }
}