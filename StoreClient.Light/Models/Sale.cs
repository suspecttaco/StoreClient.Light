using Newtonsoft.Json;

namespace StoreClient.Light.Models;

public class Sale
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("user_id")]
    public int UserId { get; set; }

    [JsonProperty("customer_id")]
    public int? CustomerId { get; set; } 

    [JsonProperty("total")]
    public decimal Total { get; set; }

    [JsonProperty("payment_method")]
    public string PaymentMethod { get; set; } = "cash"; // 'cash', 'card', 'credit'

    [JsonProperty("date")]
    public DateTime Date { get; set; }

    // Lista de productos vendidos
    [JsonProperty("products")]
    public List<SaleDetail> Details { get; set; } = new List<SaleDetail>();
    
    [JsonProperty("user")]
    public string UserName { get; set; }
    
    [JsonProperty("customer")]
    public string CustomerName { get; set; }
}