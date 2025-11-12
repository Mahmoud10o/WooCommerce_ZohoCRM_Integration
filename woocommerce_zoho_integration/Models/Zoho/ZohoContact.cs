
using System.Text.Json.Serialization;

namespace woocommerce_zoho_integration.Models.Zoho;

public class ZohoContact
{
    [JsonPropertyName("First_Name")]
    public string FirstName { get; set; }

    [JsonPropertyName("Last_Name")]
    public string LastName { get; set; }

    [JsonPropertyName("Email")]
    public string Email { get; set; }

    [JsonPropertyName("Phone")]
    public string Phone { get; set; }

    [JsonPropertyName("Mailing_Street")]
    public string MailingStreet { get; set; }

    [JsonPropertyName("Mailing_City")]
    public string MailingCity { get; set; }

    [JsonPropertyName("Mailing_State")]
    public string MailingState { get; set; }

    [JsonPropertyName("Mailing_Zip")]
    public string MailingZip { get; set; }

    [JsonPropertyName("Mailing_Country")]
    public string MailingCountry { get; set; }
}
