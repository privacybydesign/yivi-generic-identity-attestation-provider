using System.Text.Json.Serialization;

namespace GIAP.Server.Models;

/// <summary>
/// The issuance request to the IRMA server when issuing a credential.
/// </summary>
public class IssuanceRequest
{
    [JsonPropertyName("@context")] public required string Context { get; init; }
    [JsonPropertyName("credentials")] public required Credential[] Credentials { get; init; }
}

/// <summary>
/// Part of the IssuanceRequest.
/// </summary>
public class Credential
{
    [JsonPropertyName("credential")] public required string CredentialId { get; init; }
    [JsonPropertyName("validity")] public required long Validity { get; init; }
    [JsonPropertyName("attributes")] public required Dictionary<string, string> Attributes { get; init; }
}