using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SMARTHR.Controllers;
using SMARTHR.WEB.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics.Contracts;
using System.Globalization;
using Microsoft.AspNetCore.Components.Forms;
using System.Text.Json.Serialization;
using Azure;
using System.ComponentModel.Design;
using SMARTHR.WEB.FCCHRServices.FCCHRBusinessInterface;
using SMARTHR.WEB.FCCHRServices.D365;

namespace SMARTHR.WEB.Controllers
{
    public class LeaveController : Controller
	{
		//private readonly ILeaveService _leaveService;ILeaveService leaveService
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _configuration;
		private readonly ILogger<LeaveController> _logger;
		private readonly IAuthentications _authentications;
		public LeaveController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<LeaveController> logger,
			IAuthentications authentications)
		{
			_httpClientFactory = httpClientFactory;
			_configuration = configuration;
			_logger = logger;
			_authentications = authentications;
			//_leaveService = leaveService;
		}

		public async Task<List<LoanItems?>> GetLoanItems()
		{
			string? companyId = HttpContext.Session.GetString("CompanyCode");
			string? userId = HttpContext.Session.GetString("Username");		
			// Check session for Profile data
			var profileJson = HttpContext.Session.GetString("Profile");		   
			try
			{
				// Create HttpClient
				using var client = _httpClientFactory.CreateClient();

				// Step 1: Acquire access token
				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for user {UserId}", userId);					
				}

				// Step 2: Prepare API request
				var apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.ReadLoanedItemsDetailsUrl;
				var requestBody = new LoanInfoContractRequest
				{
					LeaveCreationContract = new userInfoContract
					{
						UserId = userId,
						CompanyCode = companyId,					
					}
				};

				//var jsonOptions = new JsonSerializerOptions
				//{
				//	PropertyNamingPolicy = null,
				//	WriteIndented = true,
				//	PropertyNameCaseInsensitive = true
				//};
				var jsonOptions = _authentications.GetJsonOptions();
				var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
				_logger.LogInformation("Sending request to {ApiUrl} for user {UserId}: {RequestBody}", apiUrl, userId, jsonString);

				var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				// Step 3: Make API call
				var response = await client.PostAsync(apiUrl, content);

				if (response.IsSuccessStatusCode)
				{
					var responseContent = await response.Content.ReadAsStringAsync();
					
					_logger.LogInformation("Raw API response for user {UserId}: {ResponseContent}",userId, responseContent);

					try
					{
						var result = JsonSerializer.Deserialize<LoanItemsResponse>(responseContent, jsonOptions) ?? new LoanItemsResponse { LoanedItemsResult = new List<LoanItems>() };					

						// Log deserialized result for debugging
						_logger.LogInformation("Deserialized LeaveTypes count: {Count}", result.LoanedItemsResult!=null? result.LoanedItemsResult.Count:0);						
						return result.LoanedItemsResult;
					}
					catch (JsonException ex)
					{
						_logger.LogError(ex, "Failed to deserialize API response for user {UserId}: {ResponseContent}", userId, responseContent);
						List<LoanItems?> LoanedItemsResult = new List<LoanItems?>();
						return LoanedItemsResult;
					}
				}
				else
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					_logger.LogError("API call failed for user {UserId}: {StatusCode}, {ReasonPhrase}, {ErrorContent}",
						userId, response.StatusCode, response.ReasonPhrase, errorContent);
					List<LoanItems?> LoanedItemsResult = new List<LoanItems?>();
					return LoanedItemsResult; 
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Profile retrieval failed for user {UserId}", userId ?? "Unknown");
				List<LoanItems?> LoanedItemsResult = new List<LoanItems?>();
				return LoanedItemsResult;
				
			}
		}



		//Submite
		public async Task<IActionResult> SubmitRequestWorkFlow()
		{
			var leave = new WorkflowTrackingList();
			string? companyId = HttpContext.Session.GetString("CompanyCode");
			string? userId = HttpContext.Session.GetString("Username");		
			_logger.LogInformation("TrackRequestWorkFlow called with RecordId: {RecordId}, TableId: {TableId}, CompanyId: {CompanyId}", 0, 0, companyId);

			try
			{
				using var client = _httpClientFactory.CreateClient();
				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for company {CompanyId}", companyId);
					return BadRequest(new { error = "Failed to acquire access token" });
				}

				var apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.SubmitWorkflowUrl;

				var requestBody = new WorkFlowSubmitResponse
				{
					WfContract = new WorkFlowSubmitRequest
					{
						CompanyId = companyId,
						WorkflowTemplateName = "Leave Request",
						TableId = 66492,
						RecordId = 5637157326,
						Comments= "TestSubmit",
						RaisedUser= "001038",
						SubmittingUser= "001038"
					}
				};

				//var jsonOptions = new JsonSerializerOptions
				//{
				//	PropertyNamingPolicy = null,
				//	PropertyNameCaseInsensitive = true,
				//	NumberHandling = JsonNumberHandling.AllowReadingFromString
				//};
				var jsonOptions = _authentications.GetJsonOptions();
				var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
				var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

				_logger.LogInformation("Sending API request to {ApiUrl} with body: {RequestBody}", apiUrl, jsonString);

				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await client.PostAsync(apiUrl, content);
				var responseContent = await response.Content.ReadAsStringAsync();

				_logger.LogInformation("API Response: StatusCode: {StatusCode}, Content: {ResponseContent}", response.StatusCode, responseContent);

				if (response.IsSuccessStatusCode)
				{
					try
					{
						var wkFlowResponse = JsonSerializer.Deserialize<WorkflowTrackingList>(responseContent, jsonOptions);
						leave = wkFlowResponse ?? new WorkflowTrackingList();
						_logger.LogInformation("Deserialized wfTrackingList with {Count} entries", "");
					}
					catch (JsonException ex)
					{
						_logger.LogError(ex, "Failed to deserialize JSON response for company {CompanyId}. Error at path: {Path}, Message: {Message}",
							companyId, ex.Path, ex.Message);
						return BadRequest(new { error = "Failed to deserialize API response" });
					}
				}
				else
				{
					_logger.LogError("API call failed: {StatusCode}, {ReasonPhrase}, Content: {ResponseContent}", response.StatusCode, response.ReasonPhrase, responseContent);
					return BadRequest(new { error = $"API call failed: {response.ReasonPhrase}" });
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception while getting Leave Details for CompanyId: {CompanyId}, RecordId: {RecordId}", companyId, 0);
				return BadRequest(new { error = "An unexpected error occurred" });
			}

			return Ok(leave);
		}

		//Work flow history
		[HttpGet]
		public async Task<IActionResult> TrackRequestWorkFlow(long RecordId, int TableId,string TransType)
		{
			var leave = new WorkflowTrackingList();
			string? companyId = HttpContext.Session.GetString("CompanyCode");
			string? userId = HttpContext.Session.GetString("Username");
			
			_logger.LogInformation("TrackRequestWorkFlow called with RecordId: {RecordId}, TableId: {TableId}, CompanyId: {CompanyId}", RecordId, TableId, companyId);

			try
			{
				using var client = _httpClientFactory.CreateClient();
				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for company {CompanyId}", companyId);
					return BadRequest(new { error = "Failed to acquire access token" });
				}

				var apiUrl = $"{_configuration["D365:BaseUrl"]}"+ D365ApiURL.ReadWFTrackingDetailsUrl;

				var requestBody = new WorkFlowHistoryTrackingResponse
				{
					WfContract = new WorkFlowHistoryTracking
					{
						CompanyId = companyId,
						TransType = TransType,
						TableId = TableId,
						RecordId = RecordId
					}
				};

				//var jsonOptions = new JsonSerializerOptions
				//{
				//	PropertyNamingPolicy = null,
				//	PropertyNameCaseInsensitive = true,
				//	NumberHandling = JsonNumberHandling.AllowReadingFromString
				//};
				var jsonOptions = _authentications.GetJsonOptions();
				var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
				var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

				_logger.LogInformation("Sending API request to {ApiUrl} with body: {RequestBody}", apiUrl, jsonString);

				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await client.PostAsync(apiUrl, content);
				var responseContent = await response.Content.ReadAsStringAsync();

				_logger.LogInformation("API Response: StatusCode: {StatusCode}, Content: {ResponseContent}", response.StatusCode, responseContent);

				if (response.IsSuccessStatusCode)
				{
					try
					{
						var wkFlowResponse = JsonSerializer.Deserialize<WorkflowTrackingList>(responseContent, jsonOptions);
						leave = wkFlowResponse ?? new WorkflowTrackingList();
						_logger.LogInformation("Deserialized wfTrackingList with {Count} entries","");
					}
					catch (JsonException ex)
					{
						_logger.LogError(ex, "Failed to deserialize JSON response for company {CompanyId}. Error at path: {Path}, Message: {Message}",
							companyId, ex.Path, ex.Message);
						return BadRequest(new { error = "Failed to deserialize API response" });
					}
				}
				else
				{
					_logger.LogError("API call failed: {StatusCode}, {ReasonPhrase}, Content: {ResponseContent}", response.StatusCode, response.ReasonPhrase, responseContent);
					return BadRequest(new { error = $"API call failed: {response.ReasonPhrase}" });
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception while getting Leave Details for CompanyId: {CompanyId}, RecordId: {RecordId}", companyId, RecordId);
				return BadRequest(new { error = "An unexpected error occurred" });
			}

			return Ok(leave);
		}
		public async Task<IActionResult> LeaveRequest()
        {

			var loanItems =await GetLoanItems();
			ViewBag.LoanItems = loanItems;
			//var r =await SubmitRequestWorkFlow();
			string? username = HttpContext.Session.GetString("Username");
			ViewBag.Username = username;
			var response = await ReadLeaveInformation();
			var model = response?.LeaveDetails ?? new List<SMARTHR.WEB.Models.LeaveDetails>();
			return View(model);
        }
		private async Task<ReadleaveInfoRespone> ReadLeaveInformation()
		{
			var leave = new ReadleaveInfoRespone();
			string? companyid = HttpContext.Session.GetString("CompanyCode");
			string? userId = HttpContext.Session.GetString("Username");

			try
			{
				using var client = _httpClientFactory.CreateClient();
				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for company {CompanyId}", companyid);
					return leave;
				}

				var apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.ReadLeaveInfoUrl;

				var requestBody = new ReadLeaveRequestInfo
				{
					userLeaveContract = new userLeaveContract
					{
						companyCode = companyid,
						UserId = userId
					}
				};

				//var jsonOptions = new JsonSerializerOptions
				//{
				//	PropertyNamingPolicy = null,
				//	PropertyNameCaseInsensitive = true,
				//	NumberHandling = JsonNumberHandling.AllowReadingFromString
				//};
				var jsonOptions = _authentications.GetJsonOptions();
				var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
				var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await client.PostAsync(apiUrl, content);
				var responseContent = await response.Content.ReadAsStringAsync();

				_logger.LogInformation("Employee API Raw JSON: {Json}", responseContent);

				if (response.IsSuccessStatusCode)
				{
					try
					{
						// Log the value of noOfDays for leaveDetails[0]
						using var document = JsonDocument.Parse(responseContent);
						var leaveDetails = document.RootElement.GetProperty("leaveDetails");
						if (leaveDetails.GetArrayLength() > 0)
						{
							var firstLeave = leaveDetails[0];
							var noOfDaysValue = firstLeave.GetProperty("noOfDays").ToString();
							_logger.LogInformation("leaveDetails[0].noOfDays: {Value} (TokenType: {TokenType})",
								noOfDaysValue, firstLeave.GetProperty("noOfDays").ValueKind);
						}

						var employeeResponse = JsonSerializer.Deserialize<ReadleaveInfoRespone>(responseContent, jsonOptions);
						leave = employeeResponse ?? new ReadleaveInfoRespone();
					}
					catch (JsonException ex)
					{
						_logger.LogError(ex, "Failed to deserialize JSON response for company {CompanyId}. Error at path: {Path}, Message: {Message}",
							companyid, ex.Path, ex.Message);
						return new ReadleaveInfoRespone();
					}
				}
				else
				{
					_logger.LogError("API call failed: {StatusCode}, {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception while getting Leave Details {CompanyId}", companyid);
			}

			return leave;
		}
		public async Task<IActionResult> LeaveResumption()
        {
			string? username = HttpContext.Session.GetString("Username");
			ViewBag.Username = username;
			var res = await ReadLeaveResumptionInformation();


			var model = res?.leaveResumptionDetails ?? new List<SMARTHR.WEB.Models.leaveResumptionDetails>();
			return View(model);
			
        }
				
        public IActionResult LeaveEncashment() 
        {
			string? username = HttpContext.Session.GetString("Username");
			ViewBag.Username = username;
			//var res = await UpdateLeaveRequest();
			return View();
        }
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> GetLeaveType(string EmpId)
		{
			var model = new LeaveData();

			// Check session for Profile data
			var profileJson = HttpContext.Session.GetString("Profile");
			ProfileViewModel? profile = null;

			if (string.IsNullOrEmpty(profileJson))
			{
				_logger.LogWarning("Session expired or Profile data missing.");
				return Json(new { success = false, message = "Session expired. Please log in again." });
			}

			try
			{
				profile = JsonSerializer.Deserialize<ProfileViewModel>(profileJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
				if (profile == null || string.IsNullOrEmpty(profile.UserId))
				{
					_logger.LogWarning("Invalid or missing UserId in session Profile data.");
					return Json(new { success = false, message = "Invalid session data. Please log in again." });
				}
			}
			catch (JsonException ex)
			{
				_logger.LogError(ex, "Failed to deserialize session Profile data.");
				return Json(new { success = false, message = "Error processing session data." });
			}

			try
			{
				// Create HttpClient
				using var client = _httpClientFactory.CreateClient();

				// Step 1: Acquire access token
				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for user {UserId}", profile.UserId);
					return Json(new { success = false, message = "Failed to authenticate with the server." });
				}

				// Step 2: Prepare API request
				var apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.GetLeaveTypesUrl;
				var requestBody = new EssUserLeaveTypeViewModel
				{
					UserleaveTypes = new UserLeaveTypeViewModel
					{
						UserId = EmpId?? profile.UserId,
						companyCode = profile.CompanyId,
						Nationality = "",
						Religioncheck = "No"
					}
				};

				//var jsonOptions = new JsonSerializerOptions
				//{
				//	PropertyNamingPolicy = null,
				//	WriteIndented = true,
				//	PropertyNameCaseInsensitive = true
				//};
				var jsonOptions = _authentications.GetJsonOptions();
				var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
				_logger.LogInformation("Sending request to {ApiUrl} for user {UserId}: {RequestBody}", apiUrl, profile.UserId, jsonString);

				var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				// Step 3: Make API call
				var response = await client.PostAsync(apiUrl, content);

				if (response.IsSuccessStatusCode)
				{
					var responseContent = await response.Content.ReadAsStringAsync();
					if (string.IsNullOrEmpty(responseContent))
					{
						_logger.LogError("Empty response from API for user {UserId}", profile.UserId);
						return Json(new { success = false, message = "Empty response from server." });
					}

					_logger.LogInformation("Raw API response for user {UserId}: {ResponseContent}", profile.UserId, responseContent);

					try
					{
						var result = JsonSerializer.Deserialize<LeaveData>(responseContent, jsonOptions) ?? new LeaveData { LeaveTypes = new List<DDLLeaveType>() };
						if (result.LeaveTypes == null || !result.LeaveTypes.Any())
						{
							_logger.LogError("Deserialized LeaveData or LeaveTypes is null/empty for user {UserId}", profile.UserId);
							return Json(new { success = false, message = "No leave types available." });
						}

						// Log deserialized result for debugging
						_logger.LogInformation("Deserialized LeaveTypes count: {Count}", result.LeaveTypes.Count);
						foreach (var item in result.LeaveTypes)
						{
							_logger.LogInformation("LeaveType: Id={Id}, Type={Type}, BalanceKind={BalanceKind}, Balance={Balance}",
								item.Id, item.Type, item.Balance.ValueKind, item.Balance);
						}

						return Json(new { success = true, data = result });
					}
					catch (JsonException ex)
					{
						_logger.LogError(ex, "Failed to deserialize API response for user {UserId}: {ResponseContent}", profile.UserId, responseContent);
						return Json(new { success = false, message = "Invalid response format from server." });
					}
				}
				else
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					_logger.LogError("API call failed for user {UserId}: {StatusCode}, {ReasonPhrase}, {ErrorContent}",
						profile.UserId, response.StatusCode, response.ReasonPhrase, errorContent);
					return Json(new { success = false, message = $"Authentication failed: {response.ReasonPhrase}" });
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Profile retrieval failed for user {UserId}", profile?.UserId ?? "Unknown");
				return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
			}
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CancelleaveRequest()
		{
			var model = new MyProfileViewModel();
			var profileJson = HttpContext.Session.GetString("Profile");

			if (string.IsNullOrEmpty(profileJson))
			{
				_logger.LogWarning("Session expired or Profile data missing.");
				return Unauthorized(new { success = false, message = "Session expired. Please log in." });
			}

			try
			{
				var profile = JsonSerializer.Deserialize<ProfileViewModel>(profileJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
				if (profile == null || string.IsNullOrEmpty(profile.UserId))
				{
					_logger.LogWarning("Invalid or missing UserId in session Profile data.");
					return Unauthorized(new { success = false, message = "Invalid session data. Please log in." });
				}

				using var client = _httpClientFactory.CreateClient();
				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for user {UserId}", profile.UserId);
					return StatusCode(500, new { success = false, message = "Failed to authenticate with the server." });
				}

				var apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.RequestCancelledUrl;
				var requestBody = new EssCancelLeaveRequestViewModel
				{
					LeaveContract = new CancelLeaveRequestViewModel
					{
						leaveNum = "LV-000000023", 
						CompanyCode = profile.CompanyId ?? "USMF"
					}
				};

				var jsonString = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions { PropertyNamingPolicy = null, WriteIndented = true });
				
				_logger.LogInformation("Sending request to {ApiUrl} for user {UserId}: {RequestBody}", apiUrl, profile.UserId, jsonString);

				var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await client.PostAsync(apiUrl, content);

				if (response.IsSuccessStatusCode)
				{
					var responseContent = await response.Content.ReadAsStringAsync();
					if (string.IsNullOrEmpty(responseContent))
					{
						_logger.LogError("Empty response from API for user {UserId}", profile.UserId);
						return StatusCode(500, new { success = false, message = "Empty response from server." });
					}

					_logger.LogInformation("API response for user {UserId}: {Response}", profile.UserId, responseContent);
					try
					{
						var result = JsonSerializer.Deserialize<string>(responseContent, new JsonSerializerOptions());
						return Ok(new { success = true, message = result ?? "Leave request cancelled successfully" });
					}
					catch (JsonException ex)
					{
						_logger.LogError(ex, "Failed to deserialize API response for user {UserId}: {ResponseContent}", profile.UserId, responseContent);
						return StatusCode(500, new { success = false, message = "Invalid response format from server." });
					}
				}
				else
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					_logger.LogError("API call failed for user {UserId}: {StatusCode}, {ReasonPhrase}, {ErrorContent}",
						profile.UserId, response.StatusCode, response.ReasonPhrase, errorContent);
					return StatusCode((int)response.StatusCode, new { success = false, message = $"API call failed: {response.ReasonPhrase}" });
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Profile retrieval failed for user {UserId}","" ?? "Unknown");
				return StatusCode(500, new { success = false, message = $"An error occurred: {ex.Message}" });
			}
		}

		public async Task<string> GetLeaveId()
		{
			string result;
			string? companyid = HttpContext.Session.GetString("CompanyCode");
			try
			{
				using var client = _httpClientFactory.CreateClient();

				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for company {CompanyId}", companyid);
					result = "Failed to authenticate with the server.";
					result = "Error";
					return result;
				}

				var apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.GetLeaveNumIdUrl;

				var requestBody = new LeaveNumIdRequest
				{
					contract = new ContractVM
					{
						companyId = companyid
					}
				};

				//var jsonOptions = new JsonSerializerOptions
				//{
				//	PropertyNamingPolicy = null,
				//	WriteIndented = false
				//};
				var jsonOptions = _authentications.GetJsonOptions();
				var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
				var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await client.PostAsync(apiUrl, content);
				var responseContent = await response.Content.ReadAsStringAsync();

				if (response.IsSuccessStatusCode)
				{
					result = string.IsNullOrEmpty(responseContent)? string.Empty : JsonSerializer.Deserialize<string>(responseContent, jsonOptions) ?? string.Empty;
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

			return result;
		}
	
		public async Task<ActionResult<NoOfDaysVM>> GetNoOfDays([FromBody] GetNoofDays param)
		{
			try
			{
				string? userid = HttpContext.Session.GetString("Username");
				string? companycode = HttpContext.Session.GetString("CompanyCode");
				if (string.IsNullOrEmpty(userid) || string.IsNullOrEmpty(companycode))
				{
					_logger.LogError("User ID or Company Code is missing in session.");
					return BadRequest("Error: Invalid session data.");
				}

				using var client = _httpClientFactory.CreateClient();
				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for user {UserId}", userid);
					return BadRequest("Error: Failed to authenticate with the server.");
				}

				var apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.GetNoOfDaysUrl;				
				var requestBody = new leaveContractRequest
				{
					leaveContract = new leaveContract
					{
						CompanyCode = companycode,
						LeaveStart = _authentications.TryParseDate(param.leavestart??"", out string startDate) ? startDate : "",
						LeaveEnd = _authentications.TryParseDate(param.leaveend ?? "", out string endDate) ? endDate : "",
						UserId = param.EmpId?? userid,
						LeaveType = param.leavetype
					}
				};

				//var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = null, WriteIndented = false };
				var jsonOptions = _authentications.GetJsonOptions();
				var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
				var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await client.PostAsync(apiUrl, content);
				var responseContent = await response.Content.ReadAsStringAsync();

				if (response.IsSuccessStatusCode)
				{
					var result = JsonSerializer.Deserialize<NoOfDaysVM>(responseContent, jsonOptions);
					if (result == null)
					{
						_logger.LogError("Failed to deserialize response.");
						return BadRequest("Error: Invalid response.");
					}
					return Ok(result); // Return the NoOfDaysVM object
				}
				_logger.LogError("API call failed: {StatusCode}, {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
				return StatusCode((int)response.StatusCode, "Error: API call failed.");				
			}
			catch (FormatException ex)
			{
				_logger.LogError(ex, "Invalid date format for leavestart: {LeaveStart} or leaveend: {LeaveEnd}", param.leavestart, param.leaveend);
				return BadRequest("Error: Invalid date format. Please use MM/dd/yyyy.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception while calling GetNoOfDays for user {UserId}", HttpContext.Session.GetString("Username"));
				return BadRequest($"Error: {ex.Message}");
			}
		}
		
		[HttpPost]
		public async Task<decimal> GetLeaveBalance([FromBody] LeaveTypeModel model)
		{
			decimal res = 0;
			string companycode = HttpContext.Session.GetString("CompanyCode") ?? string.Empty;
			string? userid = HttpContext.Session.GetString("Username");

			try
			{
				using var client = _httpClientFactory.CreateClient();

				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for user {UserId}", userid);
					return res; // or throw new Exception("Token failure");
				}

				var apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.GetLeaveBalanceUrl;

				var requestBody = new leaveBalanceRequest
				{
					leaveBalanceContract = new leaveBalanceContract
					{
						CompanyCode = companycode,
						UserId = model.EmpId?? userid,
						LeaveType = model.LeaveType ?? "Causal leave"
					}
				};

				//var jsonOptions = new JsonSerializerOptions
				//{
				//	PropertyNamingPolicy = null,
				//	WriteIndented = false
				//};
				var jsonOptions = _authentications.GetJsonOptions();

				var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
				var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await client.PostAsync(apiUrl, content);
				var responseContent = await response.Content.ReadAsStringAsync();

				if (response.IsSuccessStatusCode)
				{
					res = JsonSerializer.Deserialize<decimal>(responseContent, jsonOptions);
				}
				else
				{
					_logger.LogError("API call failed: {StatusCode}, {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception while calling GetLeaveBalance for user {UserId}", userid);
			}

			return res;
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<EmployeeListResponse> GetAllEmployees()
		{
			var employees = new EmployeeListResponse();
			string? companyid = HttpContext.Session.GetString("CompanyCode");

			try
			{
				using var client = _httpClientFactory.CreateClient();
				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for company {CompanyId}", companyid);
					return employees;
				}

				var apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.GetAllEmployeesUrl;

				var requestBody = new LeaveNumIdRequest
				{
					contract = new ContractVM
					{
						companyId = companyid
					}
				};

				//var jsonOptions = new JsonSerializerOptions
				//{
				//	PropertyNamingPolicy = null,
				//	PropertyNameCaseInsensitive = true
				//};
				var jsonOptions = _authentications.GetJsonOptions();

				var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
				var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await client.PostAsync(apiUrl, content);
				var responseContent = await response.Content.ReadAsStringAsync();

				_logger.LogInformation("Employee API Raw JSON: {Json}", responseContent);

				if (response.IsSuccessStatusCode)
				{
					var employeeResponse = JsonSerializer.Deserialize<EmployeeListResponse>(responseContent, jsonOptions);
					employees = employeeResponse ?? new EmployeeListResponse();
				}
				else
				{
					_logger.LogError("API call failed: {StatusCode}, {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception while getting all employees for company {CompanyId}", companyid);
			}

			return employees;
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("GetBackupEmpName")]
		public async Task<IActionResult> GetBackupEmpName(string EmpId)
		{
			string? companyid = HttpContext.Session.GetString("CompanyCode");
			string? userid = HttpContext.Session.GetString("Username");

			try
			{
				using var client = _httpClientFactory.CreateClient();

				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for company {CompanyId}", companyid);
					return Json(new { message = "Failed to acquire access token." });
				}

				var apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.GetBackupEmplNamesUrl;

				var requestBody = new userInfoContractRequest
				{
					userInfoContract = new userInfoContract
					{
						CompanyCode = companyid,
						UserId = EmpId?? userid
					}
				};

				//var jsonOptions = new JsonSerializerOptions
				//{
				//	PropertyNamingPolicy = null,
				//	PropertyNameCaseInsensitive = true
				//};
				var jsonOptions = _authentications.GetJsonOptions();
				var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
				var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await client.PostAsync(apiUrl, content);
				var responseContent = await response.Content.ReadAsStringAsync();

				_logger.LogInformation("Response: {response}", responseContent); // Debug the raw JSON

				if (response.IsSuccessStatusCode)
				{
					var resultObj = JsonSerializer.Deserialize<BackupEmpResponse>(responseContent, jsonOptions);
					if (resultObj != null && resultObj.emplId != null)
					{
						return Json(resultObj);
					}
					else
					{
						_logger.LogWarning("No employee data found in response for company {CompanyId}", companyid);
						return Json(new { message = "No employee data available." });
					}
				}
				else
				{
					_logger.LogError("API call failed: {StatusCode}, {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
					return Json(new { message = "Failed to load employee data from API." });
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception while getting backup employees for {CompanyId}", companyid);
				return Json(new { message = "An error occurred while loading employee data." });
			}
		}

		//    [HttpPost]
		//    [ValidateAntiForgeryToken]
		//    public async Task<LeaveResponse?> CreateLeaveRequest([FromBody] CreateLeaveRequest input)
		//    {
		//        string? companyId = HttpContext.Session.GetString("CompanyCode");
		//        string? userId = HttpContext.Session.GetString("Username");

		//        // Validate session values
		//        if (string.IsNullOrEmpty(companyId) || string.IsNullOrEmpty(userId))
		//        {
		//            _logger.LogError("Missing CompanyCode or Username in session for user {UserId}", userId);
		//            return new LeaveResponse { Message = "Session data is missing." };
		//        }
		//        try
		//        {
		//            // Validate input
		//            if (input == null || string.IsNullOrEmpty(input.LeaveType) || input.NoOfDays == null)
		//            {
		//                _logger.LogError("Invalid input data for leave request by user {UserId}", userId);
		//                return new LeaveResponse { Message = "Invalid input data." };
		//            }

		//            // Parse dates
		//            if (!_authentications.TryParseDate(input.LeaveStart, out string startDate))
		//            {
		//                _logger.LogError("Invalid date format for LeaveStart: {LeaveStart}", input.LeaveStart);
		//                return new LeaveResponse { Message = $"Invalid date format for LeaveStart: {input.LeaveStart}" };
		//            }

		//            if (!_authentications.TryParseDate(input.LeaveEnd, out string endDate))
		//            {
		//                _logger.LogError("Invalid date format for LeaveEnd: {LeaveEnd}", input.LeaveEnd);
		//                return new LeaveResponse { Message = $"Invalid date format for LeaveEnd: {input.LeaveEnd}" };
		//            }

		//            using var client = _httpClientFactory.CreateClient();

		//            var token = await _authentications.GetAccessTokenAsync();
		//            if (string.IsNullOrEmpty(token))
		//            {
		//                _logger.LogError("Failed to acquire access token for company {CompanyId}", companyId);
		//                return new LeaveResponse { Message = "Failed to authenticate." };
		//            }
		//            string apiUrl;

		//            apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.CreateLeaveRequestUrl;
		////// Convert files to base64
		//var attachList = new List<attachRefList>();
		//if (input.attachRefList != null)
		//{
		//	foreach (var file in input.attachRefList)
		//	{
		//		using var memoryStream = new MemoryStream();
		//		//await file.(memoryStream);
		//		var base64 = Convert.ToBase64String(memoryStream.ToArray());

		//		attachList.Add(new attachRefList
		//		{
		//			fileName = file.fileName,
		//			attachRef = base64
		//		});
		//	}
		//}

		//var requestBody = new CreateleaveContractRequest
		//            {
		//                LeaveContract = new CreateLeaveRequest
		//                {
		//                    CompanyCode = companyId,
		//                    RaisedBy = userId,
		//		UserId = input.UserId,
		//                    LeaveNum = input.LeaveNum,
		//                    LeaveType = input.LeaveType,
		//                    LeaveStart = Convert.ToString(startDate),
		//                    LeaveEnd = Convert.ToString(endDate),
		//                    NoOfDays = input.NoOfDays.Value,
		//                    PayLeaveWithSalary = input.PayLeaveWithSalary ?? true,
		//                    Comments = input.Comments,
		//                    backupEmployee = input.backupEmployee,
		//		Duedate = Convert.ToString(input.Duedate),
		//                }
		//            };

		////var jsonOptions = new JsonSerializerOptions
		////{
		////    PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Match API's expected casing
		////    PropertyNameCaseInsensitive = true
		////};
		//var jsonOptions = _authentications.GetJsonOptions();
		//var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
		//            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

		//            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		//            var response = await client.PostAsync(apiUrl, content);
		//            var responseContent = await response.Content.ReadAsStringAsync();

		//            _logger.LogInformation("CreateLeaveRequest response: {StatusCode}", response.StatusCode);

		//            if (response.IsSuccessStatusCode)
		//            {
		//                var resultObj = JsonSerializer.Deserialize<LeaveResponse>(responseContent, jsonOptions);
		//                return resultObj;
		//            }
		//            else
		//            {
		//                _logger.LogError("API call failed: {StatusCode} - {ReasonPhrase} - {ResponseContent}",
		//                    response.StatusCode, response.ReasonPhrase, responseContent);
		//                return new LeaveResponse { Message = $"API call failed: {response.ReasonPhrase}" };
		//            }
		//        }
		//        catch (Exception ex)
		//        {
		//            _logger.LogError(ex, "Exception while creating leave request for user {UserId}", userId);
		//            return new LeaveResponse { Message = "An error occurred while processing the request." };
		//        }
		//    }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<LeaveResponse?> CreateLeaveRequest([FromBody] CreateleaveContractRequest input)
		{
			string? companyId = HttpContext.Session.GetString("CompanyCode");
			string? userId = HttpContext.Session.GetString("Username");

			// Validate session values
			if (string.IsNullOrEmpty(companyId) || string.IsNullOrEmpty(userId))
			{
				_logger.LogError("Missing CompanyCode or Username in session for user {UserId}", userId);
				return new LeaveResponse { Message = "Session data is missing." };
			}

			try
			{
				// Validate input
				var leaveRequest = input.LeaveContract;
				if (leaveRequest == null || string.IsNullOrEmpty(leaveRequest.LeaveType) || leaveRequest.NoOfDays == null)
				{
					_logger.LogError("Invalid input data for leave request by user {UserId}", userId);
					return new LeaveResponse { Message = "Invalid input data." };
				}

				// Parse dates
				if (!_authentications.TryParseDate(leaveRequest.LeaveStart, out string startDate))
				{
					_logger.LogError("Invalid date format for LeaveStart: {LeaveStart}", leaveRequest.LeaveStart);
					return new LeaveResponse { Message = $"Invalid date format for LeaveStart: {leaveRequest.LeaveStart}" };
				}

				if (!_authentications.TryParseDate(leaveRequest.LeaveEnd, out string endDate))
				{
					_logger.LogError("Invalid date format for LeaveEnd: {LeaveEnd}", leaveRequest.LeaveEnd);
					return new LeaveResponse { Message = $"Invalid date format for LeaveEnd: {leaveRequest.LeaveEnd}" };
				}

				using var client = _httpClientFactory.CreateClient();

				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for company {CompanyId}", companyId);
					return new LeaveResponse { Message = "Failed to authenticate." };
				}

				string apiUrl = $"{_configuration["D365:BaseUrl"]}{D365ApiURL.CreateLeaveRequestUrl}";

				// Use attachRefList directly from input
				var attachList = leaveRequest.attachRefList ?? new List<attachRefList>();

				var requestBody = new CreateleaveContractRequest
				{
					LeaveContract = new CreateLeaveRequest
					{
						CompanyCode = companyId, // From session
						RaisedBy = userId, // From session
						UserId = leaveRequest.UserId,
						LeaveNum = leaveRequest.LeaveNum,
						LeaveType = leaveRequest.LeaveType,
						LeaveStart = Convert.ToString(startDate),
						LeaveEnd = Convert.ToString(endDate),
						NoOfDays = leaveRequest.NoOfDays,
						PayLeaveWithSalary = leaveRequest.PayLeaveWithSalary ?? true,
						Comments = leaveRequest.Comments,
						backupEmployee = leaveRequest.backupEmployee,
						Duedate = Convert.ToString(leaveRequest.Duedate),
						attachRefList = attachList // Use the input attachRefList directly
					}
				};

				var jsonOptions = _authentications.GetJsonOptions();
				var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
				var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await client.PostAsync(apiUrl, content);
				var responseContent = await response.Content.ReadAsStringAsync();

				_logger.LogInformation("CreateLeaveRequest response: {StatusCode}", response.StatusCode);

				if (response.IsSuccessStatusCode)
				{
					var resultObj = JsonSerializer.Deserialize<LeaveResponse>(responseContent, jsonOptions);
					return resultObj;
				}
				else
				{
					_logger.LogError("API call failed: {StatusCode} - {ReasonPhrase} - {ResponseContent}",
						response.StatusCode, response.ReasonPhrase, responseContent);
					return new LeaveResponse { Message = $"API call failed: {response.ReasonPhrase}" };
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception while creating leave request for user {UserId}", userId);
				return new LeaveResponse { Message = "An error occurred while processing the request." };
			}
		}
		public async Task<ReadleaveResponeById> ReadLeaveRequest(string leaveId)
		{
			var leave = new ReadleaveResponeById();
			string companycode = HttpContext.Session.GetString("CompanyCode") ?? string.Empty;
			string? userid = HttpContext.Session.GetString("Username");
			string? ResquestType=string.Empty;
			string[] strId= leaveId.Split(',');
			
			if (strId[0].Length > 0)
			{
				leaveId= strId[0];
			}
			if(strId.Length > 1)
			{
				if (strId[1].Length > 0)
				{
					userid = strId[1];
				}
			}
			if(strId.Length > 2)
			{
				if (strId[2].Length > 0)
				{
					ResquestType = strId[2];
				}
			}
			
			try
			{
				using var client = _httpClientFactory.CreateClient();

				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for user {UserId}", userid);
					//return res; // or throw new Exception("Token failure");
				}
				var apiUrl="";
				ESSReadLeaveRequest requestBody=new ESSReadLeaveRequest();
				ReadBusinessTripRequest requestBody1 = new ReadBusinessTripRequest();
				if (ResquestType.Trim() == ("Business Trip").Trim())
				{
					apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.ReadBusinessTripRequestUrl;
					requestBody1 = new ReadBusinessTripRequest
					{
						readBusinessTripAPIContract = new readBusinessTripAPIContract
						{
							companyId = companycode,
							BusinessTripReqId = leaveId
						}
					};
					//var jsonOptions = new JsonSerializerOptions
					//{
					//	PropertyNamingPolicy = null,
					//	WriteIndented = false
					//};
					var jsonOptions = _authentications.GetJsonOptions();
					var jsonString = JsonSerializer.Serialize(requestBody1, jsonOptions);
					var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

					var response = await client.PostAsync(apiUrl, content);
					var responseContent = await response.Content.ReadAsStringAsync();
					//if (response.IsSuccessStatusCode)
					//{
					//	try
					//	{
					//		// Log the value of noOfDays for leaveDetails[0]
					//		using var document = JsonDocument.Parse(responseContent);
					//		//var leaveDetails = document.RootElement.GetProperty("leaveDetails");
					//		//if (leaveDetails.GetArrayLength() > 0)
					//		//{
					//		//	var firstLeave = leaveDetails[0];
					//		//	var noOfDaysValue = firstLeave.GetProperty("noOfDays").ToString();
					//		//	_logger.LogInformation("leaveDetails[0].noOfDays: {Value} (TokenType: {TokenType})",
					//		//		noOfDaysValue, firstLeave.GetProperty("noOfDays").ValueKind);
					//		//}

					//		var businessTripDetails = JsonSerializer.Deserialize<List<BusinessTripDeatils>>(responseContent, jsonOptions);
					//		if (businessTripDetails == null || !businessTripDetails.Any())
					//		{
					//			_logger.LogWarning("No business trip details returned for BusinessTripReqId: {BusinessTripReqId}", leaveId);
					//			return Ok(new { businessTripDetails = new List<BusinessTripDeatils>() });
					//		}
					//		return Ok(new { businessTripDetails });
					//		//if (leave.LeaveDetails != null)
					//		//{

					//		//	var attachmentsJson = JsonSerializer.Serialize(leave.LeaveDetails[0].AttachRefList);
					//		//	HttpContext.Session.SetString("attachments", attachmentsJson);
					//		//	var json = HttpContext.Session.GetString("attachments");
					//		//	List<AttachRefItem>? attachments = new List<AttachRefItem>();

					//		//	if (!string.IsNullOrEmpty(json))
					//		//	{
					//		//		ViewBag.attachemnts = JsonSerializer.Deserialize<List<AttachRefItem>>(json);
					//		//	}
					//		//}
					//	}
					//	catch (JsonException ex)
					//	{
					//		_logger.LogError(ex, "Failed to deserialize JSON response for company {CompanyId}. Error at path: {Path}, Message: {Message}",
					//			companycode, ex.Path, ex.Message);
					//		return new ReadleaveResponeById();
					//	}
					//}
					//else
					//{
					//	_logger.LogError("API call failed: {StatusCode}, {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
					//}
				
				}
				else
				{
					apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.ReadLeaveRequestUrl;
					requestBody = new ESSReadLeaveRequest
					{
						ReadLeaveRequest = new ReadLeaveRequestVm
						{
							companyCode = companycode,
							UserId = userid,
							LeaveId = leaveId
						}
					};
					//var jsonOptions = new JsonSerializerOptions
					//{
					//	PropertyNamingPolicy = null,
					//	WriteIndented = false
					//};
					var jsonOptions = _authentications.GetJsonOptions();
					var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
					var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

					var response = await client.PostAsync(apiUrl, content);
					var responseContent = await response.Content.ReadAsStringAsync();
					if (response.IsSuccessStatusCode)
					{
						try
						{
							// Log the value of noOfDays for leaveDetails[0]
							using var document = JsonDocument.Parse(responseContent);
							var leaveDetails = document.RootElement.GetProperty("leaveDetails");
							if (leaveDetails.GetArrayLength() > 0)
							{
								var firstLeave = leaveDetails[0];
								var noOfDaysValue = firstLeave.GetProperty("noOfDays").ToString();
								_logger.LogInformation("leaveDetails[0].noOfDays: {Value} (TokenType: {TokenType})",
									noOfDaysValue, firstLeave.GetProperty("noOfDays").ValueKind);
							}

							var employeeResponse = JsonSerializer.Deserialize<ReadleaveResponeById>(responseContent, jsonOptions);
							leave = employeeResponse ?? new ReadleaveResponeById();
							if (leave.LeaveDetails != null)
							{

								var attachmentsJson = JsonSerializer.Serialize(leave.LeaveDetails[0].AttachRefList);
								HttpContext.Session.SetString("attachments", attachmentsJson);
								var json = HttpContext.Session.GetString("attachments");
								List<AttachRefItem>? attachments = new List<AttachRefItem>();

								if (!string.IsNullOrEmpty(json))
								{
									ViewBag.attachemnts = JsonSerializer.Deserialize<List<AttachRefItem>>(json);
								}
							}
						}
						catch (JsonException ex)
						{
							_logger.LogError(ex, "Failed to deserialize JSON response for company {CompanyId}. Error at path: {Path}, Message: {Message}",
								companycode, ex.Path, ex.Message);
							return new ReadleaveResponeById();
						}
					}
					else
					{
						_logger.LogError("API call failed: {StatusCode}, {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
					}
				}				
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception while calling GetLeaveBalance for user {UserId}", userid);
			}

			return leave;
			
		}
	

		public async Task<UpdateLeaveResponse?> UpdateLeaveRequest([FromBody] CreateLeaveRequest input)
        {
            string? companyid = HttpContext.Session.GetString("CompanyCode");
            string? userid = HttpContext.Session.GetString("Username");

            try
            {               
                using var client = _httpClientFactory.CreateClient();

                var token = await _authentications.GetAccessTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("Failed to acquire access token for company {CompanyId}", companyid);
                    return null;
                }

                var apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.UpdateLeaveRequestUrl;

                var requestBody = new leaveResponseRequest
                {
					leaveResponse = new leaveResponse
                    {
						requestType = "Update",
						CompanyCode = companyid,
                        UserId = userid,
                        leaveID = input.LeaveID,
                        leaveType = input.LeaveType,
                        leaveStart = Convert.ToString(input.LeaveStart),
                        leaveEnd = Convert.ToString(input.LeaveEnd),
                        backDatedLeave = "No",
						payleavewithSalary = "No",
						NoOfDays = input.NoOfDays.ToString(),
						Comments = input.Comments
                    }
                };

				//var jsonOptions = new JsonSerializerOptions
				//{
				//    PropertyNamingPolicy = null,
				//    PropertyNameCaseInsensitive = true
				//};
				var jsonOptions = _authentications.GetJsonOptions();
				var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsync(apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("CreateLeaveRequest response: {response}", responseContent);

                if (response.IsSuccessStatusCode)
                {
                    var resultObj = JsonSerializer.Deserialize<UpdateLeaveResponse>(responseContent, jsonOptions);
                    var msg = resultObj;
                    return resultObj;
                }
                else
                {
                    _logger.LogError("API call failed: {StatusCode} - {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while creating leave request for user {UserId}", userid);
                return null;
            }
        }
        public async Task<LeaveResponse?> DeleteLeaveRequest(string leaveId )
        {
            string? companyid = HttpContext.Session.GetString("CompanyCode");
            string? userid = HttpContext.Session.GetString("Username");

            try
            {
                using var client = _httpClientFactory.CreateClient();

                var token = await _authentications.GetAccessTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("Failed to acquire access token for company {CompanyId}", companyid);
                    return null;
                }

                var apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.DeleteLeaveRequestUrl;

                var requestBody = new DeleteleaveResponseRequest
                {
                    leaveRequestDetails = new leaveRequestDetails
                    {
                        CompanyCode = companyid,
                        UserId = userid,
                        leaveID = leaveId,

                    }
                };

				//var jsonOptions = new JsonSerializerOptions
				//{
				//    PropertyNamingPolicy = null,
				//    PropertyNameCaseInsensitive = true
				//};
				var jsonOptions = _authentications.GetJsonOptions();
				var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsync(apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Delete Leave request response: {response}", responseContent);

                if (response.IsSuccessStatusCode)
                {
                    var resultObj = JsonSerializer.Deserialize<LeaveResponse>(responseContent, jsonOptions);
                    var msg = resultObj?.Message;
                    return resultObj;
                }
                else
                {
                    _logger.LogError("API call failed: {StatusCode} - {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                    return null;

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while creating leave request for user {UserId}", userid);
                return null;
            }
        }            		
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<wfContractResponse?> SubmitWorkflow(wfContract input)
		{
			string? companyId = HttpContext.Session.GetString("CompanyCode");
			string? userId = HttpContext.Session.GetString("Username");
			try
			{
				// Validate input
				using var client = _httpClientFactory.CreateClient();

				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for company {CompanyId}", companyId);
					return new wfContractResponse { Message = "Failed to authenticate." };
				}
				string apiUrl;

				apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.SubmitWorkflowUrl;

				var requestBody = new wfContractRequest
				{
					wfContract = new wfContract
					{
						CompanyId = companyId,
						raisedUser = userId,

						workflowTemplateName = input.workflowTemplateName ?? "Leave Request",
						submittingUser = input.submittingUser ?? "001038",
						RecordId = input.RecordId ?? "5637152076",
						TableId = input.TableId ?? "66492",

						Comments = input.Comments ?? "TestSubmit"
					}
				};

				//var jsonOptions = new JsonSerializerOptions
				//{
				//	PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Match API's expected casing
				//	PropertyNameCaseInsensitive = true
				//};
				var jsonOptions = _authentications.GetJsonOptions();
				var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
				var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await client.PostAsync(apiUrl, content);
				var responseContent = await response.Content.ReadAsStringAsync();

				_logger.LogInformation("SubmitWorkflow response: {StatusCode}", response.StatusCode);

				if (response.IsSuccessStatusCode)
				{
					var resultObj = JsonSerializer.Deserialize<wfContractResponse>(responseContent, jsonOptions);
					return resultObj;
				}
				else
				{
					_logger.LogError("API call failed: {StatusCode} - {ReasonPhrase} - {ResponseContent}",
						response.StatusCode, response.ReasonPhrase, responseContent);
					return new wfContractResponse { Message = $"API call failed: {response.ReasonPhrase}" };
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception while SubmitWorkflow for user {UserId}", userId);
				return new wfContractResponse { Message = "An error occurred while processing the request." };
			}
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<wfContractResponse?> ApproveWorkflow(_contract input)
		{
			string? companyId = HttpContext.Session.GetString("CompanyCode");
			string? userId = HttpContext.Session.GetString("Username");
			try
			{
				// Validate input
				using var client = _httpClientFactory.CreateClient();

				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for company {CompanyId}", companyId);
					return new wfContractResponse { Message = "Failed to authenticate." };
				}
				string apiUrl;

				apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.ApproveWorkflowUrl;

				var requestBody = new ApprovewfRequest
				{
					_contract = new _contract
					{
						CompanyId = companyId,
						SpecialApproval = input.SpecialApproval ?? "Chairmans",


						RecordId = input.RecordId ?? "5637149826",
						TableId = input.TableId ?? "31647",

						Comments = input.Comments ?? "OK"
					}
				};

				//var jsonOptions = new JsonSerializerOptions
				//{
				//	PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Match API's expected casing
				//	PropertyNameCaseInsensitive = true
				//};
				var jsonOptions = _authentications.GetJsonOptions();
				var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
				var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await client.PostAsync(apiUrl, content);
				var responseContent = await response.Content.ReadAsStringAsync();

				_logger.LogInformation("ApproveWorkflow response: {StatusCode}", response.StatusCode);

				if (response.IsSuccessStatusCode)
				{
					var resultObj = JsonSerializer.Deserialize<wfContractResponse>(responseContent, jsonOptions);
					return resultObj;
				}
				else
				{
					_logger.LogError("API call failed: {StatusCode} - {ReasonPhrase} - {ResponseContent}",
						response.StatusCode, response.ReasonPhrase, responseContent);
					return new wfContractResponse { Message = $"API call failed: {response.ReasonPhrase}" };
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception while ApproveWorkflow for user {UserId}", userId);
				return new wfContractResponse { Message = "An error occurred while processing the request." };
			}
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<wfContractResponse?> DelegateWorkflow(_contract input)
		{
			try
			{

				using var client = _httpClientFactory.CreateClient();

				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for company {EmpId}", input.EmpId);
					return new wfContractResponse { Message = "Failed to authenticate." };
				}
				string apiUrl;

				apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.DelegateWorkflowUrl;

				var requestBody = new DelegatewfRequest
				{
					_contract = new _contract
					{
						RaisedBy = input.RaisedBy ?? "001063",
						EmpId = input.EmpId ?? "001063",
						raisedUser = input.raisedUser ?? "000002",

						RecordId = input.RecordId ?? "5637149826",

						Comments = input.Comments ?? "delegate"
					}
				};

				//var jsonOptions = new JsonSerializerOptions
				//{
				//	PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Match API's expected casing
				//	PropertyNameCaseInsensitive = true
				//};
				var jsonOptions = _authentications.GetJsonOptions();
				var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
				var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await client.PostAsync(apiUrl, content);
				var responseContent = await response.Content.ReadAsStringAsync();

				_logger.LogInformation("DelegateWorkflow response: {StatusCode}", response.StatusCode);

				if (response.IsSuccessStatusCode)
				{
					var resultObj = JsonSerializer.Deserialize<wfContractResponse>(responseContent, jsonOptions);
					return resultObj;
				}
				else
				{
					_logger.LogError("API call failed: {StatusCode} - {ReasonPhrase} - {ResponseContent}",
						response.StatusCode, response.ReasonPhrase, responseContent);
					return new wfContractResponse { Message = $"API call failed: {response.ReasonPhrase}" };
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception while DelegateWorkflow for user {EmpId}", input.EmpId);
				return new wfContractResponse { Message = "An error occurred while processing the request." };
			}
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<wfContractResponse?> RejectWorkflow(_contract input)
		{
			try
			{
				string? companyId = HttpContext.Session.GetString("CompanyCode");
				using var client = _httpClientFactory.CreateClient();

				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for company {EmpId}", input.EmpId);
					return new wfContractResponse { Message = "Failed to authenticate." };
				}
				string apiUrl;

				apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.RejectWorkflowUrl;

				var requestBody = new RejectwfRequest
				{
					_contract = new _contract
					{
						RecordId = input.RecordId ?? "5637157326",
						TableId = input.TableId ?? "66492",
						CompanyId = companyId,
						Comments = input.Comments ?? "TestReject"
					}
				};

				//var jsonOptions = new JsonSerializerOptions
				//{
				//	PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Match API's expected casing
				//	PropertyNameCaseInsensitive = true
				//};
				var jsonOptions = _authentications.GetJsonOptions();
				var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
				var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await client.PostAsync(apiUrl, content);
				var responseContent = await response.Content.ReadAsStringAsync();

				_logger.LogInformation("DelegateWorkflow response: {StatusCode}", response.StatusCode);

				if (response.IsSuccessStatusCode)
				{
					var resultObj = JsonSerializer.Deserialize<wfContractResponse>(responseContent, jsonOptions);
					return resultObj;
				}
				else
				{
					_logger.LogError("API call failed: {StatusCode} - {ReasonPhrase} - {ResponseContent}",
						response.StatusCode, response.ReasonPhrase, responseContent);
					return new wfContractResponse { Message = $"API call failed: {response.ReasonPhrase}" };
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception while DelegateWorkflow for user {EmpId}", input.EmpId);
				return new wfContractResponse { Message = "An error occurred while processing the request." };
			}
		}

		public async Task<LeaveResumptionDetailsResponse> ReadLeaveResumption(string leaveid)
		{
			var leave = new LeaveResumptionDetailsResponse();
			string? companyid = HttpContext.Session.GetString("CompanyCode");
			string? userId = HttpContext.Session.GetString("Username");
			ViewBag.userId = userId;
			try
			{
				using var client = _httpClientFactory.CreateClient();
				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for company {CompanyId}", companyid);
					return leave;
				}

				var apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.ReadLeaveResumptionUrl;

				var requestBody = new userLeaveResumptionContractRequest
				{
					readUpdateLeave = new readUpdateLeave
					{
						companyCode = companyid,
						userID = userId,
						leaveID = leaveid ?? "LV-000000406"
					}
				};

				//var jsonOptions = new JsonSerializerOptions
				//{
				//	PropertyNamingPolicy = null,
				//	PropertyNameCaseInsensitive = true
				//};
				var jsonOptions = _authentications.GetJsonOptions();
				var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
				var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await client.PostAsync(apiUrl, content);
				var responseContent = await response.Content.ReadAsStringAsync();

				_logger.LogInformation("ReadLeaveResumption API Raw JSON: {Json}", responseContent);

				if (response.IsSuccessStatusCode)
				{
					var employeeResponse = JsonSerializer.Deserialize<LeaveResumptionDetailsResponse>(responseContent, jsonOptions);
					leave = employeeResponse ?? new LeaveResumptionDetailsResponse();
				}
				else
				{
					_logger.LogError("API call failed: {StatusCode}, {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception while getting ReadLeaveResumption {CompanyId}", companyid);
			}

			return leave;
		}
		private async Task<userLeaveResumptionContractInfoResponse> ReadLeaveResumptionInformation()
		{
			var leave = new userLeaveResumptionContractInfoResponse();
			string? companyid = HttpContext.Session.GetString("CompanyCode");
			string? userId = HttpContext.Session.GetString("Username");

			try
			{
				using var client = _httpClientFactory.CreateClient();
				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for company {CompanyId}", companyid);
					return leave;
				}

				var apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.ReadLeaveResumptionInfoUrl;

				var requestBody = new userLeaveResumptionContractInfoRequest
				{
					userLeaveResumptionContract = new userLeaveResumptionContract
					{
						companyCode = companyid,
						userID = userId
					}
				};

				//var jsonOptions = new JsonSerializerOptions
				//{
				//	PropertyNamingPolicy = null,
				//	PropertyNameCaseInsensitive = true
				//};
				var jsonOptions = _authentications.GetJsonOptions();
				var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
				var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await client.PostAsync(apiUrl, content);
				var responseContent = await response.Content.ReadAsStringAsync();

				_logger.LogInformation("ReadLeaveResumption API Raw JSON: {Json}", responseContent);

				if (response.IsSuccessStatusCode)
				{
					var employeeResponse = JsonSerializer.Deserialize<userLeaveResumptionContractInfoResponse>(responseContent, jsonOptions);
					leave = employeeResponse ?? new userLeaveResumptionContractInfoResponse();
				}
				else
				{
					_logger.LogError("API call failed: {StatusCode}, {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception while getting ReadLeaveResumption {CompanyId}", companyid);
			}

			return leave;
		}
		[HttpPost]
		[Route("/Leave/UpdateLeaveResumptionRequest")]
		[ValidateAntiForgeryToken]
		[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
		public async Task<UpdateLeaveResumptionResponse> UpdateLeaveResumptionRequest([FromBody] ReadUpdateLeaveResumptionResponse input)
		{
			string? companyId = HttpContext.Session.GetString("CompanyCode");
			string? userId = HttpContext.Session.GetString("Username");

			var response = new UpdateLeaveResumptionResponse
			{
				LeaveDetails = new List<LeaveDetail> { new LeaveDetail { MsgFlag = "Failed" } },
				LeaveResponseMsg = new List<LeaveResponseMsg> { new LeaveResponseMsg { MessageFlag = "Failed" } }

			};

			try
			{
				// Validate session
				if (string.IsNullOrEmpty(companyId) || string.IsNullOrEmpty(userId))
				{
					response.LeaveDetails[0].Msg = "Session data is missing. Please log in again.";
					_logger.LogError("Session data missing: CompanyCode={CompanyId}, UserId={UserId}", companyId, userId);
					return response;
				}

				// Validate input
				if (input?.readUpdateLeave == null || string.IsNullOrEmpty(input.readUpdateLeave.leaveID) || string.IsNullOrEmpty(input.readUpdateLeave.ReturnDate))
				{
					response.LeaveDetails[0].Msg = "Leave ID and Return Date are required.";
					_logger.LogError("Invalid input: LeaveID={LeaveID}, ReturnDate={ReturnDate}, UserId={UserId}",
						input?.readUpdateLeave?.leaveID, input?.readUpdateLeave?.ReturnDate, userId);
					return response;
				}

				// Parse ReturnDate (convert dd/MM/yyyy to M/d/yyyy for D365)
				if (!_authentications.TryParseDate(input.readUpdateLeave.ReturnDate, out string returnDate))
				{
					response.LeaveDetails[0].Msg = "Invalid date format for ReturnDate. Expected format: dd/MM/yyyy.";
					_logger.LogError("Invalid date format for ReturnDate: {ReturnDate}, UserId={UserId}", input.readUpdateLeave.ReturnDate, userId);
					return response;
				}

				// Get access token
				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					response.LeaveDetails[0].Msg = "Failed to acquire access token.";
					_logger.LogError("Failed to acquire access token for company {CompanyId}, UserId={UserId}", companyId, userId);
					return response;
				}

				// Prepare request body for D365 API
				var requestBody = new ReadUpdateLeaveResumptionResponse
				{
					readUpdateLeave = new ReadUpdateLeaveResumption
					{
						CompanyCode = companyId,
						leaveID = input.readUpdateLeave.leaveID,
						RequestType = "Update",
						ReturnDate = returnDate,
						WfComments = input.readUpdateLeave.WfComments,
						attachRefList=input.readUpdateLeave.attachRefList
					}
				};

				//var jsonOptions = new JsonSerializerOptions
				//{
				//	PropertyNamingPolicy = null,
				//	PropertyNameCaseInsensitive = true,
				//	Converters = { new FlexibleIntConverter() }
				//};
				var jsonOptions = _authentications.GetJsonOptions();
				// Log request payload
				_logger.LogInformation("Sending to D365: {RequestBody}, UserId={UserId}", JsonSerializer.Serialize(requestBody, jsonOptions), userId);

				// Make API call
				using var client = _httpClientFactory.CreateClient();
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.UpdateLeaveResumptionUrl;
				var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
				var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

				var apiResponse = await client.PostAsync(apiUrl, content);
				var responseContent = await apiResponse.Content.ReadAsStringAsync();

				_logger.LogInformation("D365 API response: {Response}, UserId={UserId}", responseContent, userId);

				if (apiResponse.IsSuccessStatusCode)
				{
					var resultObj = JsonSerializer.Deserialize<UpdateLeaveResumptionResponse>(responseContent, jsonOptions);
					if (resultObj?.LeaveResponseMsg?.Any() != true)
					{
						response.LeaveResponseMsg[0].message = "No data returned from D365 API.";
						_logger.LogWarning("Empty LeaveDetails in D365 response for UserId={UserId}", userId);
						return response;
					}

					return resultObj;
				}
				else
				{
					response.LeaveResponseMsg[0].message = $"D365 API call failed: {apiResponse.ReasonPhrase}";
					_logger.LogError("D365 API call failed: StatusCode={StatusCode}, ReasonPhrase={ReasonPhrase}, UserId={UserId}",
						apiResponse.StatusCode, apiResponse.ReasonPhrase, userId);
					return response;
				}
			}
			catch (Exception ex)
			{
				response.LeaveResponseMsg[0].message = "An error occurred while processing the request.";
				_logger.LogError(ex, "Exception in UpdateLeaveResumptionRequest for UserId={UserId}", userId);
				return response;
			}
		}

        [HttpPost]
        public async Task<FileResponce> CreateAttachFile([FromForm] FileUploadViewModel formData)
        {
            var fileres = new FileResponce();
            string? companyId = HttpContext.Session.GetString("CompanyCode");
            var read = new LeaveDetails();
            try
            {
                using var client = _httpClientFactory.CreateClient();
                var token = await _authentications.GetAccessTokenAsync();

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("Failed to acquire access token for company {CompanyId}", companyId);
                    return fileres;
                }

                var apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.CreateAttachFileUrl;

                if (formData.Files != null && formData.Files.Count > 0)
                {
                    var attachList = new List<AttachRef>();

                    foreach (var file in formData.Files)
                    {
                        using var memoryStream = new MemoryStream();
                        await file.CopyToAsync(memoryStream);

                        string base64 = Convert.ToBase64String(memoryStream.ToArray());

                        attachList.Add(new AttachRef
                        {
                            fileName = file.FileName,
                            attachRef = base64
                        });
                        HttpContext.Session.SetString("fileName", file.FileName ?? "");
                        HttpContext.Session.SetString("attachRef", base64 ?? "");

                    }

                    var updAttachFile = new UpdAttachFile
                    {
                        companyCode = companyId,
                        uniqueRecID = formData.uniqueRecID,
                        requestTableID = formData.requestTableID,
                        attachRefList = attachList
                    };

                    var requestBody = new RootObject
                    {
                        updAttachFile = updAttachFile
                    };

					//var jsonOptions = new JsonSerializerOptions
					//{
					//    PropertyNamingPolicy = null,
					//    PropertyNameCaseInsensitive = true
					//};
					var jsonOptions = _authentications.GetJsonOptions();
					var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
                    var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var response = await client.PostAsync(apiUrl, content);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    _logger.LogInformation("createAttachFile API Raw JSON: {Json}", responseContent);

                    if (response.IsSuccessStatusCode)
                    {
                        var fileResponse = JsonSerializer.Deserialize<FileResponce>(responseContent, jsonOptions);
                        fileres = fileResponse ?? new FileResponce();
                    }
                    else
                    {
                        _logger.LogError("API call failed: {StatusCode}, {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                    }
                }
                else
                {
                    _logger.LogWarning("No files were uploaded for attach file.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while calling createAttachFile for company {CompanyId}", companyId);
            }

            return fileres;
        }	    
	}
}
