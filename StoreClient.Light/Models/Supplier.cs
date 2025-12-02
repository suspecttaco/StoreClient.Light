using Newtonsoft.Json;

namespace StoreClient.Light.Models;

public class Supplier : Person
{

    [JsonProperty("contact_name")]
    public string ContactName { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }
    
}