using GIAP.Server.Services;
using Shouldly;
using NSubstitute;

namespace GIAP.Tests;

public class IdentityProviderServiceTests
{
    /// <summary>
    /// Given no identity providers file
    /// When the service is initialized,
    /// Then an exception is thrown 
    /// </summary>
    [Fact]
    public void NoIdentityProviderJsonFound()
    {
        var fileSystem = Substitute.For<IFileSystem>();
        var expectedPath = Path.Combine(
            AppContext.BaseDirectory,
            "Configuration",
            "identity-providers.json"
        );

        fileSystem.Exists(expectedPath).Returns(false);

        var service = new IdentityProviderService(fileSystem);

        Should.Throw<FileNotFoundException>(() => service.Initialize()).Message
            .ShouldBe("Identity providers file not found");
    }

    /// <summary>
    /// Given an empty identity providers file
    /// When the service is initialized,
    /// Then an exception is thrown
    /// </summary>
    [Fact]
    public void NoIdentityProvidersFound()
    {
        var fileSystem = Substitute.For<IFileSystem>();
        var expectedPath = Path.Combine(
            AppContext.BaseDirectory,
            "Configuration",
            "identity-providers.json"
        );

        fileSystem.Exists(expectedPath).Returns(true);
        fileSystem.ReadAllText(expectedPath).Returns("[]");

        var service = new IdentityProviderService(fileSystem);
        Should.Throw<InvalidDataException>(() => service.Initialize()).Message
            .ShouldBe("No identity providers found");
    }


    /// <summary>
    /// Given a JSON identity providers file with missing keys
    /// When the service is initialized,
    /// Then an exception is thrown
    /// </summary>
    [Fact]
    public void IdentityProviderJsonWithMissingKeys()
    {
        var fileSystem = Substitute.For<IFileSystem>();
        var expectedPath = Path.Combine(
            AppContext.BaseDirectory,
            "Configuration",
            "identity-providers.json"
        );

        fileSystem.Exists(expectedPath).Returns(true);

        // JSON doesn't include all the keys from the IdentityProvider model
        const string json = """
                            [
                                {
                                  "slug": "example-1"
                                },
                                {
                                  "slug": "example-2"
                                }
                            ]
                            """;

        fileSystem.ReadAllText(expectedPath).Returns(json);

        var service = new IdentityProviderService(fileSystem);

        Should.Throw<InvalidDataException>(() => service.Initialize()).Message
            .ShouldBe("Invalid JSON and/or missing keys in identity providers file");
    }

    /// <summary>
    /// Given a malformed JSON identity providers file
    /// When the service is initialized,
    /// Then an exception is thrown
    /// </summary>
    [Fact]
    public void MalformedIdentityProviderJson()
    {
        var fileSystem = Substitute.For<IFileSystem>();
        var expectedPath = Path.Combine(
            AppContext.BaseDirectory,
            "Configuration",
            "identity-providers.json"
        );

        fileSystem.Exists(expectedPath).Returns(true);

        const string json = """
                            [
                                {
                                  "name": "Example Identity Provider 1",
                                  "slug": "example-1"
                                },
                                {
                                  "name": "Example
                            """;

        fileSystem.ReadAllText(expectedPath).Returns(json);

        var service = new IdentityProviderService(fileSystem);

        Should.Throw<InvalidDataException>(() => service.Initialize()).Message
            .ShouldBe("Invalid JSON and/or missing keys in identity providers file");
    }

    /// <summary>
    /// Given a valid identity providers file
    /// When the service is initialized,
    /// Then no exception is thrown
    /// </summary>
    [Fact]
    public void JsonMeetsRequirements()
    {
        var fileSystem = Substitute.For<IFileSystem>();
        var expectedPath = Path.Combine(
            AppContext.BaseDirectory,
            "Configuration",
            "identity-providers.json"
        );

        fileSystem.Exists(expectedPath).Returns(true);

        const string json = """
                            [
                              {
                                "name": "Example Identity Provider 1",
                                "slug": "example-1",
                                "openIdWellKnownUrl": "https://example.local/.well-known/openid-configuration",
                                "clientId": "123-456-789",
                                "clientSecret": "ABC-123-XYZ",
                                "callbackPath": "/api/callback-signin-example-1",
                                "apiUrls": [
                                  "https://api.example.local/me?fields=id,given_name,family_name,companyName"
                                ],
                                "schemePath": "pbdf/Issues/example1/description.xml",
                                "issuanceValidityInMonths": 6,
                                "attributeMapping": {
                                  "id": "id",
                                  "given_name": "givenName",
                                  "family_name": "surname",
                                  "companyName": "companyName"
                                }
                              },
                              {
                                "name": "Example Identity Provider 2",
                                "slug": "example-2",
                                "openIdWellKnownUrl": "https://example.local/.well-known/openid-configuration",
                                "clientId": "987-654-321",
                                "clientSecret": "ZYX-321-CBA",
                                "callbackPath": "/api/callback-signin-example-2",
                                "schemePath": "pbdf/Issues/example2/description.xml",
                                "issuanceValidityInMonths": 6,
                                "attributeMapping": {
                                  "http://example.local/claims/id": "id",
                                  "given_name": "givenName",
                                  "family_name": "surname"
                                }
                              }
                            ]
                            """;

        fileSystem.ReadAllText(expectedPath).Returns(json);

        var service = new IdentityProviderService(fileSystem);
        Should.NotThrow(() => service.Initialize());
        service.GetBySlug("example-1")?.Name.ShouldBe("Example Identity Provider 1");
        service.GetBySlug("example-2")?.Name.ShouldBe("Example Identity Provider 2");
    }

    /// <summary>
    /// Given an invalid identity providers file with a duplicate slug
    /// When the service is initialized,
    /// Then an exception is thrown with the duplicate slug name
    /// </summary>
    [Fact]
    public void Initialize_WithDuplicateSlug_ThrowsException()
    {
        var fileSystem = Substitute.For<IFileSystem>();
        var expectedPath = Path.Combine(
            AppContext.BaseDirectory,
            "Configuration",
            "identity-providers.json"
        );

        fileSystem.Exists(expectedPath).Returns(true);

        // JSON with a duplicate slug
        const string json = """
                            [
                              {
                                "name": "Example Identity Provider 1",
                                "slug": "example-1",
                                "openIdWellKnownUrl": "https://example.local/.well-known/openid-configuration",
                                "clientId": "123-456-789",
                                "clientSecret": "ABC-123-XYZ",
                                "callbackPath": "/api/callback-signin-example-1",
                                "schemePath": "pbdf/Issues/example1/description.xml",
                                "issuanceValidityInMonths": 6,
                                "attributeMapping": {
                                  "id": "id",
                                  "given_name": "givenName"
                                }
                              },
                              {
                                "name": "Example Identity Provider 1",
                                "slug": "example-1",
                                "openIdWellKnownUrl": "https://example.local/.well-known/openid-configuration",
                                "clientId": "123-456-789",
                                "clientSecret": "ABC-123-XYZ",
                                "callbackPath": "/api/callback-signin-example-1",
                                "schemePath": "pbdf/Issues/example1/description.xml",
                                "issuanceValidityInMonths": 6,
                                "attributeMapping": {
                                  "id": "id",
                                  "given_name": "givenName"
                                }
                              }
                            ]
                            """;

        fileSystem.ReadAllText(expectedPath).Returns(json);

        var service = new IdentityProviderService(fileSystem);

        Should.Throw<InvalidDataException>(() => service.Initialize()).Message
            .ShouldBe(
                "Duplicate slug found in identity-providers.json: example-1. Slugs are counted as duplicate regardless of case sensitivity, a configured identity-providers.json with the slugs 'example' and 'Example' will cause an exception to be thrown.");
    }

    /// <summary>
    /// Given an invalid identity providers file with a duplicate slug
    /// When the service is initialized,
    /// Then an exception is thrown with the duplicate slug name
    /// </summary>
    /// <remarks>
    /// This one is different from `Initialize_WithDuplicateSlug_ThrowsException` because it checks for case insensitivity.
    /// It should also throw an exception when the slugs are the same but differ in case, for example, "example-1" and "EXAMPLE-1".
    /// </remarks>
    [Fact]
    public void Initialize_WithDuplicateSlug_ThrowsException_CaseInsensitiveVersion()
    {
        var fileSystem = Substitute.For<IFileSystem>();
        var expectedPath = Path.Combine(
            AppContext.BaseDirectory,
            "Configuration",
            "identity-providers.json"
        );

        fileSystem.Exists(expectedPath).Returns(true);

        // JSON with a duplicate slug
        const string json = """
                            [
                              {
                                "name": "Example Identity Provider 1",
                                "slug": "example-1",
                                "openIdWellKnownUrl": "https://example.local/.well-known/openid-configuration",
                                "clientId": "123-456-789",
                                "clientSecret": "ABC-123-XYZ",
                                "callbackPath": "/api/callback-signin-example-1",
                                "schemePath": "pbdf/Issues/example1/description.xml",
                                "issuanceValidityInMonths": 6,
                                "attributeMapping": {
                                  "id": "id",
                                  "given_name": "givenName"
                                }
                              },
                              {
                                "name": "Example Identity Provider 1",
                                "slug": "EXAMPLE-1",
                                "openIdWellKnownUrl": "https://example.local/.well-known/openid-configuration",
                                "clientId": "123-456-789",
                                "clientSecret": "ABC-123-XYZ",
                                "callbackPath": "/api/callback-signin-example-1",
                                "schemePath": "pbdf/Issues/example1/description.xml",
                                "issuanceValidityInMonths": 6,
                                "attributeMapping": {
                                  "id": "id",
                                  "given_name": "givenName"
                                }
                              }
                            ]
                            """;

        fileSystem.ReadAllText(expectedPath).Returns(json);

        var service = new IdentityProviderService(fileSystem);

        Should.Throw<InvalidDataException>(() => service.Initialize()).Message
            .ShouldBe(
                "Duplicate slug found in identity-providers.json: EXAMPLE-1. Slugs are counted as duplicate regardless of case sensitivity, a configured identity-providers.json with the slugs 'example' and 'Example' will cause an exception to be thrown.");
    }
}