using System.Text.Json;
using GIAP.Server.Models;

namespace GIAP.Server.Services;

/// <inheritdoc/>
public class IdentityProviderService(IFileSystem fileSystem) : IIdentityProviderService
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private List<IdentityProvider> _identityProviders = [];

    /// <inheritdoc/>
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
            }
        }
        catch (JsonException e)
        {
            throw new InvalidDataException("Invalid JSON and/or missing keys in identity providers file", e);
        }
    }

    /// <inheritdoc/>
    public IdentityProvider? GetBySlug(string slug) => _identityProviders.FirstOrDefault(idp => idp.Slug == slug);

    /// <inheritdoc/>
    public List<IdentityProvider> GetAll() => _identityProviders;
}