using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ClearOrbit.Web.Settings;
using Microsoft.Extensions.Options;

namespace ClearOrbit.Web.Services;

public class ApiClient : IApiClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _json;

    public ApiClient(HttpClient http, IOptions<ApiSettings> settings)
    {
        _http = http;
        _http.BaseAddress = new Uri(settings.Value.BaseUrl);

        _json = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<T?> GetAsync<T>(string endpoint, string? token = null)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, endpoint);
        AttachToken(req, token);
        using var res = await _http.SendAsync(req);
        return await ReadAsync<T>(res);
    }

    public async Task<T?> PostAsync<T>(string endpoint, object payload, string? token = null)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = JsonContent(payload)
        };
        AttachToken(req, token);
        using var res = await _http.SendAsync(req);
        return await ReadAsync<T>(res);
    }

    public async Task<T?> PutAsync<T>(string endpoint, object payload, string? token = null)
    {
        using var req = new HttpRequestMessage(HttpMethod.Put, endpoint)
        {
            Content = JsonContent(payload)
        };
        AttachToken(req, token);
        using var res = await _http.SendAsync(req);
        return await ReadAsync<T>(res);
    }

    public async Task<T?> DeleteAsync<T>(string endpoint, string? token = null)
    {
        using var req = new HttpRequestMessage(HttpMethod.Delete, endpoint);
        AttachToken(req, token);
        using var res = await _http.SendAsync(req);
        return await ReadAsync<T>(res);
    }

    public async Task<(bool Success, string? Error)> PostRawAsync(string endpoint, object payload)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = JsonContent(payload)
        };
        using var res = await _http.SendAsync(req);
        var body = await res.Content.ReadAsStringAsync();
        return res.IsSuccessStatusCode ? (true, null) : (false, body);
    }

    private static StringContent JsonContent(object payload) =>
        new(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

    private static void AttachToken(HttpRequestMessage req, string? token)
    {
        if (!string.IsNullOrWhiteSpace(token))
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<T?> ReadAsync<T>(HttpResponseMessage res)
    {
        var body = await res.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(body)) return default;
        try
        {
            return JsonSerializer.Deserialize<T>(body, _json);
        }
        catch
        {
            return default;
        }
    }
}