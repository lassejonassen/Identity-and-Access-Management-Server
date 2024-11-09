using Domain.Modules.Clients;
using Infrastructure.Abstractions.Validations;

namespace Infrastructure.Validations.TokenRevocation;

public class TokenRevocationValidationResponse : BaseValidationResponse
{
    public Client Client { get; set; }
}
