using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace StoreClient.Light.Models;

public class SaleResponse
{
    [JsonProperty("message")]
    [JsonPropertyName("message")]
    public string Message { get; set; }
    
    [JsonProperty("sale_id")]
    [JsonPropertyName("sale_id")]
    public int SaleId { get; set; }
}