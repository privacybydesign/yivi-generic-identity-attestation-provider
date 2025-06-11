using System.Security.Claims;
using System.Text.Json;
using GIAP.Server.Models;
using GIAP.Server.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace GIAP.Server.Controllers;

/// <summary>
/// API controller for identity providers for the frontend.
/// </summary>
/// <remarks>
/// IdentityProviderAuth could be replaced with middleware to further reduce code duplication.
/// </remarks>
/// <param name="identityProviderService">To get an identity provider configuration.</param>
/// <param name="apiClient">To get API data.</param>
/// <param name="schemeCredentialClient">To get the scheme credential data.</param>
/// <param name="credentialAttributeService">To get the credential attributes.</param>
/// <param name="irmaServerClient">To issue the credential.</param>
[ApiController]
public class IdentityProviderController(
    IdentityProviderService identityProviderService,
    ApiClient apiClient,
    SchemeCredentialClient schemeCredentialClient,
    ICredentialAttributeService credentialAttributeService,
    IrmaServerClient irmaServerClient
) : ControllerBase
{
    /// <summary>
    /// Get the name of the identity provider by its slug.
    /// </summary>
    /// <param name="slug">The identity provider slug.</param>
    /// <returns>Returns identity provider info.</returns>
    [HttpGet("/api/identity-provider/{slug}")]
    public IActionResult Get(string slug)
    {
        var identityProvider = identityProviderService.GetBySlug(slug);
        if (identityProvider == null) return NotFound();

        return Ok(new
        {
            identityProvider.Name,
            identityProvider.Slug
        });
    }

    /// <summary>
    /// Logs the user in at the identity provider.
    /// </summary>
    /// <param name="slug">The identity provider slug.</param>
    /// <param name="language">The language of the user, such as "en".</param>
    /// <returns>LocalRedirect the user back to the frontend.</returns>
    [HttpGet("/api/identity-provider/{slug}/ui-login/{language}")]
    public async Task<IActionResult> UiLogin(string slug, string language)
    {
        var (failureResponse, idpAuthData) = await IdentityProviderAuth(HttpContext, slug);
        if (failureResponse != null || idpAuthData == null) return failureResponse!;
     
        // todo debugging
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        return Redirect($"{baseUrl}/{language}/{slug}/load-attributes");
        // return LocalRedirect($"/{language}/{slug}/load-attributes");
    }

    /// <summary>
    /// Get the attributes the user received that they can use to issue a credential.
    /// </summary>
    /// <param name="slug">The identity provider slug.</param>
    /// <param name="language">The language of the user, such as "en".</param>
    /// <returns>The attributes with human-readable names and values.</returns>
    [HttpGet("/api/identity-provider/{slug}/attributes/{language}")]
    public async Task<IActionResult> GetAttributes(string slug, string language)
    {
        var (failureResponse, idpAuthData) = await IdentityProviderAuth(HttpContext, slug);
        if (failureResponse != null || idpAuthData == null) return failureResponse!;

        // Get scheme attributes
        var schemeUrl = DotNetEnv.Env.GetString("SCHEME_BASE_URL");
        var scheme = DotNetEnv.Env.GetString("SCHEME_NAME");
        var fullSchemeUrl = $"{schemeUrl}/{scheme}/{idpAuthData.IdentityProvider.SchemePath}";
        var schemeAttributes = await schemeCredentialClient.GetAttributes(fullSchemeUrl, language);

        // Get API data
        var apiUrls = idpAuthData.IdentityProvider.ApiUrls;
        var apiData = new Dictionary<string, string>();
        if (apiUrls != null)
        {
            apiData = await apiClient.Get(idpAuthData.AccessToken, apiUrls);
        }

        var credentialForDisplay = credentialAttributeService.GetCredentialAttributesForDisplay(
            schemeAttributes,
            idpAuthData.IdentityProvider.AttributeMapping,
            idpAuthData.JwtClaims,
            apiData
        );

        var response = new
        {
            idpAuthData.IdentityProvider.Name,
            idpAuthData.IdentityProvider.Slug,
            Attributes = credentialForDisplay
        };

        return Ok(response);
    }

    /// <summary>
    /// Start the IRMA session to issue a credential.
    /// </summary>
    /// <param name="slug">The identity provider slug.</param>
    /// <returns>Return the session pointer (QR) and frontendAuth as described in the IRMA protocol.</returns>
    [HttpGet("/api/identity-provider/{slug}/irma-endpoint/start")]
    public async Task<IActionResult> IrmaSessionStart(string slug)
    {
        var (failureResponse, idpAuthData) = await IdentityProviderAuth(HttpContext, slug);
        if (failureResponse != null || idpAuthData == null) return failureResponse!;

        var irmaServerBaseUrl = DotNetEnv.Env.GetString("IRMA_SERVER_BASE_URL");
        var irmaServerApiToken = DotNetEnv.Env.GetString("IRMA_SERVER_API_TOKEN");
        var schemeName = DotNetEnv.Env.GetString("SCHEME_NAME");

        var apiData = new Dictionary<string, string>();
        if (idpAuthData.IdentityProvider.ApiUrls != null)
        {
            apiData = await apiClient.Get(idpAuthData.AccessToken, idpAuthData.IdentityProvider.ApiUrls);
        }

        var credential = credentialAttributeService.GetCredentialAttributesForIssuance(
            idpAuthData.IdentityProvider.AttributeMapping,
            idpAuthData.JwtClaims,
            apiData
        );

        var irmaServerResponse = await irmaServerClient.IssueCredential(
            irmaServerBaseUrl,
            irmaServerApiToken,
            schemeName,
            idpAuthData.IdentityProvider.SchemePath,
            idpAuthData.IdentityProvider.IssuanceValidityInMonths,
            credential
        );

        // As described in the IRMA protocol, remove the token from the IRMA Server response to the frontend
        var irmaServerResponseWithoutToken = JsonSerializer.Deserialize<Dictionary<string, object>>(irmaServerResponse);
        irmaServerResponseWithoutToken!.Remove("token"); // assume the response succeeds with token

        return Ok(irmaServerResponseWithoutToken);
    }

    /// <summary>
    /// Checks if the identity provider exists and if the user is authenticated through it.
    /// If authentication fails, it challenges the user to log in at the identity provider.
    /// </summary>
    /// <param name="httpContext">The HTTP context to log in the user at the identity provider.</param>
    /// <param name="slug">The slug of the identity provider.</param>
    /// <returns>Returns a tuple with either a failure response or IdentityProviderAuthData if successful.</returns>
    private async Task<(IActionResult? failureResponse, IdentityProviderAuthData? authIdpData)> IdentityProviderAuth(
        HttpContext httpContext,
        string slug
    )
    {
        // Check if the identity provider exists
        var identityProvider = identityProviderService.GetBySlug(slug);
        if (identityProvider == null) return (NotFound(), null);

        // Authenticate
        var authResult = await httpContext.AuthenticateAsync(identityProvider.Slug);
        if (!authResult.Succeeded || authResult.Principal == null)
        {
            // If authentication failed, challenge the user to log in
            return (Challenge(identityProvider.Slug), null);
        }

        return (null, new IdentityProviderAuthData
        {
            IdentityProvider = identityProvider,
            AccessToken = GetAccessToken(authResult),
            JwtClaims = GetJwtClaims(authResult.Principal),
        });
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