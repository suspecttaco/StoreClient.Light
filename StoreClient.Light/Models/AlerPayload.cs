using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization; // Importante para .NET 9 / Blazor

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

    // El backend manda 'product_name' o 'name' dependiendo de tu versión. 
    // Pondremos los dos para asegurar.
    [JsonProperty("product_name")]
    [JsonPropertyName("product_name")]
    public string ProductName { get; set; }
    
    [JsonProperty("name")] 
    [JsonPropertyName("name")]
    public string Name { get; set; } // Fallback

    [JsonProperty("current_stock")]
    [JsonPropertyName("current_stock")]
    public decimal CurrentStock { get; set; }

    [JsonProperty("min_stock")]
    [JsonPropertyName("min_stock")]
    public decimal MinStock { get; set; }
    
    // Propiedad auxiliar para obtener el nombre correcto
    public string DisplayName => !string.IsNullOrEmpty(ProductName) ? ProductName : Name;
}