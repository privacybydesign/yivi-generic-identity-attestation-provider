using System.Text.Json.Serialization;

namespace GIAP.Server.Models;

/// <summary>
/// The response of the IRMA server when issuing a credential.
/// </summary>
public class IssuanceResponse
{
    [JsonPropertyName("sessionPtr")] public required SessionPointer SessionPtr { get; init; }
    [JsonPropertyName("token")] public required string Token { get; init; }
    [JsonPropertyName("frontendRequest")] public required FrontendRequest FrontendRequest { get; init; }
}

/// <summary>
/// Part of IssuanceResponse.
/// </summary>
public class SessionPointer
{
    [JsonPropertyName("u")] public required string U { get; init; }
    [JsonPropertyName("irmaqr")] public required string IrmaQr { get; init; }
}

/// <summary>
/// Part of IssuanceResponse.
/// </summary>
public class FrontendRequest
{
    [JsonPropertyName("authorization")] public required string Authorization { get; init; }
    [JsonPropertyName("pairingHint")] public required bool PairingHint { get; init; }

    [JsonPropertyName("minProtocolVersion")]
    public required string MinProtocolVersion { get; init; }

    [JsonPropertyName("maxProtocolVersion")]
    public required string MaxProtocolVersion { get; init; }
}