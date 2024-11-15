﻿namespace Application.Contracts;

public class OpenIdConnectLoginRequest
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string RedirectUri { get; set; }
    public string Code { get; set; }
    public IList<string> RequestedScopes { get; set; }

    public bool IsValid()
    {
        return UserName != null && Password != null && Code != null && RedirectUri != null;
    }
}
