

using woocommerce_zoho_integration.Models.WooCommerce;
using woocommerce_zoho_integration.ServiceContracts;

namespace woocommerce_zoho_integration;

public class IntegrationWorker : BackgroundService
{
    private readonly ILogger<IntegrationWorker> _logger;
    private readonly IWooCommerceService _wooService;
    private readonly IZohoCRMService _zohoService;
    private readonly IIntegrationMapper _mapper;
    private readonly IConfiguration _config;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromMinutes(1.5);
    private DateTime _lastProcessedTime = DateTime.UtcNow;

    public IntegrationWorker
        (ILogger<IntegrationWorker> logger,
        IWooCommerceService wooService,
        IZohoCRMService zohoService,
        IIntegrationMapper mapper,
        IConfiguration config)
    {
        _logger = logger;
        _wooService = wooService;
        _zohoService = zohoService;
        _mapper = mapper;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Integration Worker Service started at: {time}", DateTimeOffset.Now);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Checking for new orders...");

                // Fetch new orders from WooCommerce
                var orders = await _wooService.GetOrdersSinceAsync(_lastProcessedTime);

                if (orders.Count > 0)
                {
                    _logger.LogInformation("Found {count} new orders to process", orders.Count);

                    foreach (var order in orders)
                    {
                        await ProcessOrderAsync(order);
                    }

                    _lastProcessedTime = DateTime.UtcNow;
                }
                else
                {
                    _logger.LogInformation("No new orders found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing orders");
            }

            await Task.Delay(_pollingInterval, stoppingToken);
        }
    }

    private async Task ProcessOrderAsync(WooCommerceOrder order)
    {
        try
        {
            _logger.LogInformation("Processing Order #{orderId} for {email}", order.Id, order.Billing.Email);

            // Map WooCommerce order to CRM models
            var contact = _mapper.MapToContact(order);
            var deal = _mapper.MapToDeal(order);

            // Create/Update Contact in Zoho CRM
            var contactId = await _zohoService.CreateOrUpdateContactAsync(contact);
            _logger.LogInformation("Contact created/updated with ID: {contactId}", contactId);

            // Link the contact to the deal
            deal.ContactId = contactId;

            // Create Deal in Zoho CRM
            var dealId = await _zohoService.CreateDealAsync(deal);
            _logger.LogInformation("Deal created with ID: {dealId}", dealId);

            _logger.LogInformation("Successfully processed Order #{orderId}", order.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process Order #{orderId}", order.Id);
        }
    }
}