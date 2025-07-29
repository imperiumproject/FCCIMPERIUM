using Microsoft.AspNetCore.Mvc;
using SMARTHR.WEB.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using SMARTHR.WEB.FCCHRServices.FCCHRBusinessInterface;
using SMARTHR.WEB.FCCHRServices.D365;

namespace SMARTHR.WEB.Controllers
{
	public class ManagerController : Controller
	{
		//private readonly ILeaveService _leaveService;ILeaveService leaveService
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _configuration;
		private readonly ILogger<LeaveController> _logger;
		private readonly IAuthentications _authentications;
		public ManagerController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<LeaveController> logger,
			IAuthentications authentications)
		{
			_httpClientFactory = httpClientFactory;
			_configuration = configuration;
			_logger = logger;
			_authentications = authentications;
			//_leaveService = leaveService;
		}

		public async Task<List<LoanItems?>> GetLoanItems(string? userId)
		{
			string? companyId = HttpContext.Session.GetString("CompanyCode");
			//string? userId = HttpContext.Session.GetString("Username");
			// Check session for Profile data
			var profileJson = HttpContext.Session.GetString("Profile");
			var LoanItems = new LoanItems();
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
						CompanyCode = companyId ?? "USMF",
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

					_logger.LogInformation("Raw API response for user {UserId}: {ResponseContent}", userId, responseContent);

					try
					{
						var result = JsonSerializer.Deserialize<LoanItemsResponse>(responseContent, jsonOptions) ?? new LoanItemsResponse { LoanedItemsResult = new List<LoanItems>() };

						// Log deserialized result for debugging
						_logger.LogInformation("Deserialized LeaveTypes count: {Count}", result.LoanedItemsResult!=null?result.LoanedItemsResult.Count:0);

						return result.LoanedItemsResult;
					}
					catch (JsonException ex)
					{
						_logger.LogError(ex, "Failed to deserialize API response for user {UserId}: {ResponseContent}", userId, responseContent);
						List<LoanItems> LoanedItemsResult = new List<LoanItems>();
						return LoanedItemsResult;
					}
				}
				else
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					_logger.LogError("API call failed for user {UserId}: {StatusCode}, {ReasonPhrase}, {ErrorContent}",
						userId, response.StatusCode, response.ReasonPhrase, errorContent);
					List<LoanItems> LoanedItemsResult = new List<LoanItems>();
					return LoanedItemsResult;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Profile retrieval failed for user {UserId}", userId ?? "Unknown");
				List<LoanItems> LoanedItemsResult = new List<LoanItems>();
				return LoanedItemsResult;

			}
		}

		public async Task<IActionResult> Alltransaction()
		{
			string? userId = HttpContext.Session.GetString("Username");
			var loanItems = await GetLoanItems(userId);
			ViewBag.Loanmanager = loanItems;
			string? username = HttpContext.Session.GetString("Username");
			ViewBag.Username = username;
			var response = await ReadManagerLeaveResumption();
			var model = response?.WorkflowProcesses ?? new List<SMARTHR.WEB.Models.WorkflowProcess>();
			return View(model);
			
		}
		private async Task<WorkflowProcessList> ReadManagerLeaveResumption()
		{
			var leave = new WorkflowProcessList();
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

				var apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.WorkListByEmpGlobalFilterUrl;

				var requestBody = new WorkflowProcessServiceContractResponse
				{
					WorkflowProcessServiceContract = new WorkflowProcessServiceContract
					{
						CompanyId = companyid,
						Status = TranTypes.Submitte,
						RaisedBy = "",
						TransType = "",
						EmpId= userId
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
						var Response = JsonSerializer.Deserialize<WorkflowProcessList>(responseContent, jsonOptions);
						leave = Response ?? new WorkflowProcessList();
					}
					catch (JsonException ex)
					{
						_logger.LogError(ex, "Failed to deserialize JSON response for company {CompanyId}. Error at path: {Path}, Message: {Message}",
							companyid, ex.Path, ex.Message);
						return new WorkflowProcessList();
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
		
		public async Task<IActionResult> ReadManagerLevelRequest(string leaveId)
		{
			var unifiedResponse = new UnifiedLeaveResponse
			{
				BusinessTripDetails = new List<BusinessTripDeatils>(),
				LeaveDetails = new List<LeaveDetails>(),
				Success = false,
				loanList =new List<LoanItems>()
			};

			string companycode = HttpContext.Session.GetString("CompanyCode") ?? string.Empty;
			string? userid = HttpContext.Session.GetString("Username");
			string? requestType = string.Empty;
			string[] strId = leaveId?.Split(',') ?? Array.Empty<string>();

			if (strId.Length > 0 && !string.IsNullOrEmpty(strId[0]))
			{
				leaveId = strId[0];
			}
			if (strId.Length > 1 && !string.IsNullOrEmpty(strId[1]))
			{
				userid = strId[1];
			}
			if (strId.Length > 2 && !string.IsNullOrEmpty(strId[2]))
			{
				requestType = strId[2];
			}			
			try
			{
				///
				var loanItems = await GetLoanItems(userid);

				ViewBag.LoanUser = loanItems;

				var loanList = ViewBag.LoanUser as List<LoanItems>;



				using var client = _httpClientFactory.CreateClient();
				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for user {UserId}", userid);
					unifiedResponse.Message = "Failed to acquire access token.";
					return Ok(unifiedResponse);
				}

				var jsonOptions = new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true,
					WriteIndented = false
				};

				string apiUrl;
				if (requestType?.Trim() == TranTypes.BusinessTrip.Trim())
				{
					apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.ReadBusinessTripRequestUrl;
					var requestBody = new ReadBusinessTripRequest
					{
						readBusinessTripAPIContract = new readBusinessTripAPIContract
						{
							companyId = companycode,
							BusinessTripReqId = leaveId,
							UserId= userid
                        }
					};

					var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
					var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

					var response = await client.PostAsync(apiUrl, content);
					var responseContent = await response.Content.ReadAsStringAsync();

					if (response.IsSuccessStatusCode)
					{
						try
						{
							var businessTripDetails = JsonSerializer.Deserialize<List<BusinessTripDeatils>>(responseContent, jsonOptions);
							if (businessTripDetails == null || !businessTripDetails.Any())
							{
								_logger.LogWarning("No business trip details returned for BusinessTripReqId: {BusinessTripReqId}", leaveId);
								unifiedResponse.Message = "No business trip details found.";
								return Ok(unifiedResponse);
							}

							unifiedResponse.BusinessTripDetails = businessTripDetails;
							unifiedResponse.Success = true;

							if (businessTripDetails.Any() && businessTripDetails[0].AttachRefList != null)
							{
								var attachmentsJson = JsonSerializer.Serialize(businessTripDetails[0].AttachRefList);
								HttpContext.Session.SetString("attachments", attachmentsJson);
								ViewBag.Attachments = JsonSerializer.Deserialize<List<AttachRefItem>>(attachmentsJson);
							}
						}
						catch (JsonException ex)
						{
							_logger.LogError(ex, "Failed to deserialize JSON response for company {CompanyId}. Error at path: {Path}, Message: {Message}",
								companycode, ex.Path, ex.Message);
							unifiedResponse.Message = "Failed to deserialize business trip response.";
							return Ok(unifiedResponse);
						}
					}
					else
					{
						_logger.LogError("API call failed: {StatusCode}, {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
						unifiedResponse.Message = $"API call failed: {response.ReasonPhrase}";
						return Ok(unifiedResponse);
					}
				}
				else
				{
					apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.ReadLeaveRequestUrl;
					var requestBody = new ESSReadLeaveRequest
					{
						ReadLeaveRequest = new ReadLeaveRequestVm
						{
							companyCode = companycode,
							UserId = userid,
							LeaveId = leaveId
						}
					};

					var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
					var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

					var response = await client.PostAsync(apiUrl, content);
					var responseContent = await response.Content.ReadAsStringAsync();

					if (response.IsSuccessStatusCode)
					{
						try
						{
							var employeeResponse = JsonSerializer.Deserialize<ReadleaveResponeById>(responseContent, jsonOptions);
							if (employeeResponse == null || employeeResponse.LeaveDetails == null || !employeeResponse.LeaveDetails.Any())
							{
								_logger.LogWarning("No leave details returned for LeaveId: {LeaveId}", leaveId);
								unifiedResponse.Message = "No leave details found.";
								return Ok(unifiedResponse);
							}

							unifiedResponse.LeaveDetails = employeeResponse.LeaveDetails; // Assuming types match
							unifiedResponse.Success = true;
							unifiedResponse.loanList= ViewBag.LoanUser as List<LoanItems>;

							if (employeeResponse.LeaveDetails.Any() && employeeResponse.LeaveDetails[0].AttachRefList != null)
							{
								var attachmentsJson = JsonSerializer.Serialize(employeeResponse.LeaveDetails[0].AttachRefList);
								HttpContext.Session.SetString("attachments", attachmentsJson);
								ViewBag.Attachments = JsonSerializer.Deserialize<List<AttachRefItem>>(attachmentsJson);
							}
						}
						catch (JsonException ex)
						{
							_logger.LogError(ex, "Failed to deserialize JSON response for company {CompanyId}. Error at path: {Path}, Message: {Message}",
								companycode, ex.Path, ex.Message);
							unifiedResponse.Message = "Failed to deserialize leave response.";
							return Ok(unifiedResponse);
						}
					}
					else
					{
						_logger.LogError("API call failed: {StatusCode}, {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
						unifiedResponse.Message = $"API call failed: {response.ReasonPhrase}";
						return Ok(unifiedResponse);
					}
				}

				return Ok(unifiedResponse);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception while calling ReadManagerLevelRequest for user {UserId}", userid);
				unifiedResponse.Message = "An error occurred while processing the request.";
				return Ok(unifiedResponse);
			}
		}
		public async Task<IActionResult> ReadManagerLeaveResumption(string leaveId)
		{
			var unifiedResponse = new UnifiedLeaveResponse
			{
				BusinessTripDetails = new List<BusinessTripDeatils>(),
				LeaveDetails = new List<LeaveDetails>(),
				Success = false,
				loanList =new List<LoanItems>()
			};

			string companycode = HttpContext.Session.GetString("CompanyCode") ?? string.Empty;
			string? userid = HttpContext.Session.GetString("Username");
			string? requestType = string.Empty;
			string[] strId = leaveId?.Split(',') ?? Array.Empty<string>();

			if (strId.Length > 0 && !string.IsNullOrEmpty(strId[0]))
			{
				leaveId = strId[0];
			}
			if (strId.Length > 1 && !string.IsNullOrEmpty(strId[1]))
			{
				userid = strId[1];
			}
			if (strId.Length > 2 && !string.IsNullOrEmpty(strId[2]))
			{
				requestType = strId[2];
			}			
			try
			{
				///
				var loanItems = await GetLoanItems(userid);

				ViewBag.LoanUser = loanItems;

				var loanList = ViewBag.LoanUser as List<LoanItems>;



				using var client = _httpClientFactory.CreateClient();
				var token = await _authentications.GetAccessTokenAsync();
				if (string.IsNullOrEmpty(token))
				{
					_logger.LogError("Failed to acquire access token for user {UserId}", userid);
					unifiedResponse.Message = "Failed to acquire access token.";
					return Ok(unifiedResponse);
				}

				var jsonOptions = new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true,
					WriteIndented = false
				};

				string apiUrl;
				if (requestType?.Trim() == TranTypes.BusinessTrip.Trim())
				{
					apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.ReadBusinessTripRequestUrl;
					var requestBody = new ReadBusinessTripRequest
					{
						readBusinessTripAPIContract = new readBusinessTripAPIContract
						{
							companyId = companycode,
							BusinessTripReqId = leaveId,
							UserId= userid
                        }
					};

					var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
					var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

					var response = await client.PostAsync(apiUrl, content);
					var responseContent = await response.Content.ReadAsStringAsync();

					if (response.IsSuccessStatusCode)
					{
						try
						{
							var businessTripDetails = JsonSerializer.Deserialize<List<BusinessTripDeatils>>(responseContent, jsonOptions);
							if (businessTripDetails == null || !businessTripDetails.Any())
							{
								_logger.LogWarning("No business trip details returned for BusinessTripReqId: {BusinessTripReqId}", leaveId);
								unifiedResponse.Message = "No business trip details found.";
								return Ok(unifiedResponse);
							}

							unifiedResponse.BusinessTripDetails = businessTripDetails;
							unifiedResponse.Success = true;

							if (businessTripDetails.Any() && businessTripDetails[0].AttachRefList != null)
							{
								var attachmentsJson = JsonSerializer.Serialize(businessTripDetails[0].AttachRefList);
								HttpContext.Session.SetString("attachments", attachmentsJson);
								ViewBag.Attachments = JsonSerializer.Deserialize<List<AttachRefItem>>(attachmentsJson);
							}
						}
						catch (JsonException ex)
						{
							_logger.LogError(ex, "Failed to deserialize JSON response for company {CompanyId}. Error at path: {Path}, Message: {Message}",
								companycode, ex.Path, ex.Message);
							unifiedResponse.Message = "Failed to deserialize business trip response.";
							return Ok(unifiedResponse);
						}
					}
					else
					{
						_logger.LogError("API call failed: {StatusCode}, {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
						unifiedResponse.Message = $"API call failed: {response.ReasonPhrase}";
						return Ok(unifiedResponse);
					}
				}
				else
				{
					apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.ReadManagerLeaveResumption;
					var requestBody = new ESSReadLeaveResumptnMgnrRequest
					{
						ReadLeaveRequestResVm = new ReadLeaveResumptionVm
						{
							CompanyCode = companycode,
							search = "",
							leaveID = leaveId
						}
					};

					var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
					var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

					var response = await client.PostAsync(apiUrl, content);
					var responseContent = await response.Content.ReadAsStringAsync();

					if (response.IsSuccessStatusCode)
					{
						try
						{
							var employeeResponse = JsonSerializer.Deserialize<ReadleaveResponeById>(responseContent, jsonOptions);
							if (employeeResponse == null || employeeResponse.LeaveDetails == null || !employeeResponse.LeaveDetails.Any())
							{
								_logger.LogWarning("No leave details returned for LeaveId: {LeaveId}", leaveId);
								unifiedResponse.Message = "No leave details found.";
								return Ok(unifiedResponse);
							}

							unifiedResponse.LeaveDetails = employeeResponse.LeaveDetails; // Assuming types match
							unifiedResponse.Success = true;
							unifiedResponse.loanList= ViewBag.LoanUser as List<LoanItems>;

							if (employeeResponse.LeaveDetails.Any() && employeeResponse.LeaveDetails[0].AttachRefList != null)
							{
								var attachmentsJson = JsonSerializer.Serialize(employeeResponse.LeaveDetails[0].AttachRefList);
								HttpContext.Session.SetString("attachments", attachmentsJson);
								ViewBag.Attachments = JsonSerializer.Deserialize<List<AttachRefItem>>(attachmentsJson);
							}
						}
						catch (JsonException ex)
						{
							_logger.LogError(ex, "Failed to deserialize JSON response for company {CompanyId}. Error at path: {Path}, Message: {Message}",
								companycode, ex.Path, ex.Message);
							unifiedResponse.Message = "Failed to deserialize leave response.";
							return Ok(unifiedResponse);
						}
					}
					else
					{
						_logger.LogError("API call failed: {StatusCode}, {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
						unifiedResponse.Message = $"API call failed: {response.ReasonPhrase}";
						return Ok(unifiedResponse);
					}
				}

				return Ok(unifiedResponse);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception while calling ReadManagerLevelRequest for user {UserId}", userid);
				unifiedResponse.Message = "An error occurred while processing the request.";
				return Ok(unifiedResponse);
			}
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<BusinessTripResponse?> SubmiteWorkFlow([FromBody] ApprovalWorkFlow input)
        {
            string? username = HttpContext.Session.GetString("Username");
            ViewBag.Username = username;
            string? companyId = HttpContext.Session.GetString("CompanyCode");

            // Validate session values
            if (string.IsNullOrEmpty(companyId) || string.IsNullOrEmpty(username))
            {
                _logger.LogError("Missing CompanyCode or Username in session for user {UserId}", username);
                return new BusinessTripResponse { Message = "Session data is missing." };
            }

            try
            {
                using var client = _httpClientFactory.CreateClient();

                var token = await _authentications.GetAccessTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("Failed to acquire access token for company {CompanyId}", companyId);
                    return new BusinessTripResponse { Message = "Failed to authenticate." };
                }
                if (input.workFlowValue == TranTypes.Approve.Trim())
                {
                    var apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.ApproveWorkflowUrl;

                    var requestBody = new ApprovalWorkFlowResponse
                    {
                        ContractWorkFlow = new ApprovalWorkFlow
                        {
                            CompanyId = companyId,
                            TableId = input != null ? input.TableId : null,
                            RecordId = input != null ? input.RecordId : 0,
                            Comments = input != null ? input.Comments : null,
                            SpecialApproval = input != null ? input.SpecialApproval : null,
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

                    _logger.LogInformation("submite work flow response: {StatusCode}", response.StatusCode);

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
                else if (input.workFlowValue == TranTypes.Rejected.Trim())
                {
                    var apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.RejectWorkflowUrl;

                    var requestBody = new RejectWorkFlowResponse
                    {
                        Contract = new RejectWorkFlow
                        {
                            CompanyId = companyId,
                            TableId = input != null ? input.TableId : null,
                            RecordId = input != null ? input.RecordId : 0,
                            Comments = input != null ? input.Comments : null,
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

                    _logger.LogInformation("submite work flow response: {StatusCode}", response.StatusCode);

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
                else if (input.workFlowValue == TranTypes.Delegation.Trim())
                {
                    return new BusinessTripResponse { Message = $"API call failed:" };
                }
                else
                {
                    return new BusinessTripResponse { Message = $"API call failed:" };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while creating Business request for user {UserId}", username);
                return new BusinessTripResponse { Message = "An error occurred while processing the request." };
            }
        }
    }
}
