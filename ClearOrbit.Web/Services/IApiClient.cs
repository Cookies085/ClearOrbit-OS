using System.Net.Http.Headers;

namespace ClearOrbit.Web.Services;

public interface IApiClient
{
    Task<T?> GetAsync<T>(string endpoint, string? token = null);
    Task<T?> PostAsync<T>(string endpoint, object payload, string? token = null);
    Task<T?> PutAsync<T>(string endpoint, object payload, string? token = null);
    Task<T?> DeleteAsync<T>(string endpoint, string? token = null);
    Task<(bool Success, string? Error)> PostRawAsync(string endpoint, object payload);
}