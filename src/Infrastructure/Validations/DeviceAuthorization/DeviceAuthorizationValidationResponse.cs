using Domain.Modules.Clients;
using Infrastructure.Abstractions.Validations;

namespace Infrastructure.Validations.DeviceAuthorization;

public class DeviceAuthorizationValidationResponse : BaseValidationResponse
{
    /// <summary>
    /// Get ot set the client.
    /// </summary>
    public Client Client { get; set; }
    public string RequestedScope { get; set; }
}