using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;
using PickDriverWeb.Components;
using PickDriverWeb.Localization;
using PickDriverWeb.Options;
using PickDriverWeb.Services;
using PickDriverWeb.State;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLocalization();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ProtectedLocalStorage>();
builder.Services.AddScoped<IAuthSessionStore, AuthSessionStore>();
builder.Services.AddScoped<PickDriverAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<PickDriverAuthStateProvider>());
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmailCooldownService>();
builder.Services.AddSingleton<AppText>();
builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection("Api"));
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en"),
        new CultureInfo("es")
    };

    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.FallBackToParentCultures = true;
    options.FallBackToParentUICultures = true;
});
builder.Services.Configure<GoogleAuthOptions>(options =>
{
    var clientId = builder.Configuration["GoogleAuth:ClientId"];
    if (string.IsNullOrWhiteSpace(clientId))
    {
        clientId = builder.Configuration["GOOGLE_CLIENT_ID"];
    }

    options.ClientId = clientId ?? string.Empty;
});
builder.Services.AddHttpClient<ApiClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<ApiOptions>>().Value;
    var baseUrl = options.BaseUrl;
    if (!baseUrl.EndsWith("/", StringComparison.Ordinal))
    {
        baseUrl += "/";
    }

    client.BaseAddress = new Uri(baseUrl);
}).ConfigurePrimaryHttpMessageHandler(sp =>
{
    var options = sp.GetRequiredService<IOptions<ApiOptions>>().Value;
    return options.UseMock ? new MockApiMessageHandler() : new HttpClientHandler();
});

var app = builder.Build();

var googleClientId = app.Configuration["GoogleAuth:ClientId"];
if (string.IsNullOrWhiteSpace(googleClientId))
{
    googleClientId = app.Configuration["GOOGLE_CLIENT_ID"];
}
app.Logger.LogInformation("GoogleAuth ClientId configured: {Configured}", !string.IsNullOrWhiteSpace(googleClientId));

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseRequestLocalization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapGet("/culture/set", (HttpContext httpContext, string culture, string? returnUrl) =>
{
    var normalizedCulture = AppStrings.NormalizeCulture(culture);
    var safeReturnUrl = string.IsNullOrWhiteSpace(returnUrl) || !Uri.TryCreate(returnUrl, UriKind.Relative, out _)
        ? "/"
        : returnUrl;

    httpContext.Response.Cookies.Append(
        CookieRequestCultureProvider.DefaultCookieName,
        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(normalizedCulture)),
        new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            HttpOnly = false
        });

    return Results.LocalRedirect(safeReturnUrl);
});
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
