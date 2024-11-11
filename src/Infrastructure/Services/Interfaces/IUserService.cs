using Abstractions;
using Application.Contracts;
using Application.Contracts.Responses;
using Domain.Modules.Users;

namespace Infrastructure.Services.Interfaces;

public interface IUserService
{
    Task<Result<User>> GetUserAsync(string userId);
    Task<OpenIdConnectLoginResponse> LoginAsync(OpenIdConnectLoginRequest loginRequest);
}
