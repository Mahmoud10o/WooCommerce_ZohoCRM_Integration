
using woocommerce_zoho_integration.Models.WooCommerce;
using woocommerce_zoho_integration.Models.Zoho;
using woocommerce_zoho_integration.ServiceContracts;

namespace woocommerce_zoho_integration.Services;

public class IntegrationMapper : IIntegrationMapper
{
    public ZohoContact MapToContact(WooCommerceOrder order)
    {
        return new ZohoContact
        {
            FirstName = order.Billing.FirstName,
            LastName = order.Billing.LastName,
            Email = order.Billing.Email,
            Phone = order.Billing.Phone,
            MailingStreet = order.Billing.Address1,
            MailingCity = order.Billing.City,
            MailingState = order.Billing.State,
            MailingZip = order.Billing.Postcode,
            MailingCountry = order.Billing.Country
        };
    }

    public ZohoDeal MapToDeal(WooCommerceOrder order)
    {
        var productNames = string.Join(", ", order.LineItems.Select(i => i.Name));

        return new ZohoDeal
        {
            DealName = $"Order #{order.Id} - {order.Billing.FirstName} {order.Billing.LastName}",
            Amount = decimal.Parse(order.Total),
            Stage = "Qualification", // Adjust based on your CRM stages
            Description = $"Products: {productNames}\nOrder Date: {order.DateCreated}",
            ClosingDate = DateTime.UtcNow.AddDays(30).ToString("yyyy-MM-dd")
        };
    }
}
