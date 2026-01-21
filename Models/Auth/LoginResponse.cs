namespace PickDriverWeb.Models.Auth;

public sealed class LoginResponse
{
    public UserPublic User { get; set; } = new();
    public string Token { get; set; } = string.Empty;
}

