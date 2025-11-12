
using woocommerce_zoho_integration.Models.WooCommerce;
using woocommerce_zoho_integration.Models.Zoho;

namespace woocommerce_zoho_integration.ServiceContracts;

public interface IIntegrationMapper
{
    ZohoContact MapToContact(WooCommerceOrder order);
    ZohoDeal MapToDeal(WooCommerceOrder order);
}
