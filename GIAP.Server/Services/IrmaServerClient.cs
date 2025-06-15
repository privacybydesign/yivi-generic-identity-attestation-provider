using System.Text.Json;
using GIAP.Server.Models;

namespace GIAP.Server.Services;

/// <inheritdoc/>
public class IrmaServerClient(HttpClient httpClient, ILogger<IrmaServerClient> logger) : IIrmaServerClient
{
    /// <inheritdoc/>
    public async Task<IssuanceResponse> IssueCredential(
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

        var issuanceRequest = new IssuanceRequest
        {
            Context = "https://irma.app/ld/request/issuance/v2",
            Credentials =
            [
                new Credential
                {
                    CredentialId = credentialId,
                    Validity = validity,
                    Attributes = attributes
                }
            ]
        };

        var response = await httpClient.PostAsJsonAsync(irmaServerUrl, issuanceRequest);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        return JsonToIssuanceResponse(json);
    }

    private IssuanceResponse JsonToIssuanceResponse(string json)
    {
        // Assuming the IRMA issuance response is always a JSON that can be mapped to IssuanceResponse.
        return JsonSerializer.Deserialize<IssuanceResponse>(json)!;
    }
}