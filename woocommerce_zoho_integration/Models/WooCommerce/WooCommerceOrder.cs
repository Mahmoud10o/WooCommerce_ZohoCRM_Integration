

using System.Text.Json.Serialization;

namespace woocommerce_zoho_integration.Models.WooCommerce;

public class WooCommerceOrder
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("date_created")]
    public string DateCreated { get; set; }

    [JsonPropertyName("total")]
    public string Total { get; set; }

    [JsonPropertyName("billing")]
    public BillingDetails Billing { get; set; }

    [JsonPropertyName("line_items")]
    public List<LineItem> LineItems { get; set; }
}
