
using System.Text.Json.Serialization;

namespace woocommerce_zoho_integration.Models.Zoho;

public class ZohoSearchResponse
{
    [JsonPropertyName("data")]
    public List<ZohoContactResult> Data { get; set; }
}
