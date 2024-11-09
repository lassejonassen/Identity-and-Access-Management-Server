namespace Infrastructure.Abstractions.Validations;

public abstract class BaseValidationResponse
{
    public bool Succeeded { get; set; }
    public string Error { get; set; } = string.Empty;
    public string ErrorDescription { get; set; } = string.Empty;
    public bool HasError => !string.IsNullOrEmpty(Error);
}
