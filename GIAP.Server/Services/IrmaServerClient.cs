namespace GIAP.Server.Services;

/// <inheritdoc/>
public class IrmaServerClient(HttpClient httpClient, ILogger<IrmaServerClient> logger) : IIrmaServerClient
{
    /// <inheritdoc/>
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
        logger.LogInformation(
            "IrmaServerBaseUrl: {IrmaServerBaseUrl}, " +
            "SchemeName: {SchemeName}, " +
            "SchemePath: {SchemePath}",
            irmaServerBaseUrl, schemeName, schemePath
        );
        logger.LogInformation("IRMA Server URL: {IrmaServerUrl}", irmaServerUrl);
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