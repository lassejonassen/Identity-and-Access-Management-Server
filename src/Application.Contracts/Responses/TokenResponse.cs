﻿namespace Application.Contracts.Responses;

public class TokenResponse
{
    /// <summary>
    /// Oauth 2
    /// </summary>
    public string access_token { get; set; }

    /// <summary>
    /// OpenId Connect
    /// </summary>
    public string id_token { get; set; }

    /// <summary>
    /// By default is Bearer
    /// </summary>

    public string token_type { get; set; } = "Bearer";

    /// <summary>
    /// Authorization Code. This is always returned when using the Hybrid Flow.
    /// </summary>
    public string code { get; set; }

    /// <summary>
    /// For Error Details if any
    /// </summary>
    public string Error { get; set; } = string.Empty;
    public string ErrorUri { get; set; }
    public string ErrorDescription { get; set; }
    public bool HasError => !string.IsNullOrEmpty(Error);
}