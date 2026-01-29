namespace PickDriverWeb.Models.Auth;

public sealed class GoogleAuthRequest
{
    public string IdToken { get; set; } = string.Empty;
    public string? InviteCode { get; set; }
}
