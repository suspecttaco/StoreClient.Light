using Newtonsoft.Json;

namespace StoreClient.Light.Models;

public class BaseModel
{
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("active")]
    public bool Active { get; set; }
}