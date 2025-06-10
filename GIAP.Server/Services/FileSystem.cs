namespace GIAP.Server.Services;

/// <summary>
/// This uses the System.IO.File class to interact with files.
/// </summary>
public class FileSystem : IFileSystem
{
    /// <inheritdoc/>
    public bool Exists(string path)
    {
        return File.Exists(path);
    }

    /// <inheritdoc/>
    public string ReadAllText(string path)
    {
        return File.ReadAllText(path);
    }
}