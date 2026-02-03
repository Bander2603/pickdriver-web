using System.Security.Claims;
using PickDriverWeb.Models.Auth;
using PickDriverWeb.State;
using PickDriverWeb.Tests.Infrastructure;
using Xunit;

namespace PickDriverWeb.Tests;

public sealed class PickDriverAuthStateProviderTests
{
    [Fact]
    public async Task GetAuthenticationStateAsync_ReturnsAnonymous_WhenSessionMissing()
    {
        var store = new FakeAuthSessionStore();
        var provider = new PickDriverAuthStateProvider(store);

        var state = await provider.GetAuthenticationStateAsync();

        Assert.False(state.User.Identity?.IsAuthenticated ?? false);
    }

    [Fact]
    public async Task GetAuthenticationStateAsync_ReturnsClaims_WhenSessionPresent()
    {
        var session = new AuthSession
        {
            Token = "demo-token",
            User = new UserPublic
            {
                Id = 42,
                Username = "demo",
                Email = "demo@example.com",
                EmailVerified = true
            }
        };

        var store = new FakeAuthSessionStore(session);
        var provider = new PickDriverAuthStateProvider(store);

        var state = await provider.GetAuthenticationStateAsync();

        Assert.True(state.User.Identity?.IsAuthenticated ?? false);
        Assert.Equal("demo", state.User.Identity?.Name);
        Assert.Equal("42", state.User.FindFirstValue(ClaimTypes.NameIdentifier));
        Assert.Equal("demo@example.com", state.User.FindFirstValue(ClaimTypes.Email));
        Assert.Equal("true", state.User.FindFirstValue("email_verified"));
    }
}
