using Microsoft.Extensions.Primitives;

namespace OAuth.AuthorizationServer;

public static class HttpContextExtensions
{
    public static IDictionary<string, StringValues> ParseOAuthParameters(this HttpContext httpContext, List<string>? excluding = null)
    {
        excluding ??= new List<string>();
        
        var parameters = httpContext.Request.HasFormContentType
            ? httpContext.Request.Form
                .Where(v => !excluding.Contains(v.Key))
                .ToDictionary(v => v.Key, v => v.Value)
            : httpContext.Request.Query
                .Where(v => !excluding.Contains(v.Key))
                .ToDictionary(v => v.Key, v => v.Value);

        return parameters;
    }
}