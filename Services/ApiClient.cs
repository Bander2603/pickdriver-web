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
    private readonly IAuthSessionStore _sessionStore;

    public ApiClient(HttpClient http, IAuthSessionStore sessionStore)
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

    public Task<ApiResult<TResponse>> PutAsync<TRequest, TResponse>(string path, TRequest request, bool auth = false)
        => SendAsync<TResponse>(HttpMethod.Put, path, request, auth);

    public Task<ApiResult<object?>> PutAsync<TRequest>(string path, TRequest request, bool auth = false)
        => SendAsync<object?>(HttpMethod.Put, path, request, auth);

    public Task<ApiResult<TResponse>> DeleteAsync<TResponse>(string path, bool auth = false)
        => SendAsync<TResponse>(HttpMethod.Delete, path, null, auth);

    public Task<ApiResult<object?>> DeleteAsync(string path, bool auth = false)
        => SendAsync<object?>(HttpMethod.Delete, path, null, auth);

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

        HttpResponseMessage response;
        try
        {
            response = await _http.SendAsync(message);
        }
        catch (HttpRequestException)
        {
            return new ApiResult<TResponse>
            {
                Success = false,
                ErrorMessage = "No se pudo conectar con la API.",
                StatusCode = 0
            };
        }
        catch (TaskCanceledException)
        {
            return new ApiResult<TResponse>
            {
                Success = false,
                ErrorMessage = "La solicitud a la API expiro.",
                StatusCode = 0
            };
        }

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
