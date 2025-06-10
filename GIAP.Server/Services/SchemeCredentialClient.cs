using System.Xml.Linq;

namespace GIAP.Server.Services;

/// <summary>
/// The SchemeCredentialClient is used to get credential from the scheme.
/// </summary>
/// <param name="httpClient">The HTTP client to call the scheme endpoint.</param>
public class SchemeCredentialClient(HttpClient httpClient)
{
    /// <summary>
    /// Get the attributes of a credential scheme by its URL.
    /// Maps the XML response to a dictionary with the attribute id as keys and the display name as values.
    /// </summary>
    /// <param name="schemeUrl">The full scheme url.</param>
    /// <param name="language">The language of the requested display names such as "en".</param>
    /// <returns>A dictionary with the attribute id as keys and the display name as values.</returns>
    public async Task<Dictionary<string, string>> GetAttributes(string schemeUrl, string language)
    {
        httpClient.DefaultRequestHeaders.Clear();

        var response = await httpClient.GetAsync(schemeUrl);
        response.EnsureSuccessStatusCode();

        var xml = await response.Content.ReadAsStringAsync();
        return XmlToDictionary(xml, language);
    }

    private Dictionary<string, string> XmlToDictionary(string xml, string language)
    {
        var attributesXElement = XDocument.Parse(xml).Descendants("Attribute");
        var attributesDictionary = new Dictionary<string, string>();

        foreach (var attribute in attributesXElement)
        {
            var id = attribute.Attribute("id")!.Value; // Assuming the id attribute always exists in the scheme
            var displayName = attribute.Element("Name")!.Element(language)?.Value ??
                              attribute.Element("Name")!.Element("en")!.Value; // Assuming 'en' always exists

            attributesDictionary[id] = displayName;
        }

        return attributesDictionary;
    }
}