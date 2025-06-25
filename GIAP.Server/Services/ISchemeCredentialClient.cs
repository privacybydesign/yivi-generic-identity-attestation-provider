namespace GIAP.Server.Services;

/// <summary>
/// The SchemeCredentialClient is used to get credential from the scheme.
/// </summary>
public interface ISchemeCredentialClient
{
    /// <summary>
    /// Get the attributes of a credential scheme by its URL.
    /// Maps the XML response to a dictionary with the attribute id as keys and the display name as values.
    /// </summary>
    /// <param name="schemeUrl">The full scheme url.</param>
    /// <param name="language">The language of the requested display names such as "en".</param>
    /// <returns>A dictionary with the attribute id as keys and the display name as values.</returns>
    Task<Dictionary<string, string>> GetAttributes(string schemeUrl, string language);
}