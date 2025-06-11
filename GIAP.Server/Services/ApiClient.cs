using System.Net.Http.Headers;
using System.Text.Json;

namespace GIAP.Server.Services;

/// <inheritdoc/>
public class ApiClient(HttpClient httpClient) : IApiClient
{
    /// <inheritdoc/>
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

    /// <inheritdoc/>
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