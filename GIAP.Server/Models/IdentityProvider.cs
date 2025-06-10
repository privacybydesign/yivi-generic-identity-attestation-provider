namespace GIAP.Server.Models;

/// <summary>
/// Represents a single identity provider configuration from identity-providers.json.
/// </summary>
public class IdentityProvider
{
    /// <summary>
    /// The name used to display within the UI.
    /// It doesn't have to be unique like the Slug.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Slug must be unique across all identity providers.
    /// Used for web url paths and as the OpenID Connect scheme name.
    /// For example, "ms-entra-id-company-name".
    /// </summary>
    public required string Slug { get; init; }

    /// <summary>
    /// The full URL to the .well-known/openid-configuration endpoint of the identity provider.
    /// For example, "https://example.com/.well-known/openid-configuration".
    /// </summary>
    public required string OpenIdWellKnownUrl { get; init; }

    /// <summary>
    /// The client ID registered at the identity provider.
    /// </summary>
    public required string ClientId { get; init; }

    /// <summary>
    /// The client secret registered at the identity provider.
    /// </summary>
    public required string ClientSecret { get; init; }

    /// <summary>
    /// The callback path registered at the identity provider.
    /// Used to redirect the user to after successful authentication. Also known as the redirect URI.
    /// Must be unique for each identity provider due to Microsoft.AspNetCore.Authentication.OpenIdConnect.
    /// </summary>
    public required string CallbackPath { get; init; }

    /// <summary>
    /// The scheme path used to build the credential scheme URL.
    /// For example, "pbdf/Issues/example/description.xml"
    /// </summary>
    public required string SchemePath { get; init; }

    /// <summary>
    /// Validity in months of the issued credential in the Yivi app.
    /// </summary>
    public required int IssuanceValidityInMonths { get; init; }

    /// <summary>
    /// Mapping of attribute ID's received from the identity provider to the attributes used in the credential.
    /// For example, the key "given_name" will be mapped to the value "givenName".
    /// The JWT the identity provider returns is expected to return key-value pairs.
    /// </summary>
    /// <remarks>
    /// To support getting nested values, in the future this could also include JSON pointers or similar.
    /// </remarks>
    public required Dictionary<string, string> AttributeMapping { get; init; }

    /// <summary>
    /// Optional list of API URLs to get additional attributes required for issuing a credential.
    /// The access token from the identity provider will be used to call these API URLs.
    /// The API URLs are expected to return JSON with key-value pairs.
    /// </summary>
    public List<string>? ApiUrls { get; init; }
}