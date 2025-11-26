using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace StoreClient.Light.Models;

public class User
{
    [JsonProperty("id")]
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonProperty("username")]
    [JsonPropertyName("username")]
    public string Username { get; set; }
    
    [JsonProperty("password")]
    [JsonPropertyName("password")]
    public string Password { get; set; }
    
    [JsonProperty("full_name")]
    [JsonPropertyName("full_name")]
    public string FullName { get; set; }
    
    [JsonProperty("role")]
    [JsonPropertyName("role")]
    public string Role { get; set; }
    
    [JsonProperty("active")]
    [JsonPropertyName("active")]
    public bool Active { get; set; }
}