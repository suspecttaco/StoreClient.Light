using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace StoreClient.Light.Models;

public class Supplier
{
    [JsonProperty("id")]
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonProperty("contact_name")]
    [JsonPropertyName("contact_name")]
    public string ContactName { get; set; }

    [JsonProperty("phone")]
    [JsonPropertyName("phone")]
    public string Phone { get; set; }

    [JsonProperty("email")]
    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonProperty("address")]
    [JsonPropertyName("address")]
    public string Address { get; set; }

    [JsonProperty("active")]
    [JsonPropertyName("active")]
    public bool Active { get; set; }

    public override string ToString()
    {
        return Name;
    }
}