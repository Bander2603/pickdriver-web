namespace PickDriverWeb.Models.Auth;

public sealed class VerifyEmailRequest
{
    public string Token { get; set; } = string.Empty;
}

