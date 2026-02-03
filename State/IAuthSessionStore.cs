namespace PickDriverWeb.State;

public interface IAuthSessionStore
{
    Task<AuthSession?> GetAsync();
    Task SetAsync(AuthSession? session);
}
