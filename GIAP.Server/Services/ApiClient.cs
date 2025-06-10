using System.Net.Http.Headers;
using System.Text.Json;

namespace GIAP.Server.Services;

/// <summary>
/// The ApiClient is used to get data from an API using the access token from the identity provider.
/// </summary>
/// <param name="httpClient">The HTTP client used to call the API.</param>
public class ApiClient(HttpClient httpClient)
{
    /// <summary>
    /// Get the combined API data from multiple URLs.
    /// </summary>
    /// <param name="accessToken">The access token from the identity provider</param>
    /// <param name="urls">The GET urls to get data from the API</param>
    /// <returns>The combined API data from multiple URLs.</returns>
    public async Task<Dictionary<string, string>> Get(string accessToken, List<string> urls)
    {
        var combinedApiData = new Dictionary<string, string>();

        foreach (var url in urls)
        {
            var response = await Get(accessToken, url);
            foreach (var (attributeId, attributeValue) in response)
            {
                combinedApiData.TryAdd(attributeId, attributeValue);
            }
        }

        return combinedApiData;
    }

    /// <summary>
    /// Get attributes from an API using the access token from the IdP and API url defined within the configuration.
    /// </summary>
    /// <param name="accessToken">The access token from the identity provider</param>
    /// <param name="url">The GET url to get data from the API</param>
    /// <returns>Returns the data from the API</returns>
    public async Task<Dictionary<string, string>> Get(string accessToken, string url)
    {
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();

        return JsonToDictionary(json);
    }

    private Dictionary<string, string> JsonToDictionary(string json)
    {
        // Assuming the API Resource response is always a JSON with string keys and values.
        // Backlog has an issue to make this more generic (for example, by using JSON pointers if it has nested values).
        return JsonSerializer.Deserialize<Dictionary<string, string>>(json)!;
    }
}