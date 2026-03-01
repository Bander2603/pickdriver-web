using System.Net;
using System.Net.Http.Json;
using Bunit;
using PickDriverWeb.Components.Pages;
using PickDriverWeb.Models.Auth;
using PickDriverWeb.Models.Races;
using PickDriverWeb.Services;
using PickDriverWeb.State;
using PickDriverWeb.Tests.Infrastructure;
using Xunit;

namespace PickDriverWeb.Tests;

public sealed class DashboardComponentTests
{
    [Fact]
    public void UsesBrowserTimeZone_ForNextRaceDate()
    {
        using var ctx = new TestContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        ctx.JSInterop.Setup<string?>("pickdriverRaces.getBrowserTimeZone").SetResult("Asia/Tokyo");

        var session = new AuthSession
        {
            Token = "demo-token",
            User = new UserPublic { Id = 1, Username = "demo", Email = "demo@example.com" }
        };

        var race = new Race
        {
            Id = 1,
            Round = 1,
            Name = "Australian Grand Prix",
            CircuitName = "Albert Park Grand Prix Circuit",
            Country = "Australia",
            CountryCode = "AU",
            RaceTime = new DateTimeOffset(2026, 3, 15, 5, 0, 0, TimeSpan.Zero)
        };

        var handler = new StubHttpMessageHandler(request =>
        {
            var path = request.RequestUri?.AbsolutePath.Trim('/') ?? string.Empty;
            return path switch
            {
                "races/upcoming" => Ok(race),
                "leagues/my" => Ok(new List<object>()),
                "standings/f1/drivers" => Ok(new List<object>()),
                "standings/f1/teams" => Ok(new List<object>()),
                _ => new HttpResponseMessage(HttpStatusCode.NotFound)
            };
        });

        ctx.Services.AddPickDriverTestServices(handler, new FakeAuthSessionStore(session));

        var cut = ctx.RenderComponent<Dashboard>();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("15 Mar 2026 14:00", cut.Markup);
        });
    }

    private static HttpResponseMessage Ok<T>(T payload)
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(payload, options: ApiJson.Options)
        };
    }
}
