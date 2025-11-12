using woocommerce_zoho_integration;
using woocommerce_zoho_integration.ServiceContracts;
using woocommerce_zoho_integration.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<IntegrationWorker>();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IWooCommerceService, WooCommerceService>();
builder.Services.AddSingleton<IZohoCRMService, ZohoCRMService>();
builder.Services.AddSingleton<IIntegrationMapper, IntegrationMapper>();
builder.Services.AddHostedService<IntegrationWorker>();
var host = builder.Build();
host.Run();
