using System.Text.Json;
using GIAP.Server.Models;

namespace GIAP.Server.Services;

/// <summary>
/// Service for managing identity providers from configuration.
/// Class is used as a singleton, see Program.cs.
/// </summary>
/// <param name="fileSystem">File system used to read the identity providers file.</param>
public class IdentityProviderService(IFileSystem fileSystem)
{
    private List<IdentityProvider> _identityProviders = [];

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    /// <summary>
    /// Initializes identity providers from configuration.
    /// </summary>
    /// <exception cref="FileNotFoundException">Thrown when no identity providers file is found.</exception>
    /// <exception cref="InvalidDataException">Thrown when the JSON is invalid or has missing keys.</exception>
    public void Initialize()
    {
        var identityProvidersFilePath = Path.Combine(
            AppContext.BaseDirectory,
            "Configuration",
            "identity-providers.json"
        );

        if (!fileSystem.Exists(identityProvidersFilePath))
            throw new FileNotFoundException("Identity providers file not found", identityProvidersFilePath);

        try
        {
            var json = fileSystem.ReadAllText(identityProvidersFilePath);
            _identityProviders = JsonSerializer.Deserialize<List<IdentityProvider>>(json, _serializerOptions) ?? [];

            if (_identityProviders.Count == 0)
            {
                throw new InvalidDataException("No identity providers found");
            }

            var slugs = new Dictionary<string, bool>();
            foreach (var identityProvider in _identityProviders)
            {
                // If a slug is already added to the dictionary, it means a duplicate exists in the idp config file
                if (slugs.ContainsKey(identityProvider.Slug))
                {
                    throw new InvalidDataException(
                        $"Duplicate slug found in identity-providers.json: {identityProvider.Slug}");
                }

                slugs[identityProvider.Slug] = true;
                throw new InvalidOperationException("No identity providers found");
            }
        }
        catch (JsonException e)
        {
            throw new InvalidDataException("Invalid JSON and/or missing keys in identity providers file", e);
        }
    }

    /// <summary>
    /// Get an identity provider by its web url slug.
    /// </summary>
    /// <param name="slug">A web url slug, for example, "idp-slug".</param>
    /// <returns>The identity provider, if none found, it returns null.</returns>
    public IdentityProvider? GetBySlug(string slug) => _identityProviders.FirstOrDefault(idp => idp.Slug == slug);
}