using Application.Contracts.Users;

namespace Application.Abstractions.Services;

public interface IUserInfoService
{
    Task<UserInfoResponse> GetUserInfoAsync();
}
