namespace PickDriverWeb.Models.Auth;

public sealed class ResendVerificationResponse
{
    public string? Message { get; set; }
    public string? VerificationToken { get; set; }
}

