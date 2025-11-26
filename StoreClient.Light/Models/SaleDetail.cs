using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace StoreClient.Light.Models;

public class SaleDetail
{
    [JsonProperty("product_id")]
    [JsonPropertyName("product_id")]
    public int ProductId { get; set; }

    [JsonProperty("product_name")] // Opcional: si el backend lo manda, sirve para reportes
    [JsonPropertyName("product_name")]
    public string ProductName { get; set; }

    [JsonProperty("amount")] // Cantidad (puede ser decimal por los kilos)
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonProperty("unit_price")]
    [JsonPropertyName("unit_price")]
    public decimal UnitPrice { get; set; }

    [JsonProperty("subtotal")]
    [JsonPropertyName("subtotal")]
    public decimal Subtotal { get; set; }
}