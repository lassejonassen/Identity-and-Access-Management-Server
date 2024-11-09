using Application.Abstractions.Services;
using Common;
using Infrastructure.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuthorizeResultService _authorizeResultService;
    private readonly ICodeStoreService _codeStoreService;
    private readonly IUserService _userService;

    public AuthController(IHttpContextAccessor httpContextAccessor,
        IAuthorizeResultService authorizeResultService,
        ICodeStoreService codeStoreService,
        IUserService userService)
    {
        _httpContextAccessor = httpContextAccessor;
        _authorizeResultService = authorizeResultService;
        _codeStoreService = codeStoreService;
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(OpenIdConnectLoginRequest loginRequest)
    {
        if (!loginRequest.IsValid())
        {
            return Redirect("/error");
        }

        var loginResult = await _userService.LoginAsync(loginRequest);

        if (loginResult.Succeeded)
        {
            var result =  _codeStoreService.UpdatedClientDataByCode(loginRequest.Code, loginResult.ClaimsPrincipal, loginRequest.RequestedScopes);

            if (result != null)
            {
                loginRequest.RedirectUri = loginRequest.RedirectUri + "&code=" + loginRequest.Code;
                return Redirect(loginRequest.RedirectUri);
            }
        }

        return Redirect("error");
    }

    [HttpGet("authorize")]
    public IActionResult Authorize(AuthorizationRequest authorizationRequest)
    {
        var result = _authorizeResultService.AuthorizeRequest(_httpContextAccessor, authorizationRequest);

        if (result.HasError)
        {
            return Redirect("/error");
        }

        if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
        {
            var updateCodeResult = _codeStoreService.UpdatedClientDataByCode(result.Code,
                    _httpContextAccessor.HttpContext.User, result.RequestedScopes);

            if (updateCodeResult != null)
            {
                result.RedirectUri = result.RedirectUri + "&code=" + result.Code;
                return Redirect(result.RedirectUri);
            }
            else
            {
                return Redirect("/error");
            }
        }

        return Redirect($"/login?redirect_uri={result.RedirectUri}&code={result.Code}&requested_scopes={result.RequestedScopes}");
    }
}
