namespace GIAP.Server.Models;

/// <summary>
/// The response of the IRMA server when issuing a credential.
/// </summary>
public class IssuanceResponse
{
    public required SessionPointer SessionPtr { get; init; }
    public required string Token { get; init; }
    public required FrontendRequest FrontendRequest { get; init; }
}

/// <summary>
/// Part of IssuanceResponse.
/// </summary>
public class SessionPointer
{
    public required string U { get; init; }
    public required string Irmaqr { get; init; }
}

/// <summary>
/// Part of IssuanceResponse.
/// </summary>
public class FrontendRequest
{
    public required string Authorization { get; init; }
    public required bool PairingHint { get; init; }
    public required string MinProtocolVersion { get; init; }
    public required string MaxProtocolVersion { get; init; }
}