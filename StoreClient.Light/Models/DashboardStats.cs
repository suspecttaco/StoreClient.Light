using Newtonsoft.Json;

namespace StoreClient.Light.Models;

public class DashboardStats
{
    [JsonProperty("total_sales")]
    public decimal TotalSales { get; set; }
    
    [JsonProperty("sales_count")]
    public int SalesCount { get; set; }
    
    [JsonProperty("low_stock_count")]
    public int LowStockCount { get; set; }
    
    [JsonProperty("total_products")]
    public int TotalProducts { get; set; }
    
    [JsonProperty("trend_labels")]
    public List<string> TrendLabels { get; set; } = new();

    [JsonProperty("trend_values")]
    public List<decimal> TrendValues { get; set; } = new();
}