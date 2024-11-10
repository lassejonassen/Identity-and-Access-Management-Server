using Microsoft.EntityFrameworkCore;
using Domain.Modules.OAuth;
using Domain.Modules.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Domain.Modules.Devices;

namespace Persistence.DbContexts;

public sealed class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<OAuthApplication> OAuthApplications { get; set; }
    public DbSet<OAuthToken> OAuthTokens { get; set; }
    public DbSet<DeviceFlow> DeviceFlows { get; set; }
}

