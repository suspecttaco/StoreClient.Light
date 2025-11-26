using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace StoreClient.Light.Models;

public class ResolvedAlertPayload
{
    [JsonProperty("products")]
    [JsonPropertyName("products")]
    public List<string> Products { get; set; } = new();
}