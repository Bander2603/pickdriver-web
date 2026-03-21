using System.Net;
using System.Net.Http.Json;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using PickDriverWeb.Components.Pages.Auth;
using PickDriverWeb.Localization;
using PickDriverWeb.Models;
using PickDriverWeb.Models.Auth;
using PickDriverWeb.Services;
using PickDriverWeb.Tests.Infrastructure;
using Xunit;

namespace PickDriverWeb.Tests;

public sealed class LoginComponentTests
{
    [Fact]
    public void Submit_EmptyForm_ShowsValidationMessages()
    {
        using var ctx = new TestContext();
        ctx.Services.AddPickDriverTestServices(new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)));

        var cut = ctx.RenderComponent<Login>();

        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains(AppStrings.Translate("El email es obligatorio."), cut.Markup);
            Assert.Contains(AppStrings.Translate("La password es obligatoria."), cut.Markup);
        });
    }

    [Fact]
    public void Submit_ValidCredentials_NavigatesToReturnUrl()
    {
        using var ctx = new TestContext();
        ctx.Services.AddPickDriverTestServices(new StubHttpMessageHandler(_ =>
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new LoginResponse
                {
                    User = new UserPublic { Id = 1, Username = "demo", Email = "demo@example.com" },
                    Token = "demo-token"
                }, options: ApiJson.Options)
            };
        }));

        var nav = ctx.Services.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>();
        nav.NavigateTo("/login?returnUrl=leagues");

        var cut = ctx.RenderComponent<Login>();

        cut.Find("#login-email").Change("demo@example.com");
        cut.Find("#login-password").Change("secret");
        cut.Find("form").Submit();

        cut.WaitForAssertion(() => Assert.Equal("http://localhost/leagues", nav.Uri));
    }

    [Fact]
    public void Submit_WhenApiFails_ShowsErrorMessage()
    {
        using var ctx = new TestContext();
        ctx.Services.AddPickDriverTestServices(new StubHttpMessageHandler(_ =>
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = JsonContent.Create(new ApiError { Error = true, Reason = "Credenciales invalidas" }, options: ApiJson.Options)
            };
        }));

        var cut = ctx.RenderComponent<Login>();

        cut.Find("#login-email").Change("demo@example.com");
        cut.Find("#login-password").Change("bad");
        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains(AppStrings.Translate("Credenciales invalidas"), cut.Markup);
        });
    }

    [Fact]
    public void Submit_WhenEmailNotVerified_ShowsResendVerificationAction()
    {
        using var ctx = new TestContext();
        ctx.Services.AddPickDriverTestServices(new StubHttpMessageHandler(_ =>
        {
            return new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = JsonContent.Create(new ApiError { Error = true, Reason = "Email no verificado" }, options: ApiJson.Options)
            };
        }));

        var cut = ctx.RenderComponent<Login>();

        cut.Find("#login-email").Change("demo@example.com");
        cut.Find("#login-password").Change("secret");
        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains(AppStrings.Translate("Tu cuenta aun no esta verificada."), cut.Markup);
            Assert.Contains(AppStrings.Translate("Reenviar email de verificacion"), cut.Markup);
        });
    }
}
