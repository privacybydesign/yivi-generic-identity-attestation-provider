namespace GIAP.Server.Services;

/// <summary>
/// Wrapper class to get env variables.
/// </summary>
/// <remarks>
/// Testing becomes easier with this class.
/// It's also a safer way to access environment variables.
/// </remarks>
public interface IEnvVariables
{
    /// <summary>
    /// Check if required environment variables exist.
    /// </summary>
    /// <exception cref="InvalidDataException">Throw an exception if any required env variables are missing.</exception>
    /// <remarks>
    /// This should be called at the start of program.cs to ensure that all required environment variables are set.
    /// </remarks>
    void Verify();
    
    /// <summary>
    /// Scheme base URL used to build the full URL for the scheme.
    /// A scheme base URL looks like: "https://schemes.staging.yivi.app".
    /// </summary>
    /// <returns>Scheme base URL.</returns>
    string GetSchemeBaseUrl();

    /// <summary>
    /// Scheme name used to build the full URL for the scheme.
    /// </summary>
    /// <returns></returns>
    string GetSchemeName();

    /// <summary>
    /// IRMA Server Base URL to build session URLs for credential issuance.
    /// </summary>
    /// <returns>IRMA Server Base URL.</returns>
    string GetIrmaServerBaseUrl();

    /// <summary>
    /// IRMA Server API token to authenticate with the IRMA Server.
    /// </summary>
    /// <returns>IRMA Server API token.</returns>
    string GetIrmaServerApiToken();
}