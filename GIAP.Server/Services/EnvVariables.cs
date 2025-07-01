namespace GIAP.Server.Services;

/// <inheritdoc/>
public class EnvVariables : IEnvVariables
{
    private const string SchemeBaseUrlKey = "SCHEME_BASE_URL";
    private const string SchemeNameKey = "SCHEME_NAME";
    private const string IrmaServerBaseUrlKey = "IRMA_SERVER_BASE_URL";
    private const string IrmaServerApiTokenKey = "IRMA_SERVER_API_TOKEN";

    private IReadOnlyList<string> RequiredEnvironmentVariables { get; } =
    [
        SchemeBaseUrlKey,
        SchemeNameKey,
        IrmaServerBaseUrlKey,
        IrmaServerApiTokenKey
    ];

    /// <summary>
    /// Check if required environment variables exist.
    /// </summary>
    /// <exception cref="InvalidDataException">Throw an exception if any required env variables are missing.</exception>
    /// <remarks>
    /// This should be called at the start of program.cs to ensure that all required environment variables are set.
    /// </remarks>
    public EnvVariables()
    {
        var missingVariables = new List<string>();
        foreach (var key in RequiredEnvironmentVariables)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key)))
            {
                missingVariables.Add(key);
            }
        }

        if (missingVariables.Count != 0)
        {
            throw new InvalidDataException(
                $"Not all required environment variables are set. Check the documentation README.md for the required environment variables. Make sure the .env is located at GIAP.server/.env. The following environment variables are missing: {string.Join(", ", missingVariables)}.");
        }
    }

    /// <inheritdoc/>
    public string GetSchemeBaseUrl()
    {
        return Get(SchemeBaseUrlKey);
    }

    /// <inheritdoc/>
    public string GetSchemeName()
    {
        return Get(SchemeNameKey);
    }

    /// <inheritdoc/>
    public string GetIrmaServerBaseUrl()
    {
        return Get(IrmaServerBaseUrlKey);
    }

    /// <inheritdoc/>
    public string GetIrmaServerApiToken()
    {
        return Get(IrmaServerApiTokenKey);
    }

    private string Get(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Environment variable key cannot be null or empty.");
        }

        var envVarValue = Environment.GetEnvironmentVariable(key);
        if (string.IsNullOrEmpty(envVarValue))
        {
            throw new InvalidDataException($"Environment variable '{key}' is not set or is empty.");
        }

        return envVarValue;
    }
}