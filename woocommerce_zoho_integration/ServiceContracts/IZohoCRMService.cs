
using woocommerce_zoho_integration.Models.Zoho;

namespace woocommerce_zoho_integration.ServiceContracts;

public interface IZohoCRMService
{
    Task<string> CreateOrUpdateContactAsync(ZohoContact contact);
    Task<string> CreateDealAsync(ZohoDeal deal);
}
