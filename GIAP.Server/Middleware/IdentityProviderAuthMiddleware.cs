using System.Security.Claims;
using GIAP.Server.Models;
using GIAP.Server.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace GIAP.Server.Middleware;

/// <summary>
/// Checks if the identity provider exists and if the user is authenticated through it.
/// If authentication fails, it challenges the user to log in at the identity provider.
/// It then sets the authentication data in the HttpContext to be used within the controller.
/// </summary>
/// <remarks>
/// The backlog has an issue to consider refactoring this with a policy instead.
/// </remarks>
/// <param name="next">To call to continue processing the HTTP request.</param>
/// <param name="identityProviderService">The identity provider service to get identity providers.</param>
public class IdentityProviderAuthMiddleware(RequestDelegate next, IIdentityProviderService identityProviderService)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Check 1: Only allow requests with a valid identity provider slug
        var slug = context.Request.RouteValues["slug"]?.ToString();
        if (string.IsNullOrEmpty(slug))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Identity provider slug is required.");
            return;
        }

        var identityProvider = identityProviderService.GetBySlug(slug);
        if (identityProvider == null)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("Identity provider not found.");
            return;
        }

        // Check 2: Allow requests to pass through without authentication for the following paths
        if (context.Request.Path.Equals($"/api/identity-provider/{slug}"))
        {
            await next(context);
            return;
        }

        // Check 3: Require authentication
        var authResult = await context.AuthenticateAsync(identityProvider.Slug);
        if (!authResult.Succeeded || authResult.Principal == null)
        {
            // If authentication failed, challenge the user to log in
            await context.ChallengeAsync(identityProvider.Slug);
            return;
        }

        // Save the identity provider and authentication data in context
        var authIdpData = new IdentityProviderAuthData
        {
            IdentityProvider = identityProvider,
            AccessToken = GetAccessToken(authResult),
            JwtClaims = GetJwtClaims(authResult.Principal),
        };
        context.Items["authIdpData"] = authIdpData;

        await next(context);
    }

    private Dictionary<string, string> GetJwtClaims(ClaimsPrincipal user)
    {
        return user.Claims.ToDictionary(claim => claim.Type, claim => claim.Value);
    }

    private string GetAccessToken(AuthenticateResult userResult)
    {
        return userResult.Properties!.GetTokenValue(OpenIdConnectParameterNames.AccessToken)!;
    }
}