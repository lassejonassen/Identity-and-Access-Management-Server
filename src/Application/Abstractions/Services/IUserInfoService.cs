using Application.Contracts.Responses;

namespace Application.Abstractions.Services;

public interface IUserInfoService
{
    Task<UserInfoResponse> GetUserInfoAsync();
}
