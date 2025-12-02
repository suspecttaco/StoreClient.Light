using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace StoreClient.Light.Models;

public class User : BaseModel
{
    
    [JsonProperty("username")]
    public string Username { get; set; }
    
    [JsonProperty("password")]
    public string Password { get; set; }
    
    [JsonProperty("full_name")]
    public string FullName { get; set; }
    
    [JsonProperty("role")]
    public string Role { get; set; }
    
}