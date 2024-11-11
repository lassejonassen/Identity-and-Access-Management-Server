using System.Security.Claims;
using Abstractions;
using Application.Contracts;
using Application.Contracts.Responses;
using Domain.Modules.Users;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Serilog;
using static Abstractions.Constants;

namespace Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly SignInManager<User> _signInManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger _logger;

    public async Task<Result<User>> GetUserAsync(string userId)
        => await _userRepository.GetByIdAsync(userId);

    public async Task<OpenIdConnectLoginResponse> LoginAsync(OpenIdConnectLoginRequest loginRequest)
    {
        bool validationResult = validateOpenIdLoginRequest(loginRequest);
        if (!validationResult)
        {
            _logger.Information("login process is failed for request: {request}", loginRequest);
            return new OpenIdConnectLoginResponse { Error = "The login process is failed" };
        }

        var user = await _userRepository.GetByUserNameAsync(loginRequest.UserName);
        if (user is null && loginRequest.UserName.Contains("@"))
        {
            user = await _userRepository.GetUserByEmailAsync(loginRequest.UserName);
        }

        if (user is null)
        {
            _logger.Information("User credentials does not exist: {0}", loginRequest.UserName);
            return new OpenIdConnectLoginResponse { Error = "User credentials does not exist" };
        }

        if (!VerifyUserpassword(user, loginRequest.Password))
        {
            return new OpenIdConnectLoginResponse { Error = "User credentials invalid" };
        }

        await _httpContextAccessor.HttpContext!.SignOutAsync(AuthSchemas.AuthenticationScheme);

        var response = new OpenIdConnectLoginResponse();

        var principal = ParseUserToClaimsPrincipal(response.User);

        await _httpContextAccessor.HttpContext!.SignInAsync(principal, new()
        {
            IsPersistent = true,
        });

        return response;
    }

    private ClaimsPrincipal ParseUserToClaimsPrincipal(User user)
    {
        var claims = new List<Claim>();

        var identity = new ClaimsIdentity(claims, AuthSchemas.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        return principal;
    }

    private bool validateOpenIdLoginRequest(OpenIdConnectLoginRequest request)
    {
        if (request.Code == null || request.UserName == null || request.Password == null)
            return false;
        return true;
    }

    private bool VerifyUserpassword(User user, string password)
    {
        return true;
    }
}