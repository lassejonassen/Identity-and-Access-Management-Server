namespace Application.Contracts;

public record JwkKey
{
    public string alg { get; init; }
    public string e { get; init; }
    public string n { get; init; }
    public string kty { get; init; }
    public string kid { get; init; }
}

public record Jwk
{
    public List<JwkKey> Keys { get; init; }
}
