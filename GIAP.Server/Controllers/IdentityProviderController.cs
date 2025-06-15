using GIAP.Server.Models;
using GIAP.Server.Services;
using Microsoft.AspNetCore.Mvc;

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
    IIdentityProviderService identityProviderService,
    IApiClient apiClient,
    ISchemeCredentialClient schemeCredentialClient,
    ICredentialAttributeService credentialAttributeService,
    IIrmaServerClient irmaServerClient,
    IDotNetEnvWrapper dotNetEnvWrapper,
    ILogger<IdentityProviderController> logger
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
    /// After a successful login, or if the user was already authenticated,
    /// the user is redirected to a frontend route.
    /// </summary>
    /// <remarks>
    /// This assumes the frontend navigates the user to this endpoint.
    /// This endpoint then relies on the IdentityProviderAuthMiddleware to handle the OIDC auth flow.
    /// After successful authentication, or if the user was already authenticated,
    /// the user is then redirected back to a frontend route (see main.tsx).
    /// </remarks>
    /// <param name="slug">The identity provider slug.</param>
    /// <param name="language">The language of the user, such as "en".</param>
    /// <returns>Redirect the user back to the frontend.</returns>
    [HttpGet("/api/identity-provider/{slug}/ui-login/{language}")]
    public IActionResult UiLogin(string slug, string language)
    {
        return LocalRedirect($"/{language}/{slug}/load-attributes");
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
        try
        {
            var idpAuthData = (HttpContext.Items["authIdpData"] as IdentityProviderAuthData)!; // trust middleware

            // Get scheme attributes
            var schemeBaseUrl = dotNetEnvWrapper.GetSchemeBaseUrl();
            var schemeName = dotNetEnvWrapper.GetSchemeName();
            var schemeUrl = $"{schemeBaseUrl}/{schemeName}/{idpAuthData.IdentityProvider.SchemePath}";
            var schemeAttributes = await schemeCredentialClient.GetAttributes(schemeUrl, language);

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
        catch (HttpRequestException)
        {
            return StatusCode(502, "Bad Gateway: Unable to get attributes from external services.");
        }
        catch (Exception e)
        {
            logger.LogError(e, "GetAttributes ({Slug}): Unexpected error {Error}", slug, e.Message);
            return StatusCode(500, "Internal Server Error: Something unexpected happened.");
        }
    }

    /// <summary>
    /// Start the IRMA session to issue a credential.
    /// </summary>
    /// <param name="slug">The identity provider slug.</param>
    /// <returns>Return the session pointer (QR) and frontendAuth as described in the IRMA protocol.</returns>
    [HttpGet("/api/identity-provider/{slug}/irma-endpoint/start")]
    public async Task<IActionResult> IrmaSessionStart(string slug)
    {
        try
        {
            var idpAuthData = (HttpContext.Items["authIdpData"] as IdentityProviderAuthData)!; // trust middleware
            var irmaServerBaseUrl = dotNetEnvWrapper.GetIrmaServerBaseUrl();
            var irmaServerApiToken = dotNetEnvWrapper.GetIrmaServerApiToken();
            var schemeName = dotNetEnvWrapper.GetSchemeName();

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
            var irmaServerResponseWithoutToken = new
            {
                irmaServerResponse.FrontendRequest,
                irmaServerResponse.SessionPtr
            };

            return Ok(irmaServerResponseWithoutToken);
        }
        catch (HttpRequestException)
        {
            return StatusCode(502,
                "Bad Gateway: Unable to get data for issuance or failed to issue the credential to the IRMA server.");
        }
        catch (Exception e)
        {
            logger.LogError(e, "IrmaSessionStart ({Slug}): Unexpected error {Error}", slug, e.Message);
            return StatusCode(500, "Internal Server Error: Something unexpected happened.");
        }
    }
}