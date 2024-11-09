using Microsoft.EntityFrameworkCore;
using Domain.Modules.OAuth;
using Domain.Modules.Users;

namespace Persistence.DbContexts;

public sealed class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<OAuthToken> OAuthTokens { get; set; }
    public DbSet<User> Users { get; set; }
}

