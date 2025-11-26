using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace StoreClient.Light.Models;

public class Category
{
    [JsonProperty("id")]
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonProperty("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonProperty("description")]
    [JsonPropertyName("description")]
    public string Description { get; set; }
    
    [JsonProperty("parent_id")]
    [JsonPropertyName("parent_id")]
    public int? ParentId { get; set; }

    [JsonProperty("active")]
    [JsonPropertyName("active")]
    public bool Active { get; set; }

    public override string ToString()
    {
        return Name;
    }
}