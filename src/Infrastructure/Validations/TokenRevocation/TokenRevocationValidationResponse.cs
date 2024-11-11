using Domain.Modules.Clients;

namespace Infrastructure.Validations.TokenRevocation;

public class TokenRevocationValidationResponse : BaseValidationResponse
{
    public Client Client { get; set; }
}
