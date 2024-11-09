namespace Domain.Modules.OAuth;

public interface IOAuthTokenRepository
{
    Task<OAuthToken?> GetTokenAsync(string token, string tokenTypeHint, string clientId);
    Task<OAuthToken?> GetTokenAsync(string token);
    Task RevokeToken(OAuthToken token);
    Task<int> AddAsync(OAuthToken token);
}
