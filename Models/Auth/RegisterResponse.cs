namespace PickDriverWeb.Models.Auth;

public sealed class RegisterResponse
{
    public UserPublic User { get; set; } = new();
    public bool VerificationEmailSent { get; set; }
}
