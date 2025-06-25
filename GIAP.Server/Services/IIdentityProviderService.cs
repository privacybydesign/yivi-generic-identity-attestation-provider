using GIAP.Server.Models;

namespace GIAP.Server.Services;

/// <summary>
/// Service for managing identity providers from configuration.
/// Class is used as a singleton, see Program.cs.
/// </summary>
public interface IIdentityProviderService
{
    /// <summary>
    /// Initializes identity providers from configuration.
    /// </summary>
    /// <exception cref="FileNotFoundException">Thrown when no identity providers file is found.</exception>
    /// <exception cref="InvalidDataException">Thrown when the JSON is invalid or has missing keys.</exception>
    void Initialize();

    /// <summary>
    /// Get an identity provider by its web url slug.
    /// </summary>
    /// <param name="slug">A web url slug, for example, "idp-slug".</param>
    /// <returns>The identity provider, if none found, it returns null.</returns>
    IdentityProvider? GetBySlug(string slug);

    /// <summary>
    /// Get all identity providers.
    /// </summary>
    /// <returns>A list of all the identity providers.</returns>
    IReadOnlyList<IdentityProvider> GetAll();
}