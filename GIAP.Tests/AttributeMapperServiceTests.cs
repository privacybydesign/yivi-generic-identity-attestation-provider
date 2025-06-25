using GIAP.Server.Services;
using Shouldly;

namespace GIAP.Tests;

public class AttributeMapperServiceTests
{
    /// <summary>
    /// Given JWT claims and API data with overlapping keys,
    /// when both dictionaries are combined,
    /// then no exception is thrown.
    /// </summary>
    /// <remarks>
    /// For example, when using Dictionary.Add, this method would throw an exception when given duplicates.
    /// This test makes sure that the method can handle duplicates without throwing an exception.
    /// </remarks>
    [Fact]
    public void InitializeAttributes_WithDuplicateKeys_ReturnsWithoutException()
    {
        // Arrange
        var jwtClaims = ExampleJwtClaims();
        var apiData = new Dictionary<string, string>
        {
            { "name", "John Doe" },
            { "companyName", "Doe Enterprise" },
            { "department", "Research & Development" },
            { "email", "j.doe@example.test" }
        };

        // Act
        var service = new AttributeMapperService();
        var initializedAttributes = service.InitializeAttributes(jwtClaims, apiData);

        // Assert
        initializedAttributes.ShouldNotBeEmpty();
        // if it's able to reach this, no exception was thrown
    }

    /// <summary>
    /// Given attributes and mapping configuration,
    /// when the attributes are filtered and mapped with the configuration dictionary,
    /// then return only the attributes described in the configuration.
    /// </summary>
    [Fact]
    public void FilterAndMapByConfig_WithValidMapping_ReturnsFilteredAttributes()
    {
        // Arrange
        var attributes = ExampleJwtClaims();
        var identityProviderAttributeMapping = new Dictionary<string, string>
        {
            { "http://example.example.test/example/id", "id" },
            { "given_name", "givenName" },
            { "family_name", "surname" },
            { "email", "email" }
        };
        const int expectedAttributesCountAfterFiltering = 3;

        // Act
        var service = new AttributeMapperService();
        var filteredAttributes = service.FilterAndMapByConfig(attributes, identityProviderAttributeMapping);

        // Assert
        filteredAttributes.ShouldNotBeEmpty();
        filteredAttributes.Count.ShouldBe(expectedAttributesCountAfterFiltering); // asserts that the filtering worked
        filteredAttributes["email"].ShouldBe("j.doe@example.test"); // asserts mapping works with the same key
        filteredAttributes["givenName"].ShouldBe("John"); // asserts mapping with a different key
    }

    private Dictionary<string, string> ExampleJwtClaims()
    {
        return new Dictionary<string, string>
        {
            { "name", "John Doe" },
            { "http://example.test/example", "AB-CD-EF-GH-IJ-KL-MN-OP-QR-ST-UV-WX-YZ" },
            { "given_name", "John" },
            { "family_name", "Doe" },
            { "email", "j.doe@example.test" }
        };
    }
}