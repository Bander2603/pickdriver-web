using System.Net;
using System.Net.Http.Json;
using PickDriverWeb.Models;
using PickDriverWeb.Models.Auth;
using PickDriverWeb.Services;
using PickDriverWeb.State;
using PickDriverWeb.Tests.Infrastructure;
using Xunit;

namespace PickDriverWeb.Tests;

public sealed class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_PersistsSession_OnSuccess()
    {
        var handler = new StubHttpMessageHandler(_ =>
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new LoginResponse
                {
                    User = new UserPublic { Id = 7, Username = "demo", Email = "demo@example.com" },
                    Token = "demo-token"
                }, options: ApiJson.Options)
            };
        });

        var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.test/") };
        var sessionStore = new FakeAuthSessionStore();
        var authProvider = new PickDriverAuthStateProvider(sessionStore);
        var apiClient = new ApiClient(client, sessionStore);
        var authService = new AuthService(apiClient, authProvider);

        var result = await authService.LoginAsync(new LoginRequest { Email = "demo@example.com", Password = "secret" });

        Assert.True(result.Success);
        var session = await sessionStore.GetAsync();
        Assert.NotNull(session);
        Assert.Equal("demo-token", session!.Token);
        Assert.Equal(7, session.User.Id);
    }

    [Fact]
    public async Task LoginAsync_DoesNotPersistSession_OnFailure()
    {
        var handler = new StubHttpMessageHandler(_ =>
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = JsonContent.Create(new ApiError { Error = true, Reason = "invalid" }, options: ApiJson.Options)
            };
        });

        var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.test/") };
        var sessionStore = new FakeAuthSessionStore();
        var authProvider = new PickDriverAuthStateProvider(sessionStore);
        var apiClient = new ApiClient(client, sessionStore);
        var authService = new AuthService(apiClient, authProvider);

        var result = await authService.LoginAsync(new LoginRequest { Email = "demo@example.com", Password = "secret" });

        Assert.False(result.Success);
        var session = await sessionStore.GetAsync();
        Assert.Null(session);
    }

    [Fact]
    public async Task GoogleLoginAsync_PersistsSession_OnSuccess()
    {
        var handler = new StubHttpMessageHandler(_ =>
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new LoginResponse
                {
                    User = new UserPublic { Id = 9, Username = "google", Email = "google@example.com" },
                    Token = "google-token"
                }, options: ApiJson.Options)
            };
        });

        var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.test/") };
        var sessionStore = new FakeAuthSessionStore();
        var authProvider = new PickDriverAuthStateProvider(sessionStore);
        var apiClient = new ApiClient(client, sessionStore);
        var authService = new AuthService(apiClient, authProvider);

        var result = await authService.GoogleLoginAsync(new GoogleAuthRequest { IdToken = "token" });

        Assert.True(result.Success);
        var session = await sessionStore.GetAsync();
        Assert.NotNull(session);
        Assert.Equal("google-token", session!.Token);
        Assert.Equal(9, session.User.Id);
    }

    [Fact]
    public async Task ResendVerificationAsync_CallsExpectedEndpoint()
    {
        string? path = null;
        var handler = new StubHttpMessageHandler(request =>
        {
            path = request.RequestUri?.AbsolutePath;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new AuthMessageResponse
                {
                    Message = "verification sent"
                }, options: ApiJson.Options)
            };
        });

        var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.test/") };
        var sessionStore = new FakeAuthSessionStore();
        var authProvider = new PickDriverAuthStateProvider(sessionStore);
        var apiClient = new ApiClient(client, sessionStore);
        var authService = new AuthService(apiClient, authProvider);

        var result = await authService.ResendVerificationAsync(new ResendVerificationRequest
        {
            Email = "demo@example.com"
        });

        Assert.True(result.Success);
        Assert.Equal("/auth/resend-verification", path);
        Assert.Equal("verification sent", result.Data?.Message);
    }

    [Fact]
    public async Task ForgotPasswordAsync_CallsExpectedEndpoint()
    {
        string? path = null;
        var handler = new StubHttpMessageHandler(request =>
        {
            path = request.RequestUri?.AbsolutePath;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new AuthMessageResponse
                {
                    Message = "reset email sent"
                }, options: ApiJson.Options)
            };
        });

        var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.test/") };
        var sessionStore = new FakeAuthSessionStore();
        var authProvider = new PickDriverAuthStateProvider(sessionStore);
        var apiClient = new ApiClient(client, sessionStore);
        var authService = new AuthService(apiClient, authProvider);

        var result = await authService.ForgotPasswordAsync(new ForgotPasswordRequest
        {
            Email = "demo@example.com"
        });

        Assert.True(result.Success);
        Assert.Equal("/auth/forgot-password", path);
        Assert.Equal("reset email sent", result.Data?.Message);
    }

    [Fact]
    public async Task ResetPasswordAsync_CallsExpectedEndpoint()
    {
        string? path = null;
        var handler = new StubHttpMessageHandler(request =>
        {
            path = request.RequestUri?.AbsolutePath;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new AuthMessageResponse
                {
                    Message = "password updated"
                }, options: ApiJson.Options)
            };
        });

        var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.test/") };
        var sessionStore = new FakeAuthSessionStore();
        var authProvider = new PickDriverAuthStateProvider(sessionStore);
        var apiClient = new ApiClient(client, sessionStore);
        var authService = new AuthService(apiClient, authProvider);

        var result = await authService.ResetPasswordAsync(new ResetPasswordRequest
        {
            Token = "token",
            NewPassword = "new-password"
        });

        Assert.True(result.Success);
        Assert.Equal("/auth/reset-password", path);
        Assert.Equal("password updated", result.Data?.Message);
    }

    [Fact]
    public async Task LogoutAsync_ClearsSession()
    {
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.test/") };
        var sessionStore = new FakeAuthSessionStore(new AuthSession
        {
            Token = "demo-token",
            User = new UserPublic { Id = 1, Username = "demo", Email = "demo@example.com" }
        });
        var authProvider = new PickDriverAuthStateProvider(sessionStore);
        var apiClient = new ApiClient(client, sessionStore);
        var authService = new AuthService(apiClient, authProvider);

        await authService.LogoutAsync();

        var session = await sessionStore.GetAsync();
        Assert.Null(session);
    }
}
