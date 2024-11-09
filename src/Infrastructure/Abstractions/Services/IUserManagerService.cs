using Common;
using Domain.Modules.Users;

namespace Infrastructure.Abstractions.Services;

public interface IUserManagerService
{
    Task<User> GetUserAsync(string userId);
    Task<LoginResponse> LoginUserAsync(LoginRequest request);
    Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request);
    Task<OpenIdConnectLoginResponse> LoginUserByOpenIdAsync(OpenIdConnectLoginRequest request);
}
