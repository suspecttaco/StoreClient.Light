using Newtonsoft.Json;

namespace StoreClient.Light.Models;

public class Category : CatalogItem
{
    
    [JsonProperty("parent_id")]
    public int? ParentId { get; set; }
    
}