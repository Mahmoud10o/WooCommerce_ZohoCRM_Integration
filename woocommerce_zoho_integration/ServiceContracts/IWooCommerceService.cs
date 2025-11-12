

using woocommerce_zoho_integration.Models.WooCommerce;

namespace woocommerce_zoho_integration.ServiceContracts;

public interface IWooCommerceService
{
    Task<List<WooCommerceOrder>> GetOrdersSinceAsync(DateTime since);
}
