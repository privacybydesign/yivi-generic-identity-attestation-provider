namespace GIAP.Server.Services;

public interface ICredentialAttributeService
{
    /// <summary>
    /// Get credential attributes for issuance based on the attribute mapping.
    /// </summary>
    /// <param name="idpAttributesMapping">The attribute mapping from the IdentityProvider configuration.</param>
    /// <param name="jwtClaims">JWT claims from the IdentityProvider</param>
    /// <param name="apiData">API data from the API</param>
    /// <returns>Returns the credential attributes for issuance.</returns>
    Dictionary<string, string> GetCredentialAttributesForIssuance(
        Dictionary<string, string> idpAttributesMapping,
        Dictionary<string, string> jwtClaims,
        Dictionary<string, string>? apiData
    );

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
    Dictionary<string, string> GetCredentialAttributesForDisplay(
        Dictionary<string, string> schemeAttributes,
        Dictionary<string, string> idpAttributesMapping,
        Dictionary<string, string> jwtClaims,
        Dictionary<string, string>? apiData
    );
}