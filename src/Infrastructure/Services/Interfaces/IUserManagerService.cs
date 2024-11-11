using Application.Contracts;
using Application.Contracts.Responses;
using Domain.Modules.Users;

namespace Infrastructure.Services.Interfaces;

public interface IUserManagerService
{
    Task<User> GetUserAsync(string userId);
    Task<LoginResponse> LoginUserAsync(LoginRequest request);
    Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request);
    Task<OpenIdConnectLoginResponse> LoginUserByOpenIdAsync(OpenIdConnectLoginRequest request);
}
