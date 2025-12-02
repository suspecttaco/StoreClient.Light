using Newtonsoft.Json;

namespace StoreClient.Light.Models;

public class ResolvedAlertPayload
{
    [JsonProperty("products")]
    public List<string> Products { get; set; } = new();
}