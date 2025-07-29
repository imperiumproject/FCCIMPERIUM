using Microsoft.AspNetCore.Mvc;
using SmartHR.Application.Interfaces;
using SmartHR.Application.Services;
using SMARTHR.WEB.Models;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace SMARTHR.WEB.Controllers
{
    public class PaySlipsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaySlipsController> _logger;
        private readonly IAuthentications _authentications;

        public PaySlipsController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<PaySlipsController> logger,
         IAuthentications authentications, ILeaveService leaveService)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
            _authentications = authentications;
           
        }
        [HttpGet]
        public IActionResult PaySlips()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaySlips([FromBody] Payslip payslip)
        {
            string result = "";
            string? companyid = HttpContext.Session.GetString("CompanyCode");
            string? userId = HttpContext.Session.GetString("Username");
            try
            {
                using var client = _httpClientFactory.CreateClient();

                var token = await _authentications.GetAccessTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("Failed to acquire access token for company {CompanyId}", companyid);
                result = "Failed to authenticate with the server.";
                    result = "Error";
                    return View();
                }

                var apiUrl = $"{_configuration["D365:BaseUrl"]}/FCC_ESSIntgerationServiceGroup/FCC_ESSPaySlipService/getpayslip";

                var requestBody1 = new LeaveNumIdRequest
                {
                    contract = new ContractVM
                    {
                        companyId = companyid
                    }
                };

                var requestBody = new PaySlipContractVm
                {
                    paySlipContract = new Payslip
                    {
                        CompanyCode = companyid,
                        UserID=userId,
                        Month=payslip.Month,
                        Year=payslip.Year
                    }
                };

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = null,
                    WriteIndented = false
                };

                var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsync(apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    result = string.IsNullOrEmpty(responseContent) ? string.Empty : JsonSerializer.Deserialize<string>(responseContent, jsonOptions) ?? string.Empty;
                }
                else
                {
                    _logger.LogError("API call failed: {StatusCode}, {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                    result = "API call failed.";
                    result = "Error";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while calling GetLeaveId for company {CompanyId}", companyid);
                result = $"Exception: {ex.Message}";
                result = "Error";
            }
            return View();
        }
    }
}
public class Payslip
{
	//[JsonPropertyName("companyCode")]
	public string? CompanyCode { get; set; }

	//[JsonPropertyName("month")]
	public string? Month { get; set; }

	//[JsonPropertyName("userID")]
	public string? UserID { get; set; }

	//[JsonPropertyName("year")]
	public string? Year { get; set; }

}

public class PaySlipContractVm
{
	[JsonPropertyName("paySlipContract")]
	public Payslip? paySlipContract { get; set; }

}