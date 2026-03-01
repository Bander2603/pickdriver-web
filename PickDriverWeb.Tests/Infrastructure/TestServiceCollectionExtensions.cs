using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PickDriverWeb.Options;
using PickDriverWeb.Services;
using PickDriverWeb.State;

namespace PickDriverWeb.Tests.Infrastructure;

internal static class TestServiceCollectionExtensions
{
    public static void AddPickDriverTestServices(
        this IServiceCollection services,
        HttpMessageHandler handler,
        IAuthSessionStore? sessionStore = null,
        GoogleAuthOptions? googleOptions = null)
    {
        var store = sessionStore ?? new FakeAuthSessionStore();
        var options = googleOptions ?? new GoogleAuthOptions();

        services.AddSingleton<IAuthSessionStore>(store);
        services.AddSingleton<PickDriverAuthStateProvider>();
        services.AddSingleton<AuthenticationStateProvider>(sp => sp.GetRequiredService<PickDriverAuthStateProvider>());
        services.AddSingleton<IOptions<GoogleAuthOptions>>(Microsoft.Extensions.Options.Options.Create(options));
        services.AddSingleton<ApiClient>(sp => new ApiClient(
            new HttpClient(handler) { BaseAddress = new Uri("https://example.test/") },
            sp.GetRequiredService<IAuthSessionStore>(),
            sp.GetRequiredService<PickDriverAuthStateProvider>(),
            sp.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>()));
        services.AddSingleton<AuthService>();
        services.AddSingleton<EmailCooldownService>();
    }
}
