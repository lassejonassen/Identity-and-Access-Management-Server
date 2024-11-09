using Application.Users.Queries.GetUserInfo;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;
[Route("api/user-info")]
[ApiController]
public class UserInfoController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUserInfo()
    {
        var result = await sender.Send(new GetUserInfoQuery());
        return Ok();
    }
}
