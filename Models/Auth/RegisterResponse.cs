namespace PickDriverWeb.Models.Auth;

public sealed class RegisterResponse
{
    public UserPublic User { get; set; } = new();
    public bool VerificationRequired { get; set; }
    public string? VerificationToken { get; set; }
}

