using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using PickDriverWeb.Localization;
using PickDriverWeb.Models;
using PickDriverWeb.Models.Auth;
using PickDriverWeb.Services;
using PickDriverWeb.State;
using PickDriverWeb.Tests.Infrastructure;
using Xunit;

namespace PickDriverWeb.Tests;

public sealed class ApiClientTests
{
    [Fact]
    public async Task GetAsync_ReturnsData_OnSuccess()
    {
        var handler = new StubHttpMessageHandler(_ =>
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new UserPublic { Id = 1, Username = "demo" }, options: ApiJson.Options)
            };
        });

        var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.test/") };
        var apiClient = new ApiClient(client, new FakeAuthSessionStore());

        var result = await apiClient.GetAsync<UserPublic>("auth/profile");

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data!.Id);
    }

    [Fact]
    public async Task GetAsync_ReturnsNoContent_On204()
    {
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.NoContent));
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.test/") };
        var apiClient = new ApiClient(client, new FakeAuthSessionStore());

        var result = await apiClient.GetAsync<UserPublic>("auth/profile");

        Assert.True(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetAsync_ReturnsApiErrorMessage_OnFailure()
    {
        var handler = new StubHttpMessageHandler(_ =>
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = JsonContent.Create(new ApiError { Error = true, Reason = "Bad input" }, options: ApiJson.Options)
            };
        });

        var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.test/") };
        var apiClient = new ApiClient(client, new FakeAuthSessionStore());

        var result = await apiClient.GetAsync<UserPublic>("auth/profile");

        Assert.False(result.Success);
        Assert.Equal("Bad input", result.ErrorMessage);
    }

    [Fact]
    public async Task GetAsync_ReportsUnexpectedResponse_WhenJsonInvalid()
    {
        var handler = new StubHttpMessageHandler(_ =>
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("not-json", Encoding.UTF8, "application/json")
            };
        });

        var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.test/") };
        var apiClient = new ApiClient(client, new FakeAuthSessionStore());

        var result = await apiClient.GetAsync<UserPublic>("auth/profile");

        Assert.False(result.Success);
        Assert.Equal(AppStrings.Translate("Respuesta inesperada del servidor."), result.ErrorMessage);
    }

    [Fact]
    public async Task GetAsync_AddsAuthorizationHeader_WhenAuthRequested()
    {
        AuthenticationHeaderValue? captured = null;
        var handler = new StubHttpMessageHandler(request =>
        {
            captured = request.Headers.Authorization;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new UserPublic { Id = 1, Username = "demo" }, options: ApiJson.Options)
            };
        });

        var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.test/") };
        var session = new AuthSession
        {
            Token = "demo-token",
            User = new UserPublic { Id = 1, Username = "demo", Email = "demo@example.com" }
        };
        var apiClient = new ApiClient(client, new FakeAuthSessionStore(session));

        var result = await apiClient.GetAsync<UserPublic>("auth/profile", auth: true);

        Assert.True(result.Success);
        Assert.NotNull(captured);
        Assert.Equal("Bearer", captured!.Scheme);
        Assert.Equal("demo-token", captured.Parameter);
    }

    [Fact]
    public async Task GetAsync_ReturnsRawError_WhenResponseIsPlainText()
    {
        var handler = new StubHttpMessageHandler(_ =>
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("plain error", Encoding.UTF8, "text/plain")
            };
        });

        var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.test/") };
        var apiClient = new ApiClient(client, new FakeAuthSessionStore());

        var result = await apiClient.GetAsync<UserPublic>("auth/profile");

        Assert.False(result.Success);
        Assert.Equal("plain error", result.ErrorMessage);
    }

    [Fact]
    public async Task GetAsync_ClearsSession_On401_WhenAuthRequested()
    {
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.Unauthorized));
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.test/") };
        var sessionStore = new FakeAuthSessionStore(new AuthSession
        {
            Token = "expired-token",
            User = new UserPublic { Id = 1, Username = "demo", Email = "demo@example.com" }
        });
        var apiClient = new ApiClient(client, sessionStore);

        var result = await apiClient.GetAsync<UserPublic>("auth/profile", auth: true);

        Assert.False(result.Success);
        var session = await sessionStore.GetAsync();
        Assert.Null(session);
    }

    [Fact]
    public async Task GetAsync_DoesNotClearSession_On403_WhenAuthRequested()
    {
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.Forbidden));
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.test/") };
        var sessionStore = new FakeAuthSessionStore(new AuthSession
        {
            Token = "valid-token",
            User = new UserPublic { Id = 1, Username = "demo", Email = "demo@example.com" }
        });
        var apiClient = new ApiClient(client, sessionStore);

        var result = await apiClient.GetAsync<UserPublic>("auth/profile", auth: true);

        Assert.False(result.Success);
        var session = await sessionStore.GetAsync();
        Assert.NotNull(session);
        Assert.Equal("valid-token", session!.Token);
    }
}
