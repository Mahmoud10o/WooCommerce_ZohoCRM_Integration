
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using woocommerce_zoho_integration.Models.Zoho;
using woocommerce_zoho_integration.ServiceContracts;

namespace woocommerce_zoho_integration.Services;

public class ZohoCRMService : IZohoCRMService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ZohoCRMService> _logger;
    private readonly IConfiguration _config;
    private string _accessToken = "";
    private DateTime _tokenExpiry = DateTime.MinValue;
    public ZohoCRMService(IHttpClientFactory httpClientFactory, ILogger<ZohoCRMService> logger, IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _config = config;
    }


    private async Task<string> GetAccessTokenAsync()
    {
        // Check if current token is still valid
        if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiry)
        {
            return _accessToken;
        }

        try
        {
            _logger.LogInformation("Refreshing Zoho access token...");

            var client = _httpClientFactory.CreateClient();
            var url = $"{_config["ZohoCRM:AccountsUrl"]}" +
                      $"refresh_token={_config["ZohoCRM:RefreshToken"]}&" +
                      $"client_id={_config["ZohoCRM:ClientId"]}&" +
                      $"client_secret={_config["ZohoCrm:ClientSecret"]}&" +
                      $"grant_type=refresh_token";

            var response = await client.PostAsync(url, null);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<ZohoTokenResponse>(json);

            _accessToken = tokenResponse.AccessToken;
            _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 300); // 5 min buffer

            _logger.LogInformation("Access token refreshed successfully");
            return _accessToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing Zoho access token");
            throw;
        }
    }


    public async Task<string> CreateOrUpdateContactAsync(ZohoContact contact)
    {
        try
        {
            var token = await GetAccessTokenAsync();
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Zoho-oauthtoken", token);

            // First, search for existing contact by email
            var searchUrl = $"{_config["ZohoCRM:ApiBaseUrl"]}/Contacts/search?email={contact.Email}";
            var searchResponse = await client.GetAsync(searchUrl);

            if (searchResponse.StatusCode == HttpStatusCode.OK)
            {
                var searchJson = await searchResponse.Content.ReadAsStringAsync();
                var searchResult = JsonSerializer.Deserialize<ZohoSearchResponse>(searchJson);

                if (searchResult?.Data?.Any() == true)
                {
                    // Update existing contact
                    var existingId = searchResult.Data[0].Id;
                    _logger.LogInformation("Contact exists, updating ID: {id}", existingId);

                    var updatePayload = new { data = new[] { contact } };
                    var updateContent = new StringContent(
                        JsonSerializer.Serialize(updatePayload),
                        Encoding.UTF8,
                        "application/json");

                    var updateUrl = $"{_config["ZohoCRM:ApiBaseUrl"]}/Contacts/{existingId}";
                    await client.PutAsync(updateUrl, updateContent);

                    return existingId;
                }
            }

            // Create new contact
            _logger.LogInformation("Creating new contact for {email}", contact.Email);

            var createPayload = new { data = new[] { contact } };
            var createContent = new StringContent(
                JsonSerializer.Serialize(createPayload),
                Encoding.UTF8,
                "application/json");

            var createResponse = await client.PostAsync($"{_config["ZohoCRM:ApiBaseUrl"]}/Contacts", createContent);
            createResponse.EnsureSuccessStatusCode();

            var createJson = await createResponse.Content.ReadAsStringAsync();
            var createResult = JsonSerializer.Deserialize<ZohoCreateResponse>(createJson);

            return createResult?.Data?[0]?.Details?.Id ?? "";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating/updating contact in Zoho");
            throw;
        }
    }

    public async Task<string> CreateDealAsync(ZohoDeal deal)
    {
        try
        {
            var token = await GetAccessTokenAsync();
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Zoho-oauthtoken", token);

            var payload = new { data = new[] { deal } };
            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync($"{_config["ZohoCRM:ApiBaseUrl"]}/Deals", content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ZohoCreateResponse>(json);

            var dealId = result?.Data?[0]?.Details?.Id ?? "";
            _logger.LogInformation("Deal created successfully: {id}", dealId);

            return dealId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating deal in Zoho");
             throw;
        }
    }
}



