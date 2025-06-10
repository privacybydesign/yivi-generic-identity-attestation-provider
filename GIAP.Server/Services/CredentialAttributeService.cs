namespace GIAP.Server.Services;

/// <summary>
/// Service for getting credential attributes for issuance and display.
/// </summary>
/// <param name="attributeMapperService">The attribute mapper.</param>
public class CredentialAttributeService(AttributeMapperService attributeMapperService)
{
    /// <summary>
    /// Get credential attributes for issuance based on the attribute mapping.
    /// </summary>
    /// <param name="idpAttributesMapping">The attribute mapping from the IdentityProvider configuration.</param>
    /// <param name="jwtClaims">JWT claims from the IdentityProvider</param>
    /// <param name="apiData">API data from the API</param>
    /// <returns>Returns the credential attributes for issuance.</returns>
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

    /// <summary>
    /// Get credential attributes for display based on the scheme attributes, IdP mapping, JWT claims, and API data.
    /// </summary>
    /// <remarks>
    /// This requires the scheme attributes for the human-readable names of the attributes.
    /// Rather than mapping the keys of the dictionary to IDs, it maps it to human-readable names.
    /// </remarks>
    /// <param name="schemeAttributes">The scheme attributes with keys as IDs and values as the human-readable names.</param>
    /// <param name="idpAttributesMapping">The attribute mapping from the IdentityProvider configuration.</param>
    /// <param name="jwtClaims">JWT Claims from</param>
    /// <param name="apiData">The API data from the API</param>
    /// <returns>Returns a dictionary with keys as human-readable names and attribute values.</returns>
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