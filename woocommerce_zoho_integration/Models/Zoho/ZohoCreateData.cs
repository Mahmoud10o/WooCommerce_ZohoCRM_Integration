using System.Text.Json.Serialization;

namespace woocommerce_zoho_integration.Models.Zoho;

public class ZohoCreateData
{
    [JsonPropertyName("details")]
    public ZohoDetails Details { get; set; }
}