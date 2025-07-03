namespace GIAP.Server.Services;

/// <summary>
/// Interface for environment operations.
/// Necessary for mocking in tests.
/// </summary>
public interface IEnvironment
{
    /// <summary>
    /// Get the value of an environment variable.
    /// </summary>
    /// <param name="key">The key of the environment variable.</param>
    /// <returns>The value of the environment variable.</returns>
    string? Get(string key);
}