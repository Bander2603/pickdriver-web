using PickDriverWeb.Models.Auth;
using PickDriverWeb.State;

namespace PickDriverWeb.Services;

public sealed class AuthService
{
    private readonly ApiClient _apiClient;
    private readonly PickDriverAuthStateProvider _authStateProvider;

    public AuthService(ApiClient apiClient, PickDriverAuthStateProvider authStateProvider)
    {
        _apiClient = apiClient;
        _authStateProvider = authStateProvider;
    }

    public Task<ApiResult<RegisterResponse>> RegisterAsync(RegisterRequest request)
        => _apiClient.PostAsync<RegisterRequest, RegisterResponse>("auth/register", request);

    public async Task<ApiResult<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var result = await _apiClient.PostAsync<LoginRequest, LoginResponse>("auth/login", request);
        if (result.Success && result.Data is not null)
        {
            var session = new AuthSession
            {
                Token = result.Data.Token,
                User = result.Data.User,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _authStateProvider.SetSessionAsync(session);
        }

        return result;
    }

    public Task<ApiResult<VerifyEmailResponse>> VerifyEmailAsync(string token)
        => _apiClient.PostAsync<VerifyEmailRequest, VerifyEmailResponse>(
            "auth/verify-email",
            new VerifyEmailRequest { Token = token });

    public Task<ApiResult<ResendVerificationResponse>> ResendVerificationAsync(string email)
        => _apiClient.PostAsync<ResendVerificationRequest, ResendVerificationResponse>(
            "auth/resend-verification",
            new ResendVerificationRequest { Email = email });

    public Task LogoutAsync() => _authStateProvider.SetSessionAsync(null);
}

