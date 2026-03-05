using PickDriverWeb.Models.Auth;
using PickDriverWeb.Models.Leagues;
using PickDriverWeb.Services;
using PickDriverWeb.State;
using PickDriverWeb.Tests.Infrastructure;
using System.Reflection;
using Xunit;

namespace PickDriverWeb.Tests;

public sealed class MockApiMessageHandlerTests
{
    [Fact]
    public async Task GetLeagues_ReturnsDemoLeague()
    {
        var apiClient = CreateApiClient();

        var result = await apiClient.GetAsync<List<LeaguePublic>>("leagues/my", auth: true);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data!);
        Assert.Equal("Demo League", result.Data![0].Name);
    }

    [Fact]
    public async Task PostPick_ReturnsDraftResponse()
    {
        var apiClient = CreateApiClient();

        var result = await apiClient.PostAsync<object, DraftResponse>("leagues/10/draft/1/pick", new { driverID = 1 }, auth: true);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("active", result.Data!.Status);
        Assert.Equal(1, result.Data.CurrentPickIndex);
    }

    [Fact]
    public async Task PostPick_AllowsChangingPreviousOwnPick_WhileNextPickIsPending()
    {
        var apiClient = CreateApiClient();

        var firstPick = await apiClient.PostAsync<object, DraftResponse>("leagues/10/draft/1/pick", new { driverID = 1 }, auth: true);
        Assert.True(firstPick.Success);
        Assert.NotNull(firstPick.Data);
        Assert.Equal(1, firstPick.Data!.CurrentPickIndex);

        var changedPick = await apiClient.PostAsync<object, DraftResponse>("leagues/10/draft/1/pick", new { driverID = 2 }, auth: true);
        Assert.True(changedPick.Success);
        Assert.NotNull(changedPick.Data);
        Assert.Equal(1, changedPick.Data!.CurrentPickIndex);

        var state = await apiClient.GetAsync<RaceDraft>("leagues/10/draft/1", auth: true);
        Assert.True(state.Success);
        Assert.NotNull(state.Data);
        Assert.Equal(2, state.Data!.PickedDriverIds[0]);
    }

    [Fact]
    public async Task PostPick_ReturnsDriverUnavailable_WhenDriverIsBanned()
    {
        var apiClient = CreateApiClient();

        var firstPick = await apiClient.PostAsync<object, DraftResponse>("leagues/10/draft/1/pick", new { driverID = 1 }, auth: true);
        Assert.True(firstPick.Success);

        var banResult = await apiClient.PostAsync<object, DraftResponse>(
            "leagues/10/draft/1/ban",
            new { targetUserID = 1, driverID = 3 },
            auth: true
        );
        Assert.True(banResult.Success);

        var secondPick = await apiClient.PostAsync<object, DraftResponse>("leagues/10/draft/1/pick", new { driverID = 3 }, auth: true);

        Assert.False(secondPick.Success);
        Assert.Equal("Driver no longer available", secondPick.ErrorMessage);
        Assert.Equal(409, secondPick.StatusCode);
    }

    [Fact]
    public async Task PostPick_ReturnsTurnInactive_WhenUserCannotEditAnyActiveSlot()
    {
        var handler = new MockApiMessageHandler();
        SetDraftState(handler, new RaceDraft
        {
            Id = 100,
            League = new LeagueRef { Id = 10 },
            RaceId = 1,
            PickOrder = new List<int> { 2, 3, 4 },
            PickedDriverIds = new List<int?> { 1, 2, null },
            BannedDriverIds = new List<int>(),
            BannedDriverIdsByPickIndex = new List<int?> { null, null, null },
            CurrentPickIndex = 2,
            MirrorPicks = false,
            Status = "active"
        });

        var apiClient = CreateApiClient(handler);
        var result = await apiClient.PostAsync<object, DraftResponse>("leagues/10/draft/1/pick", new { driverID = 4 }, auth: true);

        Assert.False(result.Success);
        Assert.Equal("Your turn is no longer active", result.ErrorMessage);
        Assert.Equal(403, result.StatusCode);
    }

    [Fact]
    public async Task UnknownGet_ReturnsEmptyCollection()
    {
        var apiClient = CreateApiClient();

        var result = await apiClient.GetAsync<List<string>>("unknown", auth: false);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data!);
    }

    private static ApiClient CreateApiClient(HttpMessageHandler? handler = null)
    {
        var client = new HttpClient(handler ?? new MockApiMessageHandler())
        {
            BaseAddress = new Uri("https://api.example.com/api/")
        };

        var sessionStore = new FakeAuthSessionStore(new AuthSession
        {
            Token = "demo-token",
            User = new UserPublic { Id = 1, Username = "demo", Email = "demo@example.com" }
        });

        return new ApiClient(client, sessionStore);
    }

    private static void SetDraftState(MockApiMessageHandler handler, RaceDraft draft)
    {
        var field = typeof(MockApiMessageHandler).GetField("_draftState", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(field);
        field!.SetValue(handler, draft);
    }
}
