using Newtonsoft.Json;

namespace StoreClient.Light.Models;

public class CatalogItem : BaseModel
{
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("description")]
    public string Description { get; set; }
    
    public override string ToString()
    {
        return Name;
    }
}