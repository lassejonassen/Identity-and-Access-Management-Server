namespace Infrastructure.Validations.BearerTokenUsageType;

public interface IBearerTokenUsageTypeValidation
{
    Task<BearerTokenUsageTypeValidationResponse> ValidateAsync();
}
