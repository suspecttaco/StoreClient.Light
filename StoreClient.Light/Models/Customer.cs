using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace StoreClient.Light.Models;

public class Customer
{
    [JsonProperty("id")]
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonProperty("phone")]
    [JsonPropertyName("phone")]
    public string Phone { get; set; }

    [JsonProperty("address")]
    [JsonPropertyName("address")]
    public string Address { get; set; }

    [JsonProperty("credit_limit")]
    [JsonPropertyName("credit_limit")]
    public decimal CreditLimit { get; set; }

    [JsonProperty("current_balance")] // Saldo actual (Deuda)
    [JsonPropertyName("current_balance")]
    public decimal CurrentBalance { get; set; }

    [JsonProperty("active")]
    [JsonPropertyName("active")]
    public bool Active { get; set; }

    // Para los ComboBox
    public override string ToString()
    {
        return Name;
    }
}