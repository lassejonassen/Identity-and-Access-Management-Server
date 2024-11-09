using Domain.Modules.OAuth;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Persistence.Repositories;

public sealed class OAuthTokenRepository(ApplicationDbContext context) : IOAuthTokenRepository
{
    public async Task<int> AddAsync(OAuthToken token)
    {
        await context.OAuthTokens.AddAsync(token);
        var result = await context.SaveChangesAsync();
        return result;
    }

    public async Task<OAuthToken?> GetTokenAsync(string token, string tokenTypeHint, string clientId)
        => await context.OAuthTokens.FirstOrDefaultAsync(x => x.Token == token
        && x.ClientId == clientId
        && (string.IsNullOrWhiteSpace(tokenTypeHint) || tokenTypeHint == x.TokenTypeHint));

    public async Task<OAuthToken?> GetTokenAsync(string token) 
        => await context.OAuthTokens.FirstOrDefaultAsync(x => x.Token == token);

    public async Task RevokeToken(OAuthToken token)
    {
        token.Revoked = true;
        context.OAuthTokens.Update(token);
        await context.SaveChangesAsync();
    }
}
