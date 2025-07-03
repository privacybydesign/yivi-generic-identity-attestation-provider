namespace GIAP.Server.Services;

/// <inheritdoc/>
public class EnvironmentWrapper : IEnvironment
{
    /// <inheritdoc/>
    public string? Get(string key)
    {
        return Environment.GetEnvironmentVariable(key);
    }
}