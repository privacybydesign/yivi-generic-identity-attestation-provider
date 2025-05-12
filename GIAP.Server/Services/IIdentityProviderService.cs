using GIAP.Server.Models;

namespace GIAP.Server.Services;

public interface IIdentityProviderService
{
    void Initialize();
    IdentityProvider? GetBySlug(string slug);
}