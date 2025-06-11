namespace GIAP.Server.Services;

public interface IAttributeMapperService
{
    /// <summary>
    /// Initializes the attributes for a credential with JWT claims, optionally combined with API data.
    /// </summary>
    /// <param name="jwtClaims">JWT Claims consists of ids with values</param>
    /// <param name="apiData">API data consists of ids with values</param>
    /// <returns>One dictionary with JWT claims, optionally combined with API data</returns>
    Dictionary<string, string> InitializeAttributes(
        Dictionary<string, string> jwtClaims,
        Dictionary<string, string>? apiData
    );

    /// <summary>
    /// Filters and maps the attributes based on identityProviderAttributeMapping.
    /// It skips the attributes that are not in the identityProviderAttributeMapping.
    /// </summary>
    /// <remarks>
    /// It assumes that the keys in identityProviderAttributeMapping are the keys received from the identity provider
    /// or from a resource server (API), and it assumes the values in identityProviderAttributeMapping are the scheme IDs.
    /// </remarks>
    /// <param name="attributes">Attributes that aren't filtered or mapped to scheme IDs yet</param>
    /// <param name="identityProviderAttributeMapping">Dictionary keys are IdP/API keys and values are scheme IDs</param>
    /// <returns>Returns a dictionary with keys as scheme IDs and the values from the attribute dictionary.</returns>
    Dictionary<string, string> FilterAndMapByConfig(
        Dictionary<string, string> attributes,
        Dictionary<string, string> identityProviderAttributeMapping
    );

    /// <summary>
    /// Replaces id's in a credential with human-readable names for display purposes.
    /// </summary>
    /// <param name="credentialAttributes">A dictionary with the keys and values of the credential.</param>
    /// <param name="schemeAttributes">The scheme attributes with keys as IDs and values as the human-readable names.</param>
    /// <returns>A dictionary with keys as human-readable names and attribute values.</returns>
    Dictionary<string, string> MapToHumanReadableAttributes(
        Dictionary<string, string> credentialAttributes,
        Dictionary<string, string> schemeAttributes
    );
}