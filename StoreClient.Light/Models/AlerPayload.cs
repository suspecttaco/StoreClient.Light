using Newtonsoft.Json;
using System.Collections.Generic;

namespace StoreClient.Light.Models;

public class AlertPayload
{
    [JsonProperty("products")]
    public List<StockAlertItem> Products { get; set; } = new();
}

public class StockAlertItem
{
    [JsonProperty("product_id")]
    public int ProductId { get; set; }

    [JsonProperty("product_name")]
    public string ProductName { get; set; }
    
    [JsonProperty("name")] 
    public string Name { get; set; }

    [JsonProperty("current_stock")]
    public decimal CurrentStock { get; set; }

    [JsonProperty("min_stock")]
    public decimal MinStock { get; set; }
    
    public string DisplayName => !string.IsNullOrEmpty(ProductName) ? ProductName : Name;
}