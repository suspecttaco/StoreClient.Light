using Newtonsoft.Json;

namespace StoreClient.Light.Models;

public class SaleDetail
{
    [JsonProperty("product_id")]
    public int ProductId { get; set; }

    [JsonProperty("product_name")] 
    public string ProductName { get; set; }

    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    [JsonProperty("unit_price")]
    public decimal UnitPrice { get; set; }

    [JsonProperty("subtotal")]
    public decimal Subtotal { get; set; }
}