
namespace MilkingYield.API.Handlers;

internal sealed class TokenAuthorizationHandler(IHttpContextAccessor httpContext) 
    : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        if (!request.Headers.Contains("Authorization"))
        {
            string? token = httpContext.HttpContext?.Request.Headers.Authorization ?? string.Empty;
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Add("Authorization", [token]);
            }
        }
        return base.SendAsync(request, cancellationToken);
    }
}
