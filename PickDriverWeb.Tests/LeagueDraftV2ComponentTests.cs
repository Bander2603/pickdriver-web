using System.Net;
using System.Net.Http.Json;
using Bunit;
using PickDriverWeb.Components.Pages;
using PickDriverWeb.Localization;
using PickDriverWeb.Models;
using PickDriverWeb.Models.Auth;
using PickDriverWeb.Models.Drivers;
using PickDriverWeb.Models.Leagues;
using PickDriverWeb.Models.Races;
using PickDriverWeb.Services;
using PickDriverWeb.State;
using PickDriverWeb.Tests.Infrastructure;
using Xunit;

namespace PickDriverWeb.Tests;

public sealed class LeagueDraftV2ComponentTests
{
    [Fact]
    public void LegacyDraft_KeepsTurnPickAndAutopickControls()
    {
        using var ctx = new BunitContext();
        ctx.Services.AddPickDriverTestServices(new MockApiMessageHandler(), Session());

        var cut = ctx.Render<LeagueDraft>(parameters => parameters.Add(p => p.LeagueId, 10));

        cut.WaitForAssertion(() =>
        {
            Assert.NotEmpty(cut.FindAll(".btn-pick"));
            Assert.Contains("Turno actual", cut.Markup);
            Assert.Contains("Autopick", cut.Markup);
        });
    }

    [Fact]
    public void CollectingV2_ShowsOrderWithoutTurnsOrPrivateResults()
    {
        using var ctx = new BunitContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        var fixture = new V2Fixture(bansEnabled: false, state: "collecting");
        ctx.Services.AddPickDriverTestServices(fixture.Handler, Session());

        var cut = ctx.Render<LeagueDraft>(parameters => parameters.Add(p => p.LeagueId, 10));

        cut.WaitForAssertion(() =>
        {
            Assert.Empty(cut.FindAll(".btn-pick"));
            Assert.Empty(cut.FindAll(".btn-ban"));
            Assert.Equal(3, cut.FindAll(".draft-order__item").Count);
            Assert.Empty(cut.FindAll(".draft-order__badge"));
            Assert.DoesNotContain("Turno actual", cut.Markup);
            Assert.Contains(AppStrings.Translate("Lista de picks vacia!"), cut.Markup);
            Assert.Single(cut.FindAll(".league-hub-deadlines > div"));
        });
    }

    [Fact]
    public void CollectingV2_ImmediatelyGreysPicksAtSubmissionDeadline()
    {
        using var ctx = new BunitContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        var fixture = new V2Fixture(bansEnabled: false, state: "collecting", submissionPassed: true);
        ctx.Services.AddPickDriverTestServices(fixture.Handler, Session());

        var cut = ctx.Render<LeagueDraft>(parameters => parameters.Add(p => p.LeagueId, 10));

        cut.WaitForAssertion(() =>
        {
            Assert.True(cut.Find(".btn-autopick").HasAttribute("disabled"));
            Assert.Empty(cut.FindAll(".draft-order__badge"));
        });
    }

    [Fact]
    public void ResolvedV2_RendersMissedMirrorSlotAndReloadsFullDraftAfterBan()
    {
        using var ctx = new BunitContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        var fixture = new V2Fixture(bansEnabled: true, state: "resolved");
        ctx.Services.AddPickDriverTestServices(fixture.Handler, Session());

        var cut = ctx.Render<LeagueDraft>(parameters => parameters.Add(p => p.LeagueId, 10));
        cut.WaitForAssertion(() =>
        {
            Assert.Equal(3, cut.FindAll(".draft-order__item").Count);
            Assert.Single(cut.FindAll(".draft-order__badge--missed"));
            Assert.False(cut.Find(".btn-ban").HasAttribute("disabled"));
            Assert.True(cut.Find(".btn-autopick").HasAttribute("disabled"));
            Assert.Contains("Driver One", cut.Markup);
            Assert.Contains("Driver Two", cut.Markup);
        });

        cut.Find(".btn-ban").Click();
        cut.WaitForElement("#v2-ban-target").Change("1");
        cut.Find(".league-hub-modal__actions .btn-outline-danger").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal(1, fixture.V2BanPosts);
            Assert.True(fixture.DraftGets >= 2);
            Assert.Contains("Driver Three", cut.Markup);
            Assert.Contains("draft-order__badge--banned", cut.Markup);
        });
    }

    [Fact]
    public void V2BanError_DisplaysBackendReasonWithoutLocalMutation()
    {
        using var ctx = new BunitContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        const string reason = "Ban budget exhausted for this team.";
        var fixture = new V2Fixture(bansEnabled: true, state: "resolved", banError: reason);
        ctx.Services.AddPickDriverTestServices(fixture.Handler, Session());

        var cut = ctx.Render<LeagueDraft>(parameters => parameters.Add(p => p.LeagueId, 10));
        cut.WaitForElement(".btn-ban").Click();
        cut.WaitForElement("#v2-ban-target").Change("1");
        cut.Find(".league-hub-modal__actions .btn-outline-danger").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains(reason, cut.Markup);
            Assert.Equal(1, fixture.V2BanPosts);
            Assert.Equal(1, fixture.CurrentRevision);
        });
    }

    private static FakeAuthSessionStore Session() => new(new AuthSession
    {
        Token = "test-token",
        User = new UserPublic { Id = 1, Username = "me", Email = "me@test.com" }
    });

    private sealed class V2Fixture
    {
        private readonly LeaguePublic league;
        private readonly Race race;
        private readonly List<Driver> drivers;
        private readonly string? banError;
        private readonly DateTimeOffset submissionDeadline;
        private RaceDraft draft;

        internal V2Fixture(bool bansEnabled, string state, string? banError = null, bool submissionPassed = false)
        {
            this.banError = banError;
            league = new LeaguePublic
            {
                Id = 10,
                Name = "V2 League",
                Status = "active",
                InitialRaceRound = 14,
                OwnerId = 1,
                MaxPlayers = 2,
                BansEnabled = bansEnabled,
                MirrorPicksEnabled = true
            };
            race = new Race
            {
                Id = 39,
                SeasonId = 2,
                Round = 14,
                Name = "Dutch GP",
                Country = "Netherlands",
                CircuitName = "Zandvoort",
                Status = "scheduled",
                Fp1Time = DateTimeOffset.UtcNow.AddHours(12),
                RaceTime = DateTimeOffset.UtcNow.AddDays(2)
            };
            drivers = new()
            {
                new Driver { Id = 11, FirstName = "Driver", LastName = "One", DriverCode = "ONE", Active = true },
                new Driver { Id = 12, FirstName = "Driver", LastName = "Two", DriverCode = "TWO", Active = true },
                new Driver { Id = 13, FirstName = "Driver", LastName = "Three", DriverCode = "THR", Active = true }
            };
            submissionDeadline = submissionPassed
                ? DateTimeOffset.UtcNow.AddMinutes(-1)
                : (bansEnabled ? DateTimeOffset.UtcNow.AddHours(-12) : race.Fp1Time!.Value);
            draft = BuildDraft(state, bansEnabled, new int?[] { 11, state == "resolved" ? 12 : 12, state == "resolved" ? null : 13 }, 1);
            Handler = new StubHttpMessageHandler(Respond);
        }

        internal HttpMessageHandler Handler { get; }
        internal int DraftGets { get; private set; }
        internal int V2BanPosts { get; private set; }
        internal int? CurrentRevision => draft.ResolutionRevision;

        private HttpResponseMessage Respond(HttpRequestMessage request)
        {
            var path = request.RequestUri?.AbsolutePath.Trim('/') ?? string.Empty;
            if (request.Method == HttpMethod.Post && path == "leagues/10/draft/39/v2/ban")
            {
                V2BanPosts++;
                if (banError is not null)
                {
                    return new HttpResponseMessage(HttpStatusCode.Conflict)
                    {
                        Content = JsonContent.Create(new ApiError { Error = true, Reason = banError }, options: ApiJson.Options)
                    };
                }
                draft = BuildDraft("resolved", true, new int?[] { 11, 13, null }, 2);
                return Ok(new V2BanResult
                {
                    DraftId = 100,
                    TargetUserId = 2,
                    BannedDriverId = 12,
                    TargetPickIndex = 1,
                    ResolutionRevision = 2
                });
            }

            return path switch
            {
                "leagues/my" => Ok(new List<LeaguePublic> { league }),
                "leagues/10/members" => Ok(new List<UserPublic>
                {
                    new() { Id = 1, Username = "me" },
                    new() { Id = 2, Username = "rival" }
                }),
                "players/standings/players" => Ok(Array.Empty<object>()),
                "players/standings/picks" => Ok(Array.Empty<object>()),
                "races/current" => Ok(race),
                "races/upcoming" => Ok(new List<Race> { race }),
                "races" => Ok(new List<Race> { race }),
                "drivers" => Ok(drivers),
                "leagues/10/draft/39" => DraftResponse(),
                "leagues/10/draft/39/deadlines" => Ok(new DraftDeadline
                {
                    RaceId = 39,
                    LeagueId = 10,
                    GameplayVersion = "v2",
                    SubmissionDeadline = submissionDeadline,
                    BanWindowClosesAt = league.BansEnabled ? race.Fp1Time : null,
                    FirstHalfDeadline = submissionDeadline,
                    SecondHalfDeadline = race.Fp1Time
                }),
                "leagues/10/pick-preferences" => Ok(new PickPreferenceSettings()),
                _ => new HttpResponseMessage(HttpStatusCode.NotFound)
            };
        }

        private HttpResponseMessage DraftResponse()
        {
            DraftGets++;
            return Ok(draft);
        }

        private RaceDraft BuildDraft(string state, bool bansEnabled, IEnumerable<int?> picks, int revision)
            => new()
            {
                Id = 100,
                League = new LeagueRef { Id = 10 },
                RaceId = 39,
                PickOrder = new() { 1, 2, 1 },
                PickedDriverIds = picks.ToList(),
                BannedDriverIds = revision > 1 ? new() { 12 } : new(),
                BannedDriverIdsByPickIndex = revision > 1 ? new() { null, 12, null } : new() { null, null, null },
                BannedByUserIdsByPickIndex = revision > 1 ? new() { null, 1, null } : new() { null, null, null },
                MirrorPicks = true,
                Status = "active",
                GameplayVersion = "v2",
                ResolutionState = state,
                ResolutionRevision = revision,
                SubmissionDeadline = submissionDeadline,
                BanWindowClosesAt = bansEnabled ? race.Fp1Time : null,
                BanLimitPerActor = 2,
                BansUsedByUserID = new() { ["1"] = 0, ["2"] = 0 }
            };

        private static HttpResponseMessage Ok<T>(T payload) => new(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(payload, options: ApiJson.Options)
        };
    }
}
