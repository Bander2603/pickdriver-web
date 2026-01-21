using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using PickDriverWeb.Models;
using PickDriverWeb.State;

namespace PickDriverWeb.Services;

public sealed class ApiClient
{
    private readonly HttpClient _http;
    private readonly AuthSessionStore _sessionStore;

    public ApiClient(HttpClient http, AuthSessionStore sessionStore)
    {
        _http = http;
        _sessionStore = sessionStore;
    }

    public Task<ApiResult<TResponse>> GetAsync<TResponse>(string path, bool auth = false)
        => SendAsync<TResponse>(HttpMethod.Get, path, null, auth);

    public Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(string path, TRequest request, bool auth = false)
        => SendAsync<TResponse>(HttpMethod.Post, path, request, auth);

    public Task<ApiResult<object?>> PostAsync<TRequest>(string path, TRequest request, bool auth = false)
        => SendAsync<object?>(HttpMethod.Post, path, request, auth);

    private async Task<ApiResult<TResponse>> SendAsync<TResponse>(HttpMethod method, string path, object? body, bool auth)
    {
        using var message = new HttpRequestMessage(method, path);
        if (body is not null)
        {
            message.Content = JsonContent.Create(body, options: ApiJson.Options);
        }

        if (auth)
        {
            var session = await _sessionStore.GetAsync();
            if (session is not null && !string.IsNullOrWhiteSpace(session.Token))
            {
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.Token);
            }
        }

        using var response = await _http.SendAsync(message);

        if (response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)
            {
                return new ApiResult<TResponse> { Success = true, StatusCode = (int)response.StatusCode };
            }

            try
            {
                var data = await response.Content.ReadFromJsonAsync<TResponse>(ApiJson.Options);
                return new ApiResult<TResponse>
                {
                    Success = true,
                    Data = data,
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (JsonException)
            {
                return new ApiResult<TResponse>
                {
                    Success = false,
                    ErrorMessage = "Respuesta inesperada del servidor.",
                    StatusCode = (int)response.StatusCode
                };
            }
        }

        var errorMessage = await TryReadErrorAsync(response);
        return new ApiResult<TResponse>
        {
            Success = false,
            ErrorMessage = errorMessage,
            StatusCode = (int)response.StatusCode
        };
    }

    private static async Task<string?> TryReadErrorAsync(HttpResponseMessage response)
    {
        try
        {
            var error = await response.Content.ReadFromJsonAsync<ApiError>(ApiJson.Options);
            if (!string.IsNullOrWhiteSpace(error?.Reason))
            {
                return error.Reason;
            }
        }
        catch
        {
            // Fall through to raw content.
        }

        try
        {
            var raw = await response.Content.ReadAsStringAsync();
            return string.IsNullOrWhiteSpace(raw) ? null : raw;
        }
        catch
        {
            return null;
        }
    }
}
