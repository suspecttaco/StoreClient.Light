using Newtonsoft.Json;

namespace StoreClient.Light.Models;

public class SaleResponse
{
    [JsonProperty("message")]
    public string Message { get; set; }
    
    [JsonProperty("sale_id")]
    public int SaleId { get; set; }
}