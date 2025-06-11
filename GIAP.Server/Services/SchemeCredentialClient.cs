using System.Xml.Linq;

namespace GIAP.Server.Services;

/// <inheritdoc/>
public class SchemeCredentialClient(HttpClient httpClient) : ISchemeCredentialClient
{
    /// <inheritdoc/>
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