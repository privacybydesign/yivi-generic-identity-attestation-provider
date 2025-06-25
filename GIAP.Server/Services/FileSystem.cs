namespace GIAP.Server.Services;

/// <inheritdoc/>
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