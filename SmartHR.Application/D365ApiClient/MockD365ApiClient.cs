using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using SmartHR.Application.DTOs;
using SmartHR.Domain.Entities;
using SmartHR.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartHR.Application.D365ApiClient
{
    public class MockD365ApiClient : ID365ApiClient
    {

        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IConfidentialClientApplication _clientApp;
        private readonly ILogger<MockD365ApiClient> _logger;

        public MockD365ApiClient(HttpClient httpClient, IConfiguration configuration, ILogger<MockD365ApiClient> logger, IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["D365:BaseUrl"] ?? "https://your-d365-instance.api.crm.dynamics.com/api/data/v9.2/");

            // Initialize MSAL for client credentials flow(if Future impletement)
            _clientApp = ConfidentialClientApplicationBuilder
                .Create(_configuration["AzureAd:ClientId"])
                .WithClientSecret(_configuration["AzureAd:ClientSecret"])
                .WithAuthority(new Uri($"{_configuration["AzureAd:Instance"]}{_configuration["AzureAd:TenantId"]}"))
                .Build();
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }


        public Task<IEnumerable<Employee>> GetEmployeesAsync()
        {
            var employees = new List<Employee>
            {
                new Employee(Guid.NewGuid(), "John Doe", "Engineering", "john.doe@company.com"),
                new Employee(Guid.NewGuid(), "Jane Smith", "HR", "jane.smith@company.com"),
                new Employee(Guid.NewGuid(), "Mike Johnson", "Finance", "mike.johnson@company.com"),
				new Employee(Guid.NewGuid(), "Mike1 Johnson", "Finance", "mike.johnson@company.com"),
				new Employee(Guid.NewGuid(), "Mike2 Johnson", "Finance", "mike.johnson@company.com"),
				new Employee(Guid.NewGuid(), "Mike3 Johnson", "Finance", "mike.johnson@company.com")
			};
            return Task.FromResult<IEnumerable<Employee>>(employees);
        }

        public Task<IEnumerable<LeaveRequest>> GetLeaveRequestsAsync()
        {
            var employeeId = Guid.NewGuid(); // Simplified for demo
            var leaveRequests = new List<LeaveRequest>
            {
                new LeaveRequest(Guid.NewGuid(), employeeId, DateTime.Now.AddDays(-5), DateTime.Now.AddDays(-3), "Approved"),
                new LeaveRequest(Guid.NewGuid(), employeeId, DateTime.Now.AddDays(1), DateTime.Now.AddDays(3), "Pending"),
                new LeaveRequest(Guid.NewGuid(), employeeId, DateTime.Now.AddDays(5), DateTime.Now.AddDays(7), "Rejected")
            };
            return Task.FromResult<IEnumerable<LeaveRequest>>(leaveRequests);
        }

        //public async Task<IEnumerable<MenuItem>> GetMenuItemsFromD365Async()
        //{
        //    var dummyJson = @"{
        //        ""value"": [
        //            {""Id"":1, ""name"": ""Dashboard"", ""url"": ""/Home/Index"", ""children"": [] },
        //            {   ""Id"":2,
        //                ""name"": ""Employees"", 
        //                ""url"": ""/Employee/Index"", 
        //                ""children"": [
        //                    { ""name"": ""All Employees"", ""url"": ""/Employee/Index"", ""children"": [] },
        //                    { ""name"": ""Add Employee"", ""url"": ""/Employee/Index"", ""children"": [] }
        //                ]
        //            },
        //            {   ""Id"":3,
        //                ""name"": ""Leave"", 
        //                ""url"": ""/Leave/Index"", 
        //                ""children"": [
        //                    { ""name"": ""Monthly Report"", ""url"": ""/Leave/Index"", ""children"": [] },
        //                    { ""name"": ""Yearly Report"", ""url"": ""/Leave/Index"", ""children"": [] }
        //                ]
        //            },
        //            { ""Id"":4, ""name"": ""Settings"", ""url"": ""/Settings/Index"", ""children"": [] },
        //            { ""Id"":5,""name"": ""Tasks"", ""url"": ""/Tasks/Index"", ""children"": [] }
        //        ]
        //    }";

        //    try
        //    {
        //        var client = _httpClientFactory.CreateClient();
        //        var token = await GetAccessTokenAsync();
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //        var d365ApiUrl = _configuration["D365:ApiUrl"];
        //        var response = await client.GetAsync($"{d365ApiUrl}?$select=name,url&$expand=children($select=name,url)");

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var json = await response.Content.ReadAsStringAsync();
        //            var d365Response = JsonSerializer.Deserialize<D365ApiResponse>(json);
        //            return d365Response?.Value ?? new List<MenuItem>();
        //        }
        //        else
        //        {
        //            _logger.LogWarning("D365 API request failed with status code: {StatusCode}", response.StatusCode);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Failed to fetch menu items from D365 API.");
        //    }

        //    _logger.LogInformation("Returning dummy menu data.");
        //    var dummyResponse = JsonSerializer.Deserialize<D365ApiResponse>(dummyJson);
        //    return dummyResponse?.Value ?? new List<MenuItem>();

        //}



        public async Task<IEnumerable<MenuItem>> GetMenuItemsFromD365Async()
        {
            // Hardcoded dummy JSON for fallback
            var dummyJson = @"{
        ""value"": [
            {""Id"":1, ""name"": ""Dashboard"", ""url"": ""/Home/Index"", ""children"": [] },
            {   ""Id"":2,
                ""name"": ""Employees"", 
                ""url"": ""/Employee/Index"", 
                ""children"": [
                    { ""name"": ""All Employees"", ""url"": ""/Employee/Index"", ""children"": [] },
                    { ""name"": ""Add Employee"", ""url"": ""/Employee/Index"", ""children"": [] }
                ]
            },
            {   ""Id"":3,
                ""name"": ""Leave"", 
                ""url"": ""/Leave/Index"", 
                ""children"": [
                    { ""name"": ""Monthly Report"", ""url"": ""/Leave/Index"", ""children"": [] },
                    { ""name"": ""Yearly Report"", ""url"": ""/Leave/Index"", ""children"": [] }
                ]
            },
            { ""Id"":4, ""name"": ""Settings"", ""url"": ""/Settings/Index"", ""children"": [] },
            { ""Id"":5,""name"": ""Tasks"", ""url"": ""/Tasks/Index"", ""children"": [] }
        ]
    }";

            try
            {
                var client = _httpClientFactory.CreateClient();
                var token=string.Empty;
                //var token = await GetAccessTokenAsync();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var d365Config = _configuration.GetRequiredSection("D365");
                var apiUrl = d365Config["ApiUrl"] ?? throw new InvalidOperationException("D365:ApiUrl is missing in configuration.");
                // Adjust endpoint to your specific OData entity (e.g., MenuItems)
                var endpoint = $"{apiUrl}/data/MenuItems?$select=Id,name,url&$expand=children($select=name,url)";

                var response = await client.GetAsync(endpoint);
                var jsonResponse1 = await response.Content.ReadAsStringAsync();

                // Log the raw JSON for debugging
                _logger.LogInformation("D365 API Response: {JsonResponse}", jsonResponse1);

                if (response.IsSuccessStatusCode)
                {
                    // Use case-insensitive deserialization
                    var options1 = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var d365Response = JsonSerializer.Deserialize<D365ApiResponse>(jsonResponse1, options1);
                    if (d365Response?.Value == null)
                    {
                        _logger.LogWarning("No menu items found in D365 API response: {JsonResponse}", jsonResponse1);
                        throw new Exception("No menu items found in D365 API response.");
                    }
                    return d365Response.Value;
                }
                else
                {
                    _logger.LogWarning("D365 API request failed with status code: {StatusCode}, Response: {JsonResponse}", response.StatusCode, "");
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize D365 API response: {JsonResponse}", "");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch menu items from D365 API.");
            }

            _logger.LogInformation("Returning dummy menu data.");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var dummyResponse = JsonSerializer.Deserialize<D365ApiResponse>(dummyJson, options);
            return dummyResponse?.Value ?? new List<MenuItem>();
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var tokenEndpoint = "https://login.microsoftonline.com/b445bfd7-8ff7-4f9b-b1f3-f597234bc8fd/oauth2/token";

            // Safely access configuration using GetRequiredSection to avoid null reference warnings
            var d365Config = _configuration.GetRequiredSection("D365");
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



        //public async Task<IEnumerable<Employee>> GetEmployeesAsync()
        //{
        //    var token = await GetAccessTokenAsync();
        //    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    var response = await _httpClient.GetAsync("systemusers?$select=fullname,department,emailaddress1");
        //    response.EnsureSuccessStatusCode();

        //    var content = await response.Content.ReadAsStringAsync();
        //    var json = JsonSerializer.Deserialize<JsonElement>(content);
        //    var employees = new List<Employee>();
        //    foreach (var item in json.GetProperty("value").EnumerateArray())
        //    {
        //        employees.Add(new Employee(
        //            Guid.NewGuid(), // Replace with actual D365 ID (e.g., item.GetProperty("systemuserid").GetGuid())
        //            item.GetProperty("fullname").GetString(),
        //            item.GetProperty("department").GetString(),
        //            item.GetProperty("emailaddress1").GetString()
        //        ));
        //    }
        //    return employees;
        //}

        //public async Task<IEnumerable<LeaveRequest>> GetLeaveRequestsAsync()
        //{
        //    var token = await GetAccessTokenAsync();
        //    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    var response = await _httpClient.GetAsync("hrm_leave?$select=employeeid,startdate,enddate,status");
        //    response.EnsureSuccessStatusCode();

        //    var content = await response.Content.ReadAsStringAsync();
        //    var json = JsonSerializer.Deserialize<JsonElement>(content);
        //    var leaveRequests = new List<LeaveRequest>();
        //    foreach (var item in json.GetProperty("value").EnumerateArray())
        //    {
        //        leaveRequests.Add(new LeaveRequest(
        //            Guid.NewGuid(), // Replace with actual D365 ID
        //            Guid.Parse(item.GetProperty("employeeid").GetString()),
        //            item.GetProperty("startdate").GetDateTime(),
        //            item.GetProperty("enddate").GetDateTime(),
        //            item.GetProperty("status").GetString()
        //        ));
        //    }
        //    return leaveRequests;
        //}

        //private async Task<string> GetAccessTokenAsync()
        //{
        //    var scopes = new[] { "https://your-d365-instance.api.crm.dynamics.com/.default" };
        //    var result = await _clientApp.AcquireTokenForClient(scopes).ExecuteAsync();
        //    return result.AccessToken;
        //}


    }
}
