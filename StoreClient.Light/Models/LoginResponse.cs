using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace StoreClient.Light.Models;

public class LoginResponse
{
    [JsonProperty("message")]
    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonProperty("user")]
    [JsonPropertyName("user")]
    public User User { get; set; }

    [JsonProperty("token")]
    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonProperty("error")]
    [JsonPropertyName("error")]
    public string Error { get; set; }
}