using Newtonsoft.Json;

namespace StoreClient.Light.Models;

public class LoginResponse
{
    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("user")]
    public User User { get; set; }

    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("error")]
    public string Error { get; set; }
}