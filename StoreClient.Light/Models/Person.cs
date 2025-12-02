using Newtonsoft.Json;

namespace StoreClient.Light.Models;

public class Person : BaseModel
{
    
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("phone")]
    public string Phone { get; set; }
    
    [JsonProperty("address")]
    public string Address { get; set; }
    
    public override string ToString()
    {
        return Name;
    }
}