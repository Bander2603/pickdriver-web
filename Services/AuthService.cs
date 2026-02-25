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

    public Task<ApiResult<AuthMessageResponse>> ResendVerificationAsync(ResendVerificationRequest request)
        => _apiClient.PostAsync<ResendVerificationRequest, AuthMessageResponse>("auth/resend-verification", request);

    public Task<ApiResult<AuthMessageResponse>> ForgotPasswordAsync(ForgotPasswordRequest request)
        => _apiClient.PostAsync<ForgotPasswordRequest, AuthMessageResponse>("auth/forgot-password", request);

    public Task<ApiResult<AuthMessageResponse>> ResetPasswordAsync(ResetPasswordRequest request)
        => _apiClient.PostAsync<ResetPasswordRequest, AuthMessageResponse>("auth/reset-password", request);

    public Task<ApiResult<object?>> UpdatePasswordAsync(UpdatePasswordRequest request)
        => _apiClient.PutAsync("auth/password", request, auth: true);

    public Task<ApiResult<object?>> DeleteAccountAsync()
        => _apiClient.DeleteAsync("auth/account", auth: true);

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

    public async Task<ApiResult<LoginResponse>> GoogleLoginAsync(GoogleAuthRequest request)
    {
        var result = await _apiClient.PostAsync<GoogleAuthRequest, LoginResponse>("auth/google", request);
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

    public Task LogoutAsync() => _authStateProvider.SetSessionAsync(null);
}
