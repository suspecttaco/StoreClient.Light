using Newtonsoft.Json;

namespace StoreClient.Light.Models;

public class Customer : Person
{

    [JsonProperty("credit_limit")]
    public decimal CreditLimit { get; set; }

    [JsonProperty("current_balance")] // Saldo actual (Deuda)
    public decimal CurrentBalance { get; set; }
    
}