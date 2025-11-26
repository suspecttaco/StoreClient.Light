using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace StoreClient.Light.Models;

public class Product
{
    [JsonProperty("id")]
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonProperty("code")]
    [JsonPropertyName("code")]
    public string Code { get; set; }
    
    [JsonProperty("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonProperty("description")]
    [JsonPropertyName("description")]
    public string Description { get; set; }
    
    [JsonProperty("category_id")]
    [JsonPropertyName("category_id")]
    public int? CategoryId { get; set; }
    
    [JsonProperty("category")] 
    [JsonPropertyName("category")]
    public string CategoryName { get; set; }

    [JsonProperty("buy_price")]
    [JsonPropertyName("buy_price")]
    public decimal BuyPrice { get; set; }

    [JsonProperty("sell_price")]
    [JsonPropertyName("sell_price")]
    public decimal SellPrice { get; set; }

    [JsonProperty("stock")] 
    [JsonPropertyName("stock")]
    public decimal Stock { get; set; }

    [JsonProperty("min_stock")] 
    [JsonPropertyName("min_stock")]
    public decimal MinStock { get; set; } 
        
    [JsonProperty("active")]
    [JsonPropertyName("active")]
    public bool Active { get; set; }
}