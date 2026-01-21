using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace PickDriverWeb.State;

public sealed class PickDriverAuthStateProvider : AuthenticationStateProvider
{
    private static readonly ClaimsPrincipal Anonymous = new(new ClaimsIdentity());
    private readonly AuthSessionStore _sessionStore;

    public PickDriverAuthStateProvider(AuthSessionStore sessionStore)
    {
        _sessionStore = sessionStore;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var session = await _sessionStore.GetAsync();
        if (session is null || string.IsNullOrWhiteSpace(session.Token))
        {
            return new AuthenticationState(Anonymous);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, session.User.Id.ToString(CultureInfo.InvariantCulture)),
            new(ClaimTypes.Name, session.User.Username),
            new(ClaimTypes.Email, session.User.Email)
        };

        if (session.User.EmailVerified)
        {
            claims.Add(new Claim("email_verified", "true"));
        }

        var identity = new ClaimsIdentity(claims, "PickDriver");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task SetSessionAsync(AuthSession? session)
    {
        await _sessionStore.SetAsync(session);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public Task<AuthSession?> GetSessionAsync() => _sessionStore.GetAsync();
}

