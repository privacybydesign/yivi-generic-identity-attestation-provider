using DotNetEnv;

namespace GIAP.Server.Services;

/// <inheritdoc/>
public class DotNetDotNetEnvWrapperWrapper : IDotNetEnvWrapper
{
    private const string SchemeBaseUrlKey = "SCHEME_BASE_URL";
    private const string SchemeNameKey = "SCHEME_NAME";
    private const string IrmaServerBaseUrlKey = "IRMA_SERVER_BASE_URL";
    private const string IrmaServerApiTokenKey = "IRMA_SERVER_API_TOKEN";

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
        return Env.GetString(key);
    }
}