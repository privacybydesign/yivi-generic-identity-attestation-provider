namespace GIAP.Server.Models;

/// <summary>
/// The authentication data for a user authenticated with a specific identity provider.
/// </summary>
public class IdentityProviderAuthData
{
    /// <summary>
    /// The identity provider the user is authenticated with.
    /// </summary>
    public required IdentityProvider IdentityProvider { get; init; }

    /// <summary>
    /// The access token received from the identity provider.
    /// </summary>
    public required string AccessToken { get; init; }

    /// <summary>
    /// The JWT claims from the identity provider.
    /// </summary>
    public required Dictionary<string, string> JwtClaims { get; init; }
}