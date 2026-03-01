using System.Net;
using System.Net.Http.Json;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using PickDriverWeb.Components.Pages;
using PickDriverWeb.Models;
using PickDriverWeb.Models.Auth;
using PickDriverWeb.Models.Drivers;
using PickDriverWeb.Models.Leagues;
using PickDriverWeb.Models.Teams;
using PickDriverWeb.Services;
using PickDriverWeb.State;
using PickDriverWeb.Tests.Infrastructure;
using Xunit;

namespace PickDriverWeb.Tests;

public sealed class LeaguesComponentTests
{
    [Fact]
    public void WhenLeaguesEmpty_ShowsEmptyState()
    {
        using var ctx = new TestContext();

        var session = new AuthSession
        {
            Token = "demo-token",
            User = new UserPublic { Id = 1, Username = "demo", Email = "demo@example.com" }
        };

        var handler = new StubHttpMessageHandler(request =>
        {
            var path = request.RequestUri?.AbsolutePath.Trim('/') ?? string.Empty;
            return path switch
            {
                "leagues/my" => Ok(new List<LeaguePublic>()),
                "drivers" => Ok(new List<Driver>()),
                "f1/teams" => Ok(new List<F1Team>()),
                _ => new HttpResponseMessage(HttpStatusCode.NotFound)
            };
        });

        ctx.Services.AddPickDriverTestServices(handler, new FakeAuthSessionStore(session));

        var cut = ctx.RenderComponent<Leagues>();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("Todavia no estas en ninguna liga", cut.Markup);
        });
    }

    [Fact]
    public void WhenLeaguesLoaded_ShowsLeagueRow()
    {
        using var ctx = new TestContext();

        var session = new AuthSession
        {
            Token = "demo-token",
            User = new UserPublic { Id = 1, Username = "demo", Email = "demo@example.com" }
        };

        ctx.Services.AddPickDriverTestServices(new MockApiMessageHandler(), new FakeAuthSessionStore(session));

        var cut = ctx.RenderComponent<Leagues>();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("Demo League", cut.Markup);
            Assert.Contains("Owner", cut.Markup);
        });
    }

    [Fact]
    public async Task WhenAuthEndpointReturns401_LogsOutAndRedirectsToLogin()
    {
        using var ctx = new TestContext();

        var sessionStore = new FakeAuthSessionStore(new AuthSession
        {
            Token = "expired-token",
            User = new UserPublic { Id = 1, Username = "demo", Email = "demo@example.com" }
        });

        var handler = new StubHttpMessageHandler(request =>
        {
            var path = request.RequestUri?.AbsolutePath.Trim('/') ?? string.Empty;
            return path switch
            {
                "leagues/my" => new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = JsonContent.Create(new ApiError { Error = true, Reason = "Token expirado" }, options: ApiJson.Options)
                },
                "drivers" => Ok(new List<Driver>()),
                "f1/teams" => Ok(new List<F1Team>()),
                _ => new HttpResponseMessage(HttpStatusCode.NotFound)
            };
        });

        ctx.Services.AddPickDriverTestServices(handler, sessionStore);

        var nav = ctx.Services.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>();
        nav.NavigateTo("/leagues");

        var cut = ctx.RenderComponent<Leagues>();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("/login?returnUrl=%2Fleagues", nav.Uri);
        });

        var session = await sessionStore.GetAsync();
        Assert.Null(session);
    }

    private static HttpResponseMessage Ok<T>(T payload)
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(payload, options: ApiJson.Options)
        };
    }
}
