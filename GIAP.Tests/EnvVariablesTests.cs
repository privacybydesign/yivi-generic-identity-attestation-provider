using GIAP.Server.Services;
using Shouldly;

namespace GIAP.Tests;

public class EnvVariablesTests
{
    [Fact]
    public void Constructor_MissingAllEnvironmentVariables_ThrowsException()
    {
        // assume no environment variables are set when this test is run 

        Should.Throw<InvalidDataException>(() => new EnvVariables()).Message.ShouldBe(
            "Not all required environment variables are set. Check the documentation README.md for the required environment variables. Make sure the .env is located at GIAP.server/.env. The following environment variables are missing: SCHEME_BASE_URL, SCHEME_NAME, IRMA_SERVER_BASE_URL, IRMA_SERVER_API_TOKEN.");
    }
}