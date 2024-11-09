using Common;
using Infrastructure.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;
[Route("api/accounts")]
[ApiController]
public class AccountsController(IUserManagerService userManagerService) : ControllerBase
{
    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await userManagerService.LoginUserAsync(request);

        if (!response.Succeeded)
        {
            return Redirect("/login");
        }

        return Ok();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(CreateUserRequest request)
    {
        var result = await userManagerService.CreateUserAsync(request);

        if (!result.Succeeded)
            return Redirect("/register");

        return Ok();
    }
}
