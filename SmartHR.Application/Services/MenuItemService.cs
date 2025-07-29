using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartHR.Application.DTOs;
using SmartHR.Application.Interfaces;
using SmartHR.Domain.Entities;
using SmartHR.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartHR.Application.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly ID365ApiClient _d365ApiClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MenuItemService> _logger;
        public MenuItemService(ID365ApiClient d365ApiClient, IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<MenuItemService> logger)
        {
            _d365ApiClient = d365ApiClient;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<IEnumerable<MenuItemDTOs>> GetMenuItemsFromD365Async()
        {
            try
            {
                var menuItems = await _d365ApiClient.GetMenuItemsFromD365Async();
                return MapToDTOs(menuItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch menu items from D365.");
                var dummyJson = @"{
                ""value"": [
                    {""Id"":1, ""name"": ""Dashboard"", ""url"": ""/Home/Index"", ""children"": [] },
                    {   ""Id"":2,
                        ""name"": ""Employees"", 
                        ""url"": ""/Employee/Index"", 
                        ""children"": [
                            { ""Id"":1,""name"": ""All Employees"", ""url"": ""/Employee/Index"", ""children"": [] },
                            { ""Id"":2,""name"": ""Add Employee"", ""url"": ""/Employee/Index"", ""children"": [] }
                        ]
                    },
                    {   ""Id"":3,
                        ""name"": ""Leave"", 
                        ""url"": ""/Leave/Index"", 
                        ""children"": [
                            { ""Id"":1,""name"": ""Monthly Report"", ""url"": ""/Reports/Monthly"", ""children"": [] },
                            { ""Id"":2,""name"": ""Yearly Report"", ""url"": ""/Reports/Yearly"", ""children"": [] }
                        ]
                    },
                    { ""Id"":4, ""name"": ""Settings"", ""url"": ""/Settings/Index"", ""children"": [] },
                    { ""Id"":5,""name"": ""Tasks"", ""url"": ""/Tasks/Index"", ""children"": [] },
                ]
            }";
                var d365Response = JsonSerializer.Deserialize<D365ApiResponse>(dummyJson);
                return MapToDTOs(d365Response?.Value ?? new List<MenuItem>());
            }
        }

        private IEnumerable<MenuItemDTOs> MapToDTOs(IEnumerable<MenuItem> menuItems)
        {
            return menuItems.Select(item => new MenuItemDTOs
            {
                Name = item.Name,
                Url = item.Url,
                Children = MapToDTOs(item.Children).ToList()
            });
        }
        private async Task<string> GetAccessTokenAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var tokenEndpoint = $"https://login.microsoftonline.com/{_configuration["D365:TenantId"]}/oauth2/v2.0/token";
            var requestBody = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", _configuration["D365:ClientId"]),
                new KeyValuePair<string, string>("client_secret", _configuration["D365:ClientSecret"]),
                new KeyValuePair<string, string>("scope", $"{_configuration["D365:ApiUrl"]}/.default"),
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

            var response = await client.PostAsync(tokenEndpoint, requestBody);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to obtain access token.");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(jsonResponse);
            return tokenResponse?.AccessToken;
        }
    }
}
