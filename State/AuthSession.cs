using PickDriverWeb.Models.Auth;

namespace PickDriverWeb.State;

public sealed class AuthSession
{
    public string Token { get; set; } = string.Empty;
    public UserPublic User { get; set; } = new();
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

