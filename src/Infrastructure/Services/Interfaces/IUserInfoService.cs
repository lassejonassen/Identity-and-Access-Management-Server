﻿using Application.Contracts.Responses;

namespace Infrastructure.Services.Interfaces;

public interface IUserInfoService
{
    Task<UserInfoResponse> GetUserInfoAsync();
}
