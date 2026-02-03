using PickDriverWeb.State;

namespace PickDriverWeb.Tests.Infrastructure;

internal sealed class FakeAuthSessionStore : IAuthSessionStore
{
    private AuthSession? _session;

    public FakeAuthSessionStore(AuthSession? session = null)
    {
        _session = session;
    }

    public Task<AuthSession?> GetAsync() => Task.FromResult(_session);

    public Task SetAsync(AuthSession? session)
    {
        _session = session;
        return Task.CompletedTask;
    }
}
