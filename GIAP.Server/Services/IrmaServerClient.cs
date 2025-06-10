namespace GIAP.Server.Services;

/// <summary>
/// The IrmaServerClient is used to issue credentials to the IRMA server.
/// </summary>
/// <param name="httpClient">The HTTP client to call the IRMA server endpoint.</param>
public class IrmaServerClient(HttpClient httpClient)
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
    public async Task<string> IssueCredential(
        string irmaServerBaseUrl,
        string irmaServerApiToken,
        string schemeName,
        string schemePath,
        int issuanceValidityInMonths,
        Dictionary<string, string> attributes
    )
    {
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", irmaServerApiToken);

        // pbdf/Issues/microsoftEntraIdReference/description.xml -> pbdf.microsoftEntraIdReference
        var splitSchemePath = schemePath.Split("/");
        var partCredentialId = splitSchemePath[0] + "." + splitSchemePath[2];

        var credentialId = $"{schemeName}.{partCredentialId}"; // pbdf-staging.pbdf.microsoftEntraIdReference

        var irmaServerUrl = irmaServerBaseUrl + "/session";
        var validity = DateTimeOffset.UtcNow.AddMonths(issuanceValidityInMonths).ToUnixTimeSeconds();

        var requestData = new Dictionary<string, object>
        {
            ["@context"] = "https://irma.app/ld/request/issuance/v2",
            ["credentials"] = new[]
            {
                new
                {
                    credential = credentialId,
                    validity,
                    attributes
                }
            }
        };

        var response = await httpClient.PostAsJsonAsync(irmaServerUrl, requestData);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        return json;
    }
}