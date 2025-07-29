using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartHR.Application.DTOs;
using SmartHR.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartHR.Application.D365ApiClient
{
    public class D365AccountClientApi : ID365AccountClientApi
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<D365AccountClientApi> _logger;

        public D365AccountClientApi(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<D365AccountClientApi> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var client = _httpClientFactory.CreateClient();

            // Safely access configuration using GetRequiredSection to avoid null reference warnings
            var d365Config = _configuration.GetRequiredSection("D365");
            var tokenEndpoint = d365Config["TockenUrl"] ?? "https://login.microsoftonline.com/b445bfd7-8ff7-4f9b-b1f3-f597234bc8fd/oauth2/token";

            var clientId = d365Config["ClientId"] ?? throw new InvalidOperationException("D365:ClientId is missing in configuration.");
            var clientSecret = d365Config["ClientSecret"] ?? throw new InvalidOperationException("D365:ClientSecret is missing in configuration.");
            var apiUrl = d365Config["ApiUrl"] ?? throw new InvalidOperationException("D365:ApiUrl is missing in configuration.");

            var requestBody = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("resource", apiUrl),
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

            try
            {
                var response = await client.PostAsync(tokenEndpoint, requestBody);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to obtain access token. Status: {StatusCode}, Error: {Error}", response.StatusCode, errorContent);
                    throw new Exception($"Failed to obtain access token. Status: {response.StatusCode}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(jsonResponse);
                if (tokenResponse?.AccessToken == null)
                {
                    _logger.LogError("Access token is null in token response: {JsonResponse}", jsonResponse);
                    throw new Exception("Access token is null.");
                }

                return tokenResponse.AccessToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obtaining access token.");
                throw;
            }
        }
        public bool TryParseDate(string dateStr, out string result)
        {
            string[] formats = { "dd-MM-yyyy", "dd/MM/yyyy" }; // Input formats to parse
            result = string.Empty; // Initialize output

            // Try to parse the input date string
            if (DateTime.TryParseExact(dateStr, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                // Convert the parsed date to the desired format "dd/MM/yyyy"
                result = parsedDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                return true;
            }
            return false;
        }

        public bool TryParseDateToString(string? dateStr, out DateTime result)
        {
            result = default;
            if (string.IsNullOrEmpty(dateStr))
                return false;

            string[] formats = { "dd/MM/yyyy", "dd-MM-yyyy" };
            return DateTime.TryParseExact(dateStr, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        }
    }
}
