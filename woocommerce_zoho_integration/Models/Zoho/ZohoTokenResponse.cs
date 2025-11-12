
using System.Text.Json.Serialization;


namespace woocommerce_zoho_integration.Models.Zoho;

public class ZohoTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}
