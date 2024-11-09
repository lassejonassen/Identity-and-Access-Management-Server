using Abstractions;
using Infrastructure.Abstractions.Validations;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Validations.BearerTokenUsageType;

public sealed class BearerTokenUsageTypeValidation(IHttpContextAccessor httpContextAccessor)
    : IBearerTokenUsageTypeValidation
{
    public Task<BearerTokenUsageTypeValidationResponse> ValidateAsync()
    {
        var response = new BearerTokenUsageTypeValidationResponse { Succeeded = false };

        var authorizationHeader = httpContextAccessor.HttpContext!.Request.Headers["Authorization"];

        var header = authorizationHeader.First().Trim();

        if (header is not null
            && header.StartsWith(Constants.AuthenticatedRequestScheme.AuthorizationRequestHeader))
        {
            var token = header.Substring(Constants.AuthenticatedRequestScheme.AuthorizationRequestHeader.Length).Trim();

            if (token is not null && token.Length > 0)
            {
                response.Succeeded = true;
                response.Token = token;
                response.BearerTokenUsageType = BearerTokenUsageTypeEnum.AuthorizationRequestHeader;
                return Task.FromResult(response);
            }
        }

        var postForm = httpContextAccessor.HttpContext.Request.Form;
        if (postForm is not null && postForm.Any())
        {
            if (postForm.ContainsKey(Constants.AuthenticatedRequestScheme.FormEncodedBodyParameter))
            {
                var token = postForm.Where(x => x.Key == Constants.AuthenticatedRequestScheme.AuthorizationRequestHeader)
                    .Select(x => x.Value)
                    .FirstOrDefault();

                if (token.Count == 1)
                {
                    string value = token;
                    if (value is not null)
                    {
                        response.Succeeded = true;
                        response.Token = value;
                        response.BearerTokenUsageType = BearerTokenUsageTypeEnum.FormEncodedBodyParameter;
                        return Task.FromResult(response);
                    }
                }
            }
        }

        return Task.FromResult(response);
    }
}
