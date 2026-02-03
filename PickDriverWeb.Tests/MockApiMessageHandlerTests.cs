using PickDriverWeb.Models.Auth;
using PickDriverWeb.Models.Leagues;
using PickDriverWeb.Services;
using PickDriverWeb.State;
using PickDriverWeb.Tests.Infrastructure;
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
    public async Task UnknownGet_ReturnsEmptyCollection()
    {
        var apiClient = CreateApiClient();

        var result = await apiClient.GetAsync<List<string>>("unknown", auth: false);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data!);
    }

    private static ApiClient CreateApiClient()
    {
        var client = new HttpClient(new MockApiMessageHandler())
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
}
