using Application.Contracts.Responses;

namespace Application.Users.Queries.GetUserInfo;

public sealed record GetUserInfoQuery : IQuery<UserInfoResponse>;