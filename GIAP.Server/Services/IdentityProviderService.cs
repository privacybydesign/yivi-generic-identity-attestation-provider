using System.Text.Json;
using GIAP.Server.Models;

namespace GIAP.Server.Services;

/**
 * Class is used as a singleton, see Program.cs
 */
public class IdentityProviderService(IFileSystem fileSystem) : IIdentityProviderService
{
    private List<IdentityProvider> _identityProviders = [];

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

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
                throw new InvalidOperationException("No identity providers found");
            }
        }
        catch (JsonException e)
        {
            throw new InvalidDataException("Invalid JSON and/or missing keys in identity providers file", e);
        }
    }

    public IdentityProvider? GetBySlug(string slug) => _identityProviders.FirstOrDefault(idp => idp.Slug == slug);
}