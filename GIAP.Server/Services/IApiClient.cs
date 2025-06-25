namespace GIAP.Server.Services;

/// <summary>
/// The ApiClient is used to get data from an API using the access token from the identity provider.
/// </summary>
public interface IApiClient
{
    /// <summary>
    /// Get the combined API data from multiple URLs.
    /// </summary>
    /// <param name="accessToken">The access token from the identity provider</param>
    /// <param name="urls">The GET urls to get data from the API</param>
    /// <returns>The combined API data from multiple URLs.</returns>
    Task<Dictionary<string, string>> Get(string accessToken, List<string> urls);

    /// <summary>
    /// Get attributes from an API using the access token from the IdP and API url defined within the configuration.
    /// </summary>
    /// <param name="accessToken">The access token from the identity provider</param>
    /// <param name="url">The GET url to get data from the API</param>
    /// <returns>Returns the data from the API</returns>
    Task<Dictionary<string, string>> Get(string accessToken, string url);
}