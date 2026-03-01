using System.Net;
using System.Net.Http.Json;
using Bunit;
using PickDriverWeb.Components.Pages;
using PickDriverWeb.Models.Races;
using PickDriverWeb.Services;
using PickDriverWeb.Tests.Infrastructure;
using Xunit;

namespace PickDriverWeb.Tests;

public sealed class RacesComponentTests
{
    [Fact]
    public void UsesBrowserTimeZone_ForLocalSchedule()
    {
        using var ctx = new TestContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        ctx.JSInterop.Setup<string?>("pickdriverRaces.getBrowserTimeZone").SetResult("Asia/Tokyo");
        ctx.JSInterop.Setup<int>("pickdriverRaces.getRowEndIndex", _ => true).SetResult(0);
        ctx.JSInterop.SetupVoid("pickdriverRaces.scrollToElement", _ => true);

        var race = new Race
        {
            Id = 1,
            Round = 1,
            Name = "Australian Grand Prix",
            CircuitName = "Albert Park Grand Prix Circuit",
            Country = "Australia",
            CountryCode = "AU",
            Fp1Time = new DateTimeOffset(2026, 3, 14, 0, 0, 0, TimeSpan.Zero),
            RaceTime = new DateTimeOffset(2026, 3, 15, 5, 0, 0, TimeSpan.Zero)
        };

        var handler = new StubHttpMessageHandler(request =>
        {
            var path = request.RequestUri?.AbsolutePath.Trim('/') ?? string.Empty;
            return path == "races"
                ? new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(new List<Race> { race }, options: ApiJson.Options)
                }
                : new HttpResponseMessage(HttpStatusCode.NotFound);
        });

        ctx.Services.AddPickDriverTestServices(handler);

        var cut = ctx.RenderComponent<Races>();
        cut.Find("button.race-card").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("09:00", cut.Markup);
        });
    }
}
