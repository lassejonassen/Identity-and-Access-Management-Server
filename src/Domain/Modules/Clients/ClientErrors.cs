using Abstractions;

namespace Domain.Modules.Clients;

public static class ClientErrors
{
    public static readonly Error NameIsNullOrEmpty = Error.Validation("Client.NameIsNullOrEmpty", "Client name cannot be null or empty");
    public static readonly Error NameIsLongerThanAllowed = Error.Validation("Client.NameIsLongerThanAllowed", "Client name is longer than allowed");
    public static readonly Error ClientUriInvalidFormat = Error.Validation("Client.ClientUriInvalidFormat", "Client uri is not in a valid format");
    public static readonly Error RedirectUriInvalidFormat = Error.Validation("Client.RedirectUriInvalidFormat", "Redirect uri is not in a valid format");
}
