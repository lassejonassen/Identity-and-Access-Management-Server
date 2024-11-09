using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abstractions;

namespace Domain.Modules.Users;

public static class UserErrors
{
    public static readonly Error InvalidIdFormat = Error.Failure("User.InvalidIdFormat", "The specified user id is not in a valid format.");
    public static readonly Error NotFound = Error.Failure("User.NotFound", "Could not find user with specified identifier");
}
