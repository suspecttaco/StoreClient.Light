using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace StoreClient.Light.Models;

public class SaleDetail
{
    [JsonProperty("product_id")]
    [JsonPropertyName("product_id")]
    public int ProductId { get; set; }

    [JsonProperty("product_name")] 
    [JsonPropertyName("product_name")]
    public string ProductName { get; set; }

    [JsonProperty("amount")]
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonProperty("unit_price")]
    [JsonPropertyName("unit_price")]
    public decimal UnitPrice { get; set; }

    [JsonProperty("subtotal")]
    [JsonPropertyName("subtotal")]
    public decimal Subtotal { get; set; }
}