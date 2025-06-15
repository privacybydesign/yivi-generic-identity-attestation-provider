using System.Xml;
using System.Xml.Linq;

namespace GIAP.Server.Services;

/// <inheritdoc/>
public class SchemeCredentialClient(HttpClient httpClient, ILogger<SchemeCredentialClient> logger)
    : ISchemeCredentialClient
{
    /// <inheritdoc/>
    public async Task<Dictionary<string, string>> GetAttributes(string schemeUrl, string language)
    {
        httpClient.DefaultRequestHeaders.Clear();

        try
        {
            var response = await httpClient.GetAsync(schemeUrl);
            response.EnsureSuccessStatusCode();

            var xml = await response.Content.ReadAsStringAsync();
            return XmlToDictionary(xml, language);
        }
        catch (HttpRequestException e)
        {
            logger.LogError(e, "SchemeCredentialClient: HTTP request failed ({StatusCode}) for URL: {Url}",
                e.StatusCode, schemeUrl);
            throw;
        }
        catch (Exception e) when (e is XmlException or ArgumentNullException)
        {
            logger.LogError(e, "SchemeCredentialClient: XML deserialization failed for URL: {Url}", schemeUrl);
            throw;
        }
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