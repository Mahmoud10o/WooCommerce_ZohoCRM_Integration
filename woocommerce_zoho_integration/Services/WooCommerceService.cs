
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using woocommerce_zoho_integration.Models.WooCommerce;
using woocommerce_zoho_integration.ServiceContracts;

namespace woocommerce_zoho_integration.Services;

public class WooCommerceService : IWooCommerceService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WooCommerceService> _logger;
    private readonly IConfiguration _config;
    public WooCommerceService(IHttpClientFactory httpClientFactory, ILogger<WooCommerceService> logger, IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _config = config;
    }
    public async Task<List<WooCommerceOrder>> GetOrdersSinceAsync(DateTime since)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            // Basic Auth for WooCommerce
            var authToken = Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{_config["WooCommerce:ConsumerKey"]}:{_config["WooCommerce:ConsumerSecret"]}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            var sinceParam = since.ToString("yyyy-MM-ddTHH:mm:ss");
            var url = $"{_config["WooCommerce:BaseUrl"]}/orders?after={sinceParam}&per_page=2";

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var orders = JsonSerializer.Deserialize<List<WooCommerceOrder>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return orders ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching orders from WooCommerce");
            throw;
        }
    }
}

