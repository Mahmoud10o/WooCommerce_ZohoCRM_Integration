

using System.Text.Json.Serialization;

namespace woocommerce_zoho_integration.Models.WooCommerce;

public class LineItem
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("total")]
    public string Total { get; set; }
}
