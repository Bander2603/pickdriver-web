using Microsoft.JSInterop;

namespace PickDriverWeb.Services;

public enum EmailCooldownType
{
    ResendVerification,
    PasswordReset
}

public sealed class EmailCooldownService
{
    private static readonly TimeSpan DefaultDuration = TimeSpan.FromMinutes(5);
    private readonly IJSRuntime _jsRuntime;

    public EmailCooldownService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<DateTimeOffset?> GetEndsAtAsync(EmailCooldownType type)
    {
        var key = GetStorageKey(type);
        try
        {
            var rawValue = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);
            if (string.IsNullOrWhiteSpace(rawValue) || !long.TryParse(rawValue, out var unixSeconds))
            {
                return null;
            }

            var endsAt = DateTimeOffset.FromUnixTimeSeconds(unixSeconds);
            if (endsAt <= DateTimeOffset.UtcNow)
            {
                await ClearAsync(type);
                return null;
            }

            return endsAt;
        }
        catch
        {
            return null;
        }
    }

    public async Task<DateTimeOffset> StartAsync(EmailCooldownType type, TimeSpan? duration = null)
    {
        var endsAt = DateTimeOffset.UtcNow.Add(duration ?? DefaultDuration);
        await SetEndsAtAsync(type, endsAt);
        return endsAt;
    }

    public async Task SetEndsAtAsync(EmailCooldownType type, DateTimeOffset endsAt)
    {
        var key = GetStorageKey(type);
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, endsAt.ToUnixTimeSeconds().ToString());
        }
        catch
        {
            // Ignore storage errors; cooldown is best-effort in client state.
        }
    }

    public async Task ClearAsync(EmailCooldownType type)
    {
        var key = GetStorageKey(type);
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }
        catch
        {
            // Ignore storage errors; cooldown is best-effort in client state.
        }
    }

    public static int GetRemainingSeconds(DateTimeOffset? endsAt)
    {
        if (!endsAt.HasValue)
        {
            return 0;
        }

        var remaining = (int)Math.Ceiling((endsAt.Value - DateTimeOffset.UtcNow).TotalSeconds);
        return Math.Max(remaining, 0);
    }

    public static string Format(int seconds)
    {
        var safeSeconds = Math.Max(seconds, 0);
        var minutes = safeSeconds / 60;
        var remainder = safeSeconds % 60;
        return $"{minutes:00}:{remainder:00}";
    }

    private static string GetStorageKey(EmailCooldownType type)
    {
        return type switch
        {
            EmailCooldownType.ResendVerification => "pickdriver.cooldown.resendVerificationUntil",
            EmailCooldownType.PasswordReset => "pickdriver.cooldown.passwordResetUntil",
            _ => "pickdriver.cooldown.unknown"
        };
    }
}
