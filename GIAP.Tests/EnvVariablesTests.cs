using GIAP.Server.Services;
using Shouldly;
using NSubstitute;

namespace GIAP.Tests;

public class EnvVariablesTests
{
    [Fact]
    public void Verify_AllRequiredVariablesExist_DoesNotThrow()
    {
        // Arrange
        var environment = Substitute.For<IEnvironment>();
        var envVariables = new EnvVariables(environment);
        environment.Get("IRMA_SERVER_BASE_URL").Returns("https://is.staging.example.com");
        environment.Get("IRMA_SERVER_API_TOKEN").Returns("example-server-api-token");
        environment.Get("SCHEME_BASE_URL").Returns("https://schemes.staging.yivi.app");
        environment.Get("SCHEME_NAME").Returns("pbdf-staging");

        // Act & Assert
        Should.NotThrow(() => envVariables.Verify());
    }

    [Fact]
    public void Verify_MissingAllVariables_ThrowsException()
    {
        // Arrange
        var environment = Substitute.For<IEnvironment>();
        var envVariables = new EnvVariables(environment);

        // Act & Assert
        Should.Throw<InvalidDataException>(() => envVariables.Verify()).Message.ShouldBe(
            "Not all required environment variables are set. Check the documentation README.md for the required environment variables. Make sure the .env is located at GIAP.server/.env. The following environment variables are missing: SCHEME_BASE_URL, SCHEME_NAME, IRMA_SERVER_BASE_URL, IRMA_SERVER_API_TOKEN.");
    }

    [Fact]
    public void Verify_MissingSomeEnvironmentVariables_ThrowsException()
    {
        // Arrange
        var environment = Substitute.For<IEnvironment>();

        // Missing IRMA_SERVER_API_TOKEN, SCHEME_BASE_URL, and SCHEME_NAME on purpose for testing
        environment.Get("IRMA_SERVER_BASE_URL").Returns("https://is.staging.example.com");

        var envVariables = new EnvVariables(environment);

        // Act & Assert
        Should.Throw<InvalidDataException>(() => envVariables.Verify()).Message.ShouldBe(
            "Not all required environment variables are set. Check the documentation README.md for the required environment variables. Make sure the .env is located at GIAP.server/.env. The following environment variables are missing: SCHEME_BASE_URL, SCHEME_NAME, IRMA_SERVER_API_TOKEN.");
    }

    [Fact]
    public void GetSchemeBaseUrl_WithCorrectValue_ReturnsCorrectValue()
    {
        // Arrange
        const string schemeBaseUrl = "https://schemes.staging.yivi.app";
        var environment = Substitute.For<IEnvironment>();
        environment.Get("SCHEME_BASE_URL").Returns(schemeBaseUrl);
        var envVariables = new EnvVariables(environment);

        // Act
        var result = envVariables.GetSchemeBaseUrl();

        // Assert
        result.ShouldBe(schemeBaseUrl);
    }

    [Fact]
    public void GetSchemeBaseUrl_WithEmptyValue_ThrowsException()
    {
        // Arrange
        var environment = Substitute.For<IEnvironment>();
        environment.Get("SCHEME_BASE_URL").Returns("");
        var envVariables = new EnvVariables(environment);

        // Act
        Should.Throw<InvalidDataException>(() => envVariables.GetSchemeBaseUrl())
            .Message.ShouldBe("Environment variable 'SCHEME_BASE_URL' is not set or is empty.");
    }
}