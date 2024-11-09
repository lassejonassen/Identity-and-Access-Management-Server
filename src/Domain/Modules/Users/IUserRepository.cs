using Abstractions;

namespace Domain.Modules.Users;

public interface IUserRepository
{
    Task<Result<User>> GetByIdAsync(string userId);
    Task<User?> GetByUserNameAsync(string username);
    Task<User?> GetUserByEmailAsync(string email);
}
