using Abstractions;
using Domain.Modules.Users;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Persistence.Repositories;
public sealed class UserRepository(ApplicationDbContext context)
    : IUserRepository
{
    public async Task<Result<User>> GetByIdAsync(string userId)
    {
        bool valid = Guid.TryParse(userId, out Guid parsedUserId);

        if (!valid)
        {
            return Result.Failure<User>(UserErrors.InvalidIdFormat);
        }

        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            return Result.Failure<User>(UserErrors.NotFound);
        }

        return Result.Success(user);
    }

    public Task<User?> GetByUserNameAsync(string username) => throw new NotImplementedException();
    public Task<User?> GetUserByEmailAsync(string email) => throw new NotImplementedException();
}
