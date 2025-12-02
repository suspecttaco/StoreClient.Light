using Newtonsoft.Json;

namespace StoreClient.Light.Models;

public class Product : CatalogItem
{
    
    [JsonProperty("code")]
    public string Code { get; set; }
    
    [JsonProperty("category_id")]
    public int? CategoryId { get; set; }
    
    [JsonProperty("category")] 
    public string CategoryName { get; set; }

    [JsonProperty("buy_price")]
    public decimal BuyPrice { get; set; }

    [JsonProperty("sell_price")]
    public decimal SellPrice { get; set; }

    [JsonProperty("stock")] 
    public decimal Stock { get; set; }

    [JsonProperty("min_stock")] 
    public decimal MinStock { get; set; } 
    
}