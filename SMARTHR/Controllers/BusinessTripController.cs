using Microsoft.AspNetCore.Mvc;
using SMARTHR.WEB.Models;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.ComponentModel.Design;
using SMARTHR.WEB.FCCHRServices.FCCHRBusinessInterface;
using SMARTHR.WEB.FCCHRServices.D365;

namespace SMARTHR.WEB.Controllers
{
    public class BusinessTripController : Controller
    {
		//private readonly ILeaveService _leaveService;ILeaveService leaveService
		private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LeaveController> _logger;
        private readonly IAuthentications _authentications;
        public BusinessTripController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<LeaveController> logger,
            IAuthentications authentications)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
            _authentications = authentications;          
        }
		[HttpPost]

		public async Task<wfContractResponse?> RecallWorkflow(RecallWorkFlow input)
		{
			string? companyId = HttpContext.Session.GetString("CompanyCode");
			string? userId = HttpContext.Session.GetString("Username");

			try
			{				
				using var client = _httpClientFactory.CreateClient();

				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for user {userId}", userId);
					return new wfContractResponse { Message = "Failed to authenticate." };
				}
				string apiUrl;

				apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.RecallWorkflowUrl;

				var requestBody = new RecallWorkResponse
				{
					Contract = new RecallWorkFlow
					{
						RaisedBy = userId,
						RecordId = input.RecordId,
						TableId = input.TableId,
						ReturnToLevel = input.ReturnToLevel??"0",
						CompanyId = companyId,
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

				_logger.LogInformation("RecallWorkflow response: {StatusCode}", response.StatusCode);

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
				_logger.LogError(ex, "Exception while RecallWorkflow for user {UserId}", userId);
				return new wfContractResponse { Message = "An error occurred while processing the request." };
			}
		}



		public async Task<IActionResult> BusinessTrip(businessTripContract input, string BusinessTripReqId)
        {
			string? username = HttpContext.Session.GetString("Username");
			ViewBag.Username = username;

			var model = await ReadBusinesstripInformation();
			var response = model?.BusinessDetails ?? new List<SMARTHR.WEB.Models.BusinessTripDeatils>();
			return View(response);
		}

		private async Task<BusinessTripResponseResponse> ReadBusinesstripInformation()
		{
			var businesstrip = new BusinessTripResponseResponse();
			string? companyid = HttpContext.Session.GetString("CompanyCode");
			string? userId = HttpContext.Session.GetString("Username");
			try
			{
				using var client = _httpClientFactory.CreateClient();
				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for company {CompanyId}", companyid);
					return businesstrip;
				}

				var apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.ReadBusniessTripRequestInfoUrl;

				var requestBody = new BusinessTripContractListResponse
				{
					businessTripContract = new BusinessTripContractList
					{
						companyId = companyid,
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
					var businessResponse = JsonSerializer.Deserialize<BusinessTripResponseResponse>(responseContent, jsonOptions);
					businesstrip = businessResponse ?? new BusinessTripResponseResponse();
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

			return businesstrip;
		}
		public IActionResult BusinessTripResumption()
        {
			string? username = HttpContext.Session.GetString("Username");
			ViewBag.Username = username;
			return View();
        }
        public async Task<string> GetBusinessTripId()
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

                var apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.GetBusinessTripIdUrl;

                var requestBody = new contractRequest
                {
                    contract = new Contract
                    {
                        CompanyId = companyid
                    }
                };

				//var jsonOptions = new JsonSerializerOptions
				//{
				//    PropertyNamingPolicy = null,
				//    WriteIndented = false
				//};
				var jsonOptions = _authentications.GetJsonOptions();
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
                _logger.LogError(ex, "Exception while calling GetBusinessTripId for company {CompanyId}", companyid);
                result = $"Exception: {ex.Message}";
                result = "Error";
            }

            return result;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<BusinessTripResponse?> CreateBusinessTripRequest([FromBody] businessTripContract input)
        {
			string? username = HttpContext.Session.GetString("Username");
			ViewBag.Username = username;
			string? companyId = HttpContext.Session.GetString("CompanyCode");
            string? userId = HttpContext.Session.GetString("Username");

            // Validate session values
            if (string.IsNullOrEmpty(companyId) || string.IsNullOrEmpty(userId))
            {
                _logger.LogError("Missing CompanyCode or Username in session for user {UserId}", userId);
                return new BusinessTripResponse { Message = "Session data is missing." };
            }

            try
            {
                //Parse dates
                //string? fromdate1 = input.FromDate != null ? Convert.ToString(input.FromDate) : "";
                //string? todate1 = input.ToDate != null ? Convert.ToString(input.ToDate) : "";
                if (!_authentications.TryParseDate(input.FromDate ?? DateTime.Now.ToString(), out string startDate))
                {
                    _logger.LogError("Invalid date format for LeaveStart: {LeaveStart}", input.FromDate);
                    return new BusinessTripResponse { Message = $"Invalid date format for LeaveStart: {input.FromDate}" };
                }

                if (!_authentications.TryParseDate(input.ToDate ?? DateTime.Now.ToString(), out string endDate))
                {
                    _logger.LogError("Invalid date format for LeaveEnd: {LeaveEnd}", input.ToDate);
                    return new BusinessTripResponse { Message = $"Invalid date format for LeaveEnd: {input.ToDate}" };
                }



                using var client = _httpClientFactory.CreateClient();

                var token = await _authentications.GetAccessTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("Failed to acquire access token for company {CompanyId}", companyId);
                    return new BusinessTripResponse { Message = "Failed to authenticate." };
                }

                var apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.CreateBusinessTripRequestUrl;
				var attachList = input.attachRefList ?? new List<attachRefList>();
				var requestBody = new CreateBusinessTripRequest
                {
                    businessTripContract = new businessTripContract
                    {
                        companyId = companyId,
                        userId = userId,
                        BusinessTripReqId = input.BusinessTripReqId ?? "USMF-000000052",
                        BusinessTripsExpenseType = input.BusinessTripsExpenseType ?? "Sponsored",
                        department = input.department ?? "",                       
                        FromDate = Convert.ToString(startDate),
                        ToDate = Convert.ToString(endDate),
                        SourceCountry = input.SourceCountry ?? "ABW",
                        DestinationCountry = input.DestinationCountry ?? "AFG",
                        Remarks = input.Remarks ?? "trip",
                        CurrencyCode = input.CurrencyCode ?? "KWD",
						RaisedBy=userId,
						attachRefList= attachList
                    }
                };

				//var jsonOptions = new JsonSerializerOptions
				//{
				//    PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Match API's expected casing
				//    PropertyNameCaseInsensitive = true
				//};
				var jsonOptions = _authentications.GetJsonOptions();
				var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsync(apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("CreateBusinessTrip response: {StatusCode}", response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    var resultObj = JsonSerializer.Deserialize<BusinessTripResponse>(responseContent, jsonOptions);
                    return resultObj;
                }
                else
                {
                    _logger.LogError("API call failed: {StatusCode} - {ReasonPhrase} - {ResponseContent}",
                        response.StatusCode, response.ReasonPhrase, responseContent);
                    return new BusinessTripResponse { Message = $"API call failed: {response.ReasonPhrase}" };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while creating Business request for user {UserId}", userId);
                return new BusinessTripResponse { Message = "An error occurred while processing the request." };
            }
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<BusinessTripResponse?> UpdateBusinessTripRequest([FromBody] businessTripContract input)
		{
			string? username = HttpContext.Session.GetString("Username");
			ViewBag.Username = username;
			string? companyId = HttpContext.Session.GetString("CompanyCode");
			string? userId = HttpContext.Session.GetString("Username");

			// Validate session values
			if (string.IsNullOrEmpty(companyId) || string.IsNullOrEmpty(userId))
			{
				_logger.LogError("Missing CompanyCode or Username in session for user {UserId}", userId);
				return new BusinessTripResponse { Message = "Session data is missing." };
			}

			try
			{

				if (!_authentications.TryParseDate(input.FromDate ?? DateTime.Now.ToString(), out string startDate))
				{
					_logger.LogError("Invalid date format for LeaveStart: {LeaveStart}", input.FromDate);
					return new BusinessTripResponse { Message = $"Invalid date format for LeaveStart: {input.FromDate}" };
				}

				if (!_authentications.TryParseDate(input.ToDate ?? DateTime.Now.ToString(), out string endDate))
				{
					_logger.LogError("Invalid date format for LeaveEnd: {LeaveEnd}", input.ToDate);
					return new BusinessTripResponse { Message = $"Invalid date format for LeaveEnd: {input.ToDate}" };
				}
				using var client = _httpClientFactory.CreateClient();

				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for company {CompanyId}", companyId);
					return new BusinessTripResponse { Message = "Failed to authenticate." };
				}

				var apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.UpdateBusinessTripRequestUrl;

				var requestBody = new UpdateBusinessTripRequest
				{
					businessTripContract = new businessTripContract
					{
						companyId = companyId,
						userId = userId,
						BusinessTripReqId = input.BusinessTripReqId ,
						BusinessTripsExpenseType = input.BusinessTripsExpenseType,
						department = input.department,
						FromDate = Convert.ToString(startDate)	,
						ToDate = Convert.ToString(endDate),
						SourceCountry = input.SourceCountry,
						DestinationCountry = input.DestinationCountry ,
						Remarks = input.Remarks,
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

				_logger.LogInformation("Updated response: {StatusCode}", response.StatusCode);

				if (response.IsSuccessStatusCode)
				{
					var resultObj = JsonSerializer.Deserialize<BusinessTripResponse>(responseContent, jsonOptions);
					return resultObj;
				}
				else
				{
					_logger.LogError("API call failed: {StatusCode} - {ReasonPhrase} - {ResponseContent}",
						response.StatusCode, response.ReasonPhrase, responseContent);
					return new BusinessTripResponse { Message = $"API call failed: {response.ReasonPhrase}" };
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception while creating Business request for user {UserId}", userId);
				return new BusinessTripResponse { Message = "An error occurred while processing the request." };
			}
		}

		public async Task<BusinessTripResponse> DeleteBusinessTripRequest(string BusinessTripReqId)
        {
            var bustrip = new BusinessTripResponse();
			string? companyid = HttpContext.Session.GetString("CompanyCode");
			try
            {
                using var client = _httpClientFactory.CreateClient();
                var token = await _authentications.GetAccessTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("Failed to acquire access token for company {BusinessTripReqId}", BusinessTripReqId);
                    return bustrip;
                }

                var apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.DeleteBusinessTripRequestUrl;

                var requestBody = new DeleteBusinessTripRequest
                {
                    _fccESSBusinessTripContract = new _fccESSBusinessTripContract
                    {
						companyId= companyid,
						BusinessTripReqId = BusinessTripReqId
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

                _logger.LogInformation("Employee API Raw JSON: {Json}", responseContent);

                if (response.IsSuccessStatusCode)
                {
                    var employeeResponse = JsonSerializer.Deserialize<BusinessTripResponse>(responseContent, jsonOptions);
                    bustrip = employeeResponse ?? new BusinessTripResponse();
                }
                else
                {
                    _logger.LogError("API call failed: {StatusCode}, {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while Delete BusinessTrip  Details {BusinessTripReqId}", BusinessTripReqId);
            }

            return bustrip;
        }


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult<object>> ReadBusinessTripRequest([FromForm] string BusinessTripReqId)
		{
			_logger.LogInformation("Received POST request for BusinessTripReqId: {BusinessTripReqId}", BusinessTripReqId);

			if (string.IsNullOrEmpty(BusinessTripReqId))
			{
				_logger.LogError("BusinessTripReqId is null or empty.");
				return BadRequest(new { error = "BusinessTripReqId is required." });
			}
            string? username = HttpContext.Session.GetString("Username");
            string? companyId = HttpContext.Session.GetString("CompanyCode");
			if (string.IsNullOrEmpty(companyId))
			{
				_logger.LogError("CompanyCode is not found in session for BusinessTripReqId: {BusinessTripReqId}", BusinessTripReqId);
				return BadRequest(new { error = "Company information is missing." });
			}

			try
			{
				using var client = _httpClientFactory.CreateClient();
				var token = await _authentications.GetAccessTokenAsync();
				_logger.LogInformation("Access token retrieved: {Token}", token?.Substring(0, Math.Min(token?.Length ?? 0, 10)) + "...");

				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for BusinessTripReqId: {BusinessTripReqId}", BusinessTripReqId);
					return StatusCode(500, new { error = "Failed to acquire access token." });
				}

				var apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.ReadBusinessTripRequestUrl;
				_logger.LogInformation("External API URL: {ApiUrl}", apiUrl);

				var requestBody = new ReadBusinessTripRequest
				{
					readBusinessTripAPIContract = new readBusinessTripAPIContract
					{
						companyId = companyId,
						UserId=username,
						BusinessTripReqId = BusinessTripReqId
					}
				};

				//var jsonOptions = new JsonSerializerOptions
				//{
				//	PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				//	PropertyNameCaseInsensitive = true
				//};
				var jsonOptions = _authentications.GetJsonOptions();
				var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
				_logger.LogInformation("Request body: {JsonString}", jsonString);

				var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await client.PostAsync(apiUrl, content);
				var responseContent = await response.Content.ReadAsStringAsync();

				_logger.LogInformation("External API response: StatusCode={StatusCode}, Content={ResponseContent}", response.StatusCode, responseContent);

				if (response.IsSuccessStatusCode)
				{
					var businessTripDetails = JsonSerializer.Deserialize<List<BusinessTripDeatils>>(responseContent, jsonOptions);
					if (businessTripDetails == null || !businessTripDetails.Any())
					{
						_logger.LogWarning("No business trip details returned for BusinessTripReqId: {BusinessTripReqId}", BusinessTripReqId);
						return Ok(new { businessTripDetails = new List<BusinessTripDeatils>() });
					}
					return Ok(new { businessTripDetails });
				}
				else
				{
					_logger.LogError("External API call failed: StatusCode={StatusCode}, ReasonPhrase={ReasonPhrase}, Response={ResponseContent}",
						response.StatusCode, response.ReasonPhrase, responseContent);
					return StatusCode((int)response.StatusCode, new { error = $"External API error: {response.ReasonPhrase}" });
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception while processing BusinessTripReqId: {BusinessTripReqId}. InnerException: {InnerException}",
					BusinessTripReqId, ex.InnerException?.Message);
				return StatusCode(500, new { error = "An error occurred while fetching business trip details.", details = ex.Message });
			}
		}

		public async Task<IActionResult> GetBusinessExpenceType()
		{
			string? companyid = HttpContext.Session.GetString("CompanyCode");
			try
			{				
				if (string.IsNullOrEmpty(companyid))
				{
					_logger.LogError("CompanyCode is not set in session.");
					return BadRequest("Company code is missing.");
				}

				using var client = _httpClientFactory.CreateClient();
				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for company {CompanyId}", companyid);
					return BadRequest("Failed to authenticate with the server.");
				}

				var apiUrl = $"{_configuration["D365:BaseUrl"]}"+ D365ApiURL.ExpenseTypesListUrl;

				var requestBody = new BusinessExpencetypeResponse
				{
					businessTripContract = new BusinessTypeExpence
					{
						companyId = companyid
					}
				};

				//var jsonOptions = new JsonSerializerOptions
				//{
				//	PropertyNamingPolicy = null, // Preserve exact property names
				//	WriteIndented = false
				//};
				var jsonOptions = _authentications.GetJsonOptions();
				var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
				var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await client.PostAsync(apiUrl, content);
				var responseContent = await response.Content.ReadAsStringAsync();

				// Log the raw response content
				_logger.LogInformation("API Response Content: {ResponseContent}", responseContent);

				if (response.IsSuccessStatusCode)
				{
					var expenseTypes = JsonSerializer.Deserialize<List<ExpenseTypeModel>>(responseContent, jsonOptions) ?? new List<ExpenseTypeModel>();
					_logger.LogInformation("Deserialized expenseTypes: {Count} items", expenseTypes.Count);
					return Ok(new { expenseTypes });
				}
				else
				{
					_logger.LogError("API call failed: {StatusCode}, {ReasonPhrase}, Response: {ResponseContent}",
						response.StatusCode, response.ReasonPhrase, responseContent);
					return BadRequest("API call failed.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception while calling GetBusinessExpenceType for company {CompanyId}", companyid);
				return BadRequest($"Exception: {ex.Message}");
			}
		}

		public async Task<IActionResult> GetCountries()
		{
			string? companyid = HttpContext.Session.GetString("CompanyCode");
			try
			{

				if (string.IsNullOrEmpty(companyid))
				{
					_logger.LogError("CompanyCode is not set in session.");
					return BadRequest("Company code is missing.");
				}

				using var client = _httpClientFactory.CreateClient();
				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for company {CompanyId}", companyid);
					return BadRequest("Failed to authenticate with the server.");
				}

				var apiUrl = $"{_configuration["D365:BaseUrl"]}"+ D365ApiURL.CountriesListUrl;

				var requestBody = new BusinessCountryResponse
				{
					essbusinessTripContract = new BusinessTypeExpence
					{
						companyId = companyid
					}
				};

				//var jsonOptions = new JsonSerializerOptions
				//{
				//	PropertyNamingPolicy = null, // Preserve exact property names
				//	WriteIndented = false
				//};
				var jsonOptions = _authentications.GetJsonOptions();
				var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
				var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await client.PostAsync(apiUrl, content);
				var responseContent = await response.Content.ReadAsStringAsync();

				// Log the raw response content
				_logger.LogInformation("API Response Content: {ResponseContent}", responseContent);

				if (response.IsSuccessStatusCode)
				{
					var counrtyList = JsonSerializer.Deserialize<List<CountryListModel>>(responseContent, jsonOptions) ?? new List<CountryListModel>();
					_logger.LogInformation("Deserialized expenseTypes: {Count} items", counrtyList.Count);
					return Ok(new { counrtyList });
				}
				else
				{
					_logger.LogError("API call failed: {StatusCode}, {ReasonPhrase}, Response: {ResponseContent}",
						response.StatusCode, response.ReasonPhrase, responseContent);
					return BadRequest("API call failed.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception while calling GetBusinessExpenceType for company {CompanyId}", companyid);
				return BadRequest($"Exception: {ex.Message}");
			}
		}
	}
}
