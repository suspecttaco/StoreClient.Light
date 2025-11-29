using Newtonsoft.Json;
using RestSharp;
using StoreClient.Light.Models;
using StoreClient.Light.Utils;

namespace StoreClient.Light.Services;

public class ApiService
{
    private readonly string _baseUrl;
    private readonly RestClient _client;

    public ApiService()
    {
        _baseUrl = AppConfig.BaseUrl;
        
        var options = new RestClientOptions(_baseUrl)
        {
            RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
        };
        _client = new RestClient(_baseUrl);
    }
    
    // Testear conexion
    public async Task<bool> CheckConnection()
    {
        try
        {
            var request = new RestRequest("products/");
            var response = await _client.ExecuteAsync(request);
            return response.IsSuccessful;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    // Login
    public async Task<LoginResponse?> LoginAsync(string username, string password)
    {
        var request = new RestRequest("auth/login", Method.Post);
        request.AddJsonBody(new { username, password });

        try
        {
            var response = await _client.ExecuteAsync(request);
            if (response is { IsSuccessful: true, Content: not null })
            {
                return JsonConvert.DeserializeObject<LoginResponse>(response.Content);
            }
            
            // Si falla
            return new LoginResponse { Error = $"[ERROR] {response.StatusCode}" };
        }
        catch (Exception e)
        {
            return new LoginResponse { Error = $"[ERROR] Sin conexion: {e.Message}" };
        }
    }
    
    // Metodo generico para listas
    public async Task<List<T>?> GetListAsync<T>(string endpoint)
    {
        var request = new RestRequest(endpoint);

        try
        {
            var response = await _client.ExecuteAsync(request);
            if (response is { IsSuccessful: true, Content: not null })
            {
                return JsonConvert.DeserializeObject<List<T>>(response.Content);
            }

            return new List<T>();
        }
        catch (Exception)
        {
            return new List<T>();
        }
    }

    public async Task<T?> GetByIdAsync<T>(string endpoint)
    {
        var request = new RestRequest(endpoint);

        try
        {
            var response = await _client.ExecuteAsync(request);

            if (response.IsSuccessful && response.Content != null)
            {
                return JsonConvert.DeserializeObject<T>(response.Content);
            }

            return default(T);
        }
        catch (Exception )
        {
            return default(T);
        }
    }
    
    // Metodo generico para crear
    public async Task<bool> PostAsync<T>(string endpoint, T data) where T : class
    {
        var request = new RestRequest(endpoint, Method.Post);
        request.AddJsonBody(data);

        try
        {
            var response = await _client.ExecuteAsync(request);
            return response.IsSuccessful;
        }
        catch (Exception )
        {
            return false;
        }
    }
    
    // Metodo para actualizar
    public async Task<bool> PutAsync<T>(string endpoint, T data) where T : class
    {
        var request = new RestRequest(endpoint, Method.Put);
        request.AddJsonBody(data);

        try
        {
            var response = await _client.ExecuteAsync(request);
            return response.IsSuccessful;
        }
        catch (Exception )
        {
            return false;
        }
    }
    
    // Metodo para eliminar
    public async Task<bool> DeleteAsync(string endpoint)
    {
        var request = new RestRequest(endpoint, Method.Delete);

        try
        {
            var response = await _client.ExecuteAsync(request);
            return response.IsSuccessful;
        }
        catch
        {
            return false;
        }
    }
    
    // Post con respuesta 
    public async Task<TResponse> PostWithResponseAsync<TResponse, TData>(string endpoint, TData data)
        where TData : class
        where TResponse : class
    {
        var request = new RestRequest(endpoint, Method.Post);
        request.AddJsonBody(data);

        try
        {
            var response = await _client.ExecuteAsync(request);

            if (response.IsSuccessful && response.Content != null)
            {
                return JsonConvert.DeserializeObject<TResponse>(response.Content);
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}