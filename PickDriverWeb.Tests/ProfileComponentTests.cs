using System.Net;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using PickDriverWeb.Components.Pages;
using PickDriverWeb.Localization;
using PickDriverWeb.Tests.Infrastructure;
using Xunit;

namespace PickDriverWeb.Tests;

public sealed class ProfileComponentTests
{
    [Fact]
    public void WhenUnauthenticated_RedirectsToLogin()
    {
        using var ctx = new TestContext();
        ctx.Services.AddPickDriverTestServices(new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)));

        var nav = ctx.Services.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>();
        nav.NavigateTo("/profile");

        var cut = ctx.RenderComponent<Profile>();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains(AppStrings.Translate("Redirigiendo al login..."), cut.Markup);
            Assert.Contains("/login?returnUrl=%2Fprofile", nav.Uri);
        });
    }
}
