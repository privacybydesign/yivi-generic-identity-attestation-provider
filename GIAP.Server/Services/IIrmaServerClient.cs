using GIAP.Server.Models;

namespace GIAP.Server.Services;

/// <summary>
/// The IrmaServerClient is used to issue credentials to the IRMA server.
/// </summary>
public interface IIrmaServerClient
{
    /// <summary>
    /// Issue a credential to the IRMA server.
    /// </summary>
    /// <param name="irmaServerBaseUrl">The IRMA base url such as "https://is.staging.yivi.app".</param>
    /// <param name="irmaServerApiToken">The required API token.</param>
    /// <param name="schemeName">The scheme name such as "pbdf-staging".</param>
    /// <param name="schemePath">The scheme name as defined in the IdentityProvider class.</param>
    /// <param name="issuanceValidityInMonths">The issuanceValidityInMonths as defined in the IdentityProvider class.</param>
    /// <param name="attributes">The attributes with keys as IDs mapped to their values.</param>
    /// <returns>Returns the response of the IRMA Server.</returns>
    Task<IssuanceResponse> IssueCredential(
        string irmaServerBaseUrl,
        string irmaServerApiToken,
        string schemeName,
        string schemePath,
        int issuanceValidityInMonths,
        Dictionary<string, string> attributes
    );
}