using Abstractions;
using Common;
using Domain.Modules.Users;

namespace Application.Abstractions.Services;

public interface IUserService
{
    Task<Result<User>> GetUserAsync(string userId);
    Task<OpenIdConnectLoginResponse> LoginAsync(OpenIdConnectLoginRequest loginRequest);
}
