

using System.Text.Json.Serialization;

namespace woocommerce_zoho_integration.Models.Zoho;

public class ZohoDeal
{
    [JsonPropertyName("Deal_Name")]
    public string DealName { get; set; }

    [JsonPropertyName("Amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("Stage")]
    public string Stage { get; set; }

    [JsonPropertyName("Closing_Date")]
    public string ClosingDate { get; set; }

    [JsonPropertyName("Description")]
    public string Description { get; set; }

    [JsonPropertyName("Contact_Name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ContactReference ContactName { get; set; }

    [JsonIgnore]
    public string ContactId { get; set; }
}
