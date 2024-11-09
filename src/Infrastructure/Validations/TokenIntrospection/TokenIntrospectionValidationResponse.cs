using Domain.Modules.Clients;
using Infrastructure.Abstractions.Validations;

namespace Infrastructure.Validations.TokenIntrospection;

public class TokenIntrospectionValidationResponse : BaseValidationResponse
{
    /// <summary>
    /// Get or set client.
    /// </summary>
    public Client Client { get; set; }
}