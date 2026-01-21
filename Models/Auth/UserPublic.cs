namespace PickDriverWeb.Models.Auth;

public sealed class UserPublic
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool EmailVerified { get; set; }
}

