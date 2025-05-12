namespace GIAP.Server.Models;

// Do not make this abstract, JSON deserialization in IdentityProviderService depends on this
public class IdentityProvider
{
    public required string Name { get; init; }
    public required string Slug { get; init; }
}