using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Users;

namespace Application.Users.Queries.GetUserInfo;

public sealed record GetUserInfoQuery : IQuery<UserInfoResponse>;