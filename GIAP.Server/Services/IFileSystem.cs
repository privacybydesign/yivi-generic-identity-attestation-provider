namespace GIAP.Server.Services;

/// <summary>
/// Interface for file system operations.
/// Necessary for mocking in tests.
/// </summary>
public interface IFileSystem
{
    /// <summary>
    /// Check if a file exists at the specified path.
    /// </summary>
    /// <param name="path">The path where the file is stored.</param>
    /// <returns>Either true or false depending on if the file exists.</returns>
    bool Exists(string path);

    /// <summary>
    /// Read all text from a file at the specified path.
    /// </summary>
    /// <param name="path">The path where the file is stored.</param>
    /// <returns>Returns the text from the specified file.</returns>
    string ReadAllText(string path);
}