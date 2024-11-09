using Abstractions;

namespace Infrastructure.Enumerations;

public class ErrorTypeEnum : Enumeration<ErrorTypeEnum>
{
    public static readonly ErrorTypeEnum InvalidRequest = new(1, "invalid_request");
    public static readonly ErrorTypeEnum UnauthorizedClient = new(2, "unauthorized_client");
    public static readonly ErrorTypeEnum AccessDenied = new(3, "access_denied");
    public static readonly ErrorTypeEnum UnsupportedResponseType = new(4, "unsupported_response_type");
    public static readonly ErrorTypeEnum InvalidScope = new(5, "invalid_scope");
    public static readonly ErrorTypeEnum ServerError = new(6, "server_error");
    public static readonly ErrorTypeEnum TemporarilyUnavailable = new(7, "temporarily_unavailable");
    public static readonly ErrorTypeEnum InvalidGrant = new(8, "invalid_grant");
    public static readonly ErrorTypeEnum InvalidClient = new(9, "invalid_client");
    public static readonly ErrorTypeEnum WaitForUserInteraction = new(10, "wait_for_user_interaction");

    public ErrorTypeEnum(int value, string name) : base(value, name)
    {
    }
}
