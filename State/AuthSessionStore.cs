using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace PickDriverWeb.State;

public sealed class AuthSessionStore
{
    private const string StorageKey = "pickdriver.auth.session";
    private readonly ProtectedLocalStorage _storage;
    private AuthSession? _session;
    private bool _loaded;

    public AuthSessionStore(ProtectedLocalStorage storage)
    {
        _storage = storage;
    }

    public async Task<AuthSession?> GetAsync()
    {
        if (!_loaded)
        {
            _loaded = await TryLoadAsync();
        }

        return _session;
    }

    public async Task SetAsync(AuthSession? session)
    {
        _session = session;
        _loaded = true;

        try
        {
            if (session is null)
            {
                await _storage.DeleteAsync(StorageKey);
            }
            else
            {
                await _storage.SetAsync(StorageKey, session);
            }
        }
        catch
        {
            // Ignore storage failures (e.g., during prerender).
        }
    }

    private async Task<bool> TryLoadAsync()
    {
        try
        {
            var result = await _storage.GetAsync<AuthSession>(StorageKey);
            _session = result.Success ? result.Value : null;
            return true;
        }
        catch
        {
            _session = null;
            return false;
        }
    }
}

