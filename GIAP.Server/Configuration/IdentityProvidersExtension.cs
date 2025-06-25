using GIAP.Server.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace GIAP.Server.Configuration;

/// <summary>
/// Extension method for adding identity providers to the authentication builder.
/// </summary>
public static class IdentityProvidersExtension
{
    /// <summary>
    /// Adds the identity providers from the IdentityProviderService to the authentication builder.
    /// </summary>
    /// <param name="builder">Builder to add identity providers using AddOpenIdConnect.</param>
    /// <returns>The AuthenticationBuilder builder with all the configured identity providers.</returns>
    public static AuthenticationBuilder AddIdentityProviders(this AuthenticationBuilder builder)
    {
        var identityProviderService =
            builder.Services.BuildServiceProvider().GetRequiredService<IIdentityProviderService>();
        var identityProviders = identityProviderService.GetAll();

        foreach (var identityProvider in identityProviders)
        {
            builder.AddOpenIdConnect(identityProvider.Slug, configure =>
            {
                configure.MetadataAddress = identityProvider.OpenIdWellKnownUrl;
                configure.ClientId = identityProvider.ClientId;
                configure.ClientSecret = identityProvider.ClientSecret;
                configure.CallbackPath = identityProvider.CallbackPath;
                configure.ResponseType = OpenIdConnectResponseType.Code;
                configure.SaveTokens = true; // necessary to get the access token for optional API calls
                configure.UseTokenLifetime = true; // necessary to ensure the token has the same lifetime as the cookie
                configure.GetClaimsFromUserInfoEndpoint = true; // necessary to get the user info from the IdP
            });
        }

        return builder;
    }
}