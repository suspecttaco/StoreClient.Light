using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace StoreClient.Light.Models;

public class Sale
{
    [JsonProperty("id")]
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonProperty("user_id")]
    [JsonPropertyName("user_id")]
    public int UserId { get; set; }

    [JsonProperty("customer_id")]
    [JsonPropertyName("customer_id")]
    public int? CustomerId { get; set; } // Puede ser nulo si es venta al público general

    [JsonProperty("total")]
    [JsonPropertyName("total")]
    public decimal Total { get; set; }

    [JsonProperty("payment_method")]
    [JsonPropertyName("payment_method")]
    public string PaymentMethod { get; set; } = "cash"; // 'cash', 'card', 'credit'

    [JsonProperty("date")]
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    // Lista de productos vendidos
    [JsonProperty("products")]
    [JsonPropertyName("products")]
    public List<SaleDetail> Details { get; set; } = new List<SaleDetail>();
    
    [JsonProperty("user")]
    [JsonPropertyName("user")]
    public string UserName { get; set; }
    
    [JsonProperty("customer")]
    [JsonPropertyName("customer")]
    public string CustomerName { get; set; }
}