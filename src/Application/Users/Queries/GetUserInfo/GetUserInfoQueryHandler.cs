using Application.Abstractions.Services;
using Application.Contracts.Users;

namespace Application.Users.Queries.GetUserInfo;

internal sealed class GetUserInfoQueryHandler(IUserInfoService userInfoService)
    : IQueryHandler<GetUserInfoQuery, UserInfoResponse>
{
    public async Task<Result<UserInfoResponse>> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
    {
        var userInfo = await userInfoService.GetUserInfoAsync();
        return userInfo;
    }
}
