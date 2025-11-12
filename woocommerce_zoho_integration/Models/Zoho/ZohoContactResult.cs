
using System.Text.Json.Serialization;


namespace woocommerce_zoho_integration.Models.Zoho;

public class ZohoContactResult
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
}
