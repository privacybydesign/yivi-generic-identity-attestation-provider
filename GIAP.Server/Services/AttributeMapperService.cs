namespace GIAP.Server.Services;

/// <inheritdoc/>
public class AttributeMapperService : IAttributeMapperService
{
    /// <inheritdoc/>
    public Dictionary<string, string> InitializeAttributes(
        Dictionary<string, string> jwtClaims,
        Dictionary<string, string>? apiData
    )
    {
        return apiData == null
            ? new Dictionary<string, string>(jwtClaims)
            : CombineJwtClaimsAndApiData(jwtClaims, apiData);
    }

    /// <inheritdoc/>
    public Dictionary<string, string> FilterAndMapByConfig(
        Dictionary<string, string> attributes,
        Dictionary<string, string> identityProviderAttributeMapping
    )
    {
        var credentialAttributes = new Dictionary<string, string>();

        foreach (var (id, value) in attributes)
        {
            // If the id of the attribute dictionary is in the IdP mapping, then it's part of the credential
            if (identityProviderAttributeMapping.TryGetValue(id, out var yiviId))
            {
                credentialAttributes.Add(yiviId, value);
            }
        }

        return credentialAttributes;
    }

    /// <inheritdoc/>
    public Dictionary<string, string> MapToHumanReadableAttributes(
        Dictionary<string, string> credentialAttributes,
        Dictionary<string, string> schemeAttributes)
    {
        var humanReadableAttributes = new Dictionary<string, string>();

        foreach (var (attributeId, attributeValue) in credentialAttributes)
        {
            if (schemeAttributes.TryGetValue(attributeId, out var humanReadableName))
            {
                humanReadableAttributes.Add(humanReadableName, attributeValue);
            }
        }

        return humanReadableAttributes;
    }

    private Dictionary<string, string> CombineJwtClaimsAndApiData(
        Dictionary<string, string> jwtClaims,
        Dictionary<string, string> apiData
    )
    {
        var combinedAttributes = new Dictionary<string, string>(jwtClaims); // initialize with jwtClaims

        foreach (var (attributeId, attributeValue) in apiData)
        {
            combinedAttributes[attributeId] = attributeValue;
        }

        return combinedAttributes;
    }
}