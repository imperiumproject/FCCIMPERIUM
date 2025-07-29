using Microsoft.AspNetCore.Mvc;
using SMARTHR.WEB.FCCHRServices.D365;
using SMARTHR.WEB.FCCHRServices.FCCHRBusinessInterface;
using SMARTHR.WEB.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.ComponentModel.Design;

namespace SMARTHR.WEB.Controllers
{
	public class ITAssetsController : Controller
	{

		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _configuration;
		private readonly ILogger<ITAssetsController> _logger;
		private readonly IAuthentications _authentications;
		public ITAssetsController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<ITAssetsController> logger,
			IAuthentications authentications)
		{
			_httpClientFactory = httpClientFactory;
			_configuration = configuration;
			_logger = logger;
			_authentications = authentications;			
		}

		public async Task<IActionResult> ReadAssetRequestList()
		{
			string? username = HttpContext.Session.GetString("Username");
			ViewBag.Username = username;
			//var res = ReadAssetRequestDetails("");

			var response = await ReadAssetRequestInformation();
            var model = response?.assetRequestList ?? new List<ITAssetsModels>();
            return View(model);
         
		} 

        private async Task<ITAssetsReadInfoResponse> ReadAssetRequestInformation()
        {
            var assetsList = new ITAssetsReadInfoResponse();
            string? companyid = HttpContext.Session.GetString("CompanyCode");
            string? userId = HttpContext.Session.GetString("Username");

            try
            {
                using var client = _httpClientFactory.CreateClient();
                var token = await _authentications.GetAccessTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("Failed to acquire access token for company {CompanyId}", companyid);
                    return assetsList;
                }

                var apiUrl = $"{_configuration["D365:BaseUrl"]}{D365ApiURL.ReadAssetRequestListUrl}";

                var requestBody = new ReadAssetsInfoRequest
                {
                    assetRequestContract = new ReadAssetsInfo
                    {
                        companyId = companyid,
                        employeeId = userId
                    }
                };              
                var jsonOptions = _authentications.GetJsonOptions();
                var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsync(apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Read assets API Raw JSON: {Json}", responseContent);

                if (response.IsSuccessStatusCode)
                {
                    try
                    {                                             
                        var AssetsResponse = JsonSerializer.Deserialize<ITAssetsReadInfoResponse>(responseContent, jsonOptions);
                        assetsList = AssetsResponse ?? new ITAssetsReadInfoResponse();
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "Failed to deserialize JSON response for company {CompanyId}. Error at path: {Path}, Message: {Message}",
                            companyid, ex.Path, ex.Message);
                        return new ITAssetsReadInfoResponse();
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

            return assetsList;
        }



		public async Task<ITAssetsReadInfoResponse> ReadAssetRequestDetails(string assetsId)
		{
			var assetsList = new ITAssetsReadInfoResponse();
			string? companyid = HttpContext.Session.GetString("CompanyCode");
			string? userId = HttpContext.Session.GetString("Username");			
			try
			{
				using var client = _httpClientFactory.CreateClient();
				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for company {CompanyId}", companyid);
					return assetsList;
				}

				var apiUrl = $"{_configuration["D365:BaseUrl"]}{D365ApiURL.ReadAssetRequestUrl}";

				var requestBody = new ReadAssetsDetailsRequest
				{
					assetRequestContract = new ReadAssetsDetails
					{
						companyId = companyid,
						requestId = assetsId
					}
				};
				var jsonOptions = _authentications.GetJsonOptions();
				var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
				var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await client.PostAsync(apiUrl, content);
				var responseContent = await response.Content.ReadAsStringAsync();

				_logger.LogInformation("Read assets API Raw JSON: {Json}", responseContent);

				if (response.IsSuccessStatusCode)
				{
					try
					{
						var AssetsResponse = JsonSerializer.Deserialize<ITAssetsReadInfoResponse>(responseContent, jsonOptions);
						assetsList = AssetsResponse ?? new ITAssetsReadInfoResponse();
					}
					catch (JsonException ex)
					{
						_logger.LogError(ex, "Failed to deserialize JSON response for company {CompanyId}. Error at path: {Path}, Message: {Message}",
							companyid, ex.Path, ex.Message);
						return new ITAssetsReadInfoResponse();
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

			return assetsList;
		}



		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<LeaveResponse?> CreateAssetsRequest([FromBody] AssetRequestContractResponse input)
		{
			string? companyId = HttpContext.Session.GetString("CompanyCode");
			string? userId = HttpContext.Session.GetString("Username");

			// Validate session values
			if (string.IsNullOrEmpty(companyId) || string.IsNullOrEmpty(userId))
			{
				_logger.LogError("Missing Company Code or Username in session for user {UserId}", userId);
				return new LeaveResponse { Message = "Session data is missing." };
			}

			try
			{
				// Validate input
				var assetRequestContract = input.assetRequestContract;
				if (assetRequestContract == null || string.IsNullOrEmpty(assetRequestContract.loanType))
				{
					_logger.LogError("Invalid input data for leave request by user {UserId}", userId);
					return new LeaveResponse { Message = "Invalid input data." };
				}

				// Parse dates
				if (!_authentications.TryParseDate(assetRequestContract.requiredBy, out string startDate))
				{
					_logger.LogError("Invalid date format for LeaveStart: {LeaveStart}", assetRequestContract.requiredBy);
					return new LeaveResponse { Message = $"Invalid date format for LeaveStart: {assetRequestContract.requiredBy}" };
				}

				if (!_authentications.TryParseDate(assetRequestContract.requiredTill, out string endDate))
				{
					_logger.LogError("Invalid date format for LeaveEnd: {LeaveEnd}", assetRequestContract.requiredTill);
					return new LeaveResponse { Message = $"Invalid date format for LeaveEnd: {assetRequestContract.requiredTill}" };
				}

				using var client = _httpClientFactory.CreateClient();

				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for company {CompanyId}", companyId);
					return new LeaveResponse { Message = "Failed to authenticate." };
				}

				string apiUrl = $"{_configuration["D365:BaseUrl"]}{D365ApiURL.CreateUpdateAssetRequestUrl}";

				// Use attachRefList directly from input
				var attachList = assetRequestContract.attachRefList ?? new List<AttachRef>();

				var requestBody = new AssetRequestContractResponse
				{
					assetRequestContract = new AssetRequestContract
					{
						companyId = companyId, 
						raisedBy = userId,
						employeeId = assetRequestContract.employeeId,
						requiredBy = Convert.ToString(startDate),
						requiredTill = Convert.ToString(endDate),
						requestId =assetRequestContract.requestId,
						loanType = assetRequestContract.loanType,
						purpose = assetRequestContract.purpose,
						additionalDetails = assetRequestContract.additionalDetails,
						comments = "Create",					
						attachRefList = attachList 
					}
				};

				var jsonOptions = _authentications.GetJsonOptions();
				var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
				var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await client.PostAsync(apiUrl, content);
				var responseContent = await response.Content.ReadAsStringAsync();

				_logger.LogInformation("Create assets response: {StatusCode}", response.StatusCode);

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

		public async Task<string> GetAssetRequestId()
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

				var apiUrl = $"{_configuration["D365:BaseUrl"]}" + D365ApiURL.GetAssetRequestIdUrl;

				var requestBody = new ItAssetsNumIdRequest
				{
					AssetRequestContract = new ItAssetsId
					{
						companyId = companyid
					}
				};				
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
				_logger.LogError(ex, "Exception while calling GetLeaveId for company {CompanyId}", companyid);
				result = $"Exception: {ex.Message}";
				result = "Error";
			}

			return result;
		}

		public async Task<List<LoanTypes?>> GetLoanTypes(string EmiId)
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
				var apiUrl = $"{_configuration["D365:BaseUrl"]}" + D365ApiURL.ReadLoanTypeUrl;
				var requestBody = new ItAssetsNumIdRequest
				{
					AssetRequestContract = new ItAssetsId
					{
						companyId = companyId
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
						var result = JsonSerializer.Deserialize<LoanTypeResponse>(responseContent, jsonOptions) ?? new LoanTypeResponse { loanTypeResultList = new List<LoanTypes>() };

						// Log deserialized result for debugging
						_logger.LogInformation("Deserialized LeaveTypes count: {Count}", result.loanTypeResultList != null ? result.loanTypeResultList.Count : 0);
						return result.loanTypeResultList;
					}
					catch (JsonException ex)
					{
						_logger.LogError(ex, "Failed to deserialize API response for user {UserId}: {ResponseContent}", userId, responseContent);
						List<LoanTypes?> LoanedItemsResult = new List<LoanTypes?>();
						return LoanedItemsResult;
					}
				}
				else
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					_logger.LogError("API call failed for user {UserId}: {StatusCode}, {ReasonPhrase}, {ErrorContent}",
						userId, response.StatusCode, response.ReasonPhrase, errorContent);
					List<LoanTypes?> LoanedItemsResult = new List<LoanTypes?>();
					return LoanedItemsResult;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Profile retrieval failed for user {UserId}", userId ?? "Unknown");
				List<LoanTypes?> LoanedItemsResult = new List<LoanTypes?>();
				return LoanedItemsResult;

			}
		}
	}
}
