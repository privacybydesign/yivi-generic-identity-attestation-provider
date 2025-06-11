namespace GIAP.Server.Services;

/// <summary>
/// Service for getting credential attributes for issuance and display.
/// </summary>
/// <param name="attributeMapperService">The attribute mapper.</param>
public class CredentialAttributeService(AttributeMapperService attributeMapperService) : ICredentialAttributeService
{
    /// <inheritdoc/>
    public Dictionary<string, string> GetCredentialAttributesForIssuance(
        Dictionary<string, string> idpAttributesMapping,
        Dictionary<string, string> jwtClaims,
        Dictionary<string, string>? apiData
    )
    {
        // Step 1/2: Initialize with jwtClaims (and apiData)
        var credentialAttributes = attributeMapperService.InitializeAttributes(jwtClaims, apiData);

        // Step 2/2: Filter the credential attributes using the identity provider attributes mapping
        return attributeMapperService.FilterAndMapByConfig(credentialAttributes, idpAttributesMapping);
    }

    /// <inheritdoc/>
    public Dictionary<string, string> GetCredentialAttributesForDisplay(
        Dictionary<string, string> schemeAttributes,
        Dictionary<string, string> idpAttributesMapping,
        Dictionary<string, string> jwtClaims,
        Dictionary<string, string>? apiData
    )
    {
        // Step 1/3: Initialize with jwtClaims (and apiData)
        var credentialAttributes = attributeMapperService.InitializeAttributes(jwtClaims, apiData);

        // Step 2/3: Filter the credential attributes using the identity provider attributes mapping
        credentialAttributes = attributeMapperService.FilterAndMapByConfig(credentialAttributes, idpAttributesMapping);

        // Step 3/3: Map the filtered claims to human-readable attributes using the scheme attributes
        return attributeMapperService.MapToHumanReadableAttributes(credentialAttributes, schemeAttributes);
    }
}