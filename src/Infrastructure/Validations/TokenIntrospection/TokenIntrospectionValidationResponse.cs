using Domain.Modules.Clients;

namespace Infrastructure.Validations.TokenIntrospection;

public class TokenIntrospectionValidationResponse : BaseValidationResponse
{
    /// <summary>
    /// Get or set client.
    /// </summary>
    public Client Client { get; set; }
}