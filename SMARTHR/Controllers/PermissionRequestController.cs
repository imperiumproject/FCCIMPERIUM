using Microsoft.AspNetCore.Mvc;
using SMARTHR.WEB.FCCHRServices.D365;
using SMARTHR.WEB.FCCHRServices.FCCHRBusinessInterface;
using SMARTHR.WEB.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace SMARTHR.WEB.Controllers
{
	public class PermissionRequestController : Controller
	{
		//private readonly ILeaveService _leaveService;ILeaveService leaveService
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _configuration;
		private readonly ILogger<PermissionRequestController> _logger;
		private readonly IAuthentications _authentications;
		public PermissionRequestController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<PermissionRequestController> logger,
			IAuthentications authentications)
		{
			_httpClientFactory = httpClientFactory;
			_configuration = configuration;
			_logger = logger;
			_authentications = authentications;
		}
		public IActionResult PermissionList()
		{
			//var res=GetPermissionTypes("");
			return View();
		}
		public async Task<List<PermissionTypes?>> GetPermissionTypes(string EmiId)
		{
			string? companyId = HttpContext.Session.GetString("CompanyCode");
			string? userId = HttpContext.Session.GetString("Username");
			try
			{
				using var client = _httpClientFactory.CreateClient();
				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for user {UserId}", userId);
				}
				var apiUrl = $"{_configuration["D365:BaseUrl"]}" + D365ApiURL.PermissionTypesUrl;
				var requestBody = new PermissionContractRequest
				{
					permissionContract = new PermissionContract
					{
						companyCode = companyId
					}
				};
				var jsonOptions = _authentications.GetJsonOptions();
				var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
				_logger.LogInformation("Sending request to {ApiUrl} for user {UserId}: {RequestBody}", apiUrl, userId, jsonString);

				var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
				var response = await client.PostAsync(apiUrl, content);

				if (response.IsSuccessStatusCode)
				{
					var responseContent = await response.Content.ReadAsStringAsync();

					_logger.LogInformation("Raw API response for user {UserId}: {ResponseContent}", userId, responseContent);

					try
					{
						var result = JsonSerializer.Deserialize<PermissionTypeResponse>(responseContent, jsonOptions) ?? new PermissionTypeResponse { EarlyLeaveRequestTypeList = new List<PermissionTypes>() };

						// Log deserialized result for debugging
						_logger.LogInformation("Deserialized LeaveTypes count: {Count}", result.EarlyLeaveRequestTypeList != null ? result.EarlyLeaveRequestTypeList.Count : 0);
						return result.EarlyLeaveRequestTypeList;
					}
					catch (JsonException ex)
					{
						_logger.LogError(ex, "Failed to deserialize API response for user {UserId}: {ResponseContent}", userId, responseContent);
						List<PermissionTypes?> EarlyLeaveRequestTypeList = new List<PermissionTypes?>();
						return EarlyLeaveRequestTypeList;
					}
				}
				else
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					_logger.LogError("API call failed for user {UserId}: {StatusCode}, {ReasonPhrase}, {ErrorContent}",
						userId, response.StatusCode, response.ReasonPhrase, errorContent);
					List<PermissionTypes?> EarlyLeaveRequestTypeList = new List<PermissionTypes?>();
					return EarlyLeaveRequestTypeList;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Profile retrieval failed for user {UserId}", userId ?? "Unknown");
				List<PermissionTypes?> EarlyLeaveRequestTypeList = new List<PermissionTypes?>();
				return EarlyLeaveRequestTypeList;

			}
		}
	}
}
