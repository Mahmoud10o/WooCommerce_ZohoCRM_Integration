

using System.Text.Json.Serialization;

namespace woocommerce_zoho_integration.Models.Zoho;

public class ZohoCreateResponse
{
    [JsonPropertyName("data")]
    public List<ZohoCreateData> Data { get; set; }
}
