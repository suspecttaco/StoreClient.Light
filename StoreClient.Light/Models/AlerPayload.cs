using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace StoreClient.Light.Models;

public class AlertPayload
{
    [JsonProperty("products")]
    [JsonPropertyName("products")]
    public List<StockAlertItem> Products { get; set; } = new();
}

public class StockAlertItem
{
    [JsonProperty("product_id")]
    [JsonPropertyName("product_id")]
    public int ProductId { get; set; }

    [JsonProperty("product_name")]
    [JsonPropertyName("product_name")]
    public string ProductName { get; set; }
    
    [JsonProperty("name")] 
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonProperty("current_stock")]
    [JsonPropertyName("current_stock")]
    public decimal CurrentStock { get; set; }

    [JsonProperty("min_stock")]
    [JsonPropertyName("min_stock")]
    public decimal MinStock { get; set; }
    
    public string DisplayName => !string.IsNullOrEmpty(ProductName) ? ProductName : Name;
}