using Infrastructure.Abstractions.Validations;

namespace Infrastructure.Validations.BearerTokenUsageType;

public class BearerTokenUsageTypeValidationResponse : BaseValidationResponse
{
    public string Token { get; set; }
    public BearerTokenUsageTypeEnum BearerTokenUsageType { get; set; }
}
