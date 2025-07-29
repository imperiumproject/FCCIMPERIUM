using Microsoft.AspNetCore.Mvc;
using SMARTHR.WEB.FCCHRServices.FCCHRBusinessInterface;
using SMARTHR.WEB.Models;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;

namespace SMARTHR.WEB.Controllers
{
    public class CertificateRequestController : Controller
    {
        //private readonly ILeaveService _leaveService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CertificateRequest> _logger;
        private readonly IAuthentications _authentications;

        public CertificateRequestController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<CertificateRequest> logger,
            IAuthentications authentications)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
            _authentications = authentications;
            //_leaveService = leaveService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<string> GetCertificateId()
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

                var apiUrl = $"{_configuration["D365:BaseUrl"]}/FCC_ESSIntgerationServiceGroup/FCC_ESSCertificateRequestService/getCertificateRequestId";

                var requestBody = new LeaveNumIdRequest
                {
                    contract = new ContractVM
                    {
                        companyId = companyid
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

            return result;
        }

        public async Task<IActionResult> CertificateRequest()
        {
            var response = await GetCertificateInformationAsync();
            var model = response?.CertificateDetails ?? new List<SMARTHR.WEB.Models.CertificateDetails>();
            return View(model);
        }

        private async Task<ReadCertificateInfoRespone> GetCertificateInformationAsync()
        {
            var certificateInfo = new ReadCertificateInfoRespone();
            string? companyId = HttpContext.Session.GetString("CompanyCode");
            string? userId = HttpContext.Session.GetString("Username");
            ViewBag.Username = userId;

            try
            {
                var client = _httpClientFactory.CreateClient();
                var token = await _authentications.GetAccessTokenAsync().ConfigureAwait(false);

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("Failed to acquire access token for company {CompanyId}", companyId);
                    return certificateInfo;
                }

                var apiUrl = $"{_configuration["D365:BaseUrl"]}/FCC_ESSIntgerationServiceGroup/FCC_ESSCertificateRequestService/readUpdCertificateList";

                var requestBody = new ReadCertificateRequestInfo
                {
                    certificateRequestList = new certificateRequestList
                    {
                        companyId = companyId,
                        userID = userId,
                        CertificateRequestId = string.Empty
                    }
                };

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = null,
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };

                var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsync(apiUrl, content).ConfigureAwait(false);
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                _logger.LogInformation("Certificate API Response JSON: {Json}", responseContent);

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var employeeResponse = JsonSerializer.Deserialize<ReadCertificateInfoRespone>(responseContent, jsonOptions);
                        certificateInfo = employeeResponse ?? new ReadCertificateInfoRespone();
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex,
                            "Failed to deserialize JSON response for company {CompanyId}. Message: {Message}",
                            companyId, ex.Message);
                    }
                }
                else
                {
                    _logger.LogError("API call failed. StatusCode: {StatusCode}, Reason: {ReasonPhrase}",
                        response.StatusCode, response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while retrieving certificate information for company {CompanyId}", companyId);
            }

            return certificateInfo;
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

                var apiUrl = $"{_configuration["D365:BaseUrl"]}/FCC_ESSIntgerationServiceGroup/FCC_ESSLoginService/getAllEmployees";

                var requestBody = new LeaveNumIdRequest
                {
                    contract = new ContractVM
                    {
                        companyId = companyid
                    }
                };

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = null,
                    PropertyNameCaseInsensitive = true
                };

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
        public async Task<certificateTypes> GetAllCertificate()
        {
            var employees = new certificateTypes();
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

                var apiUrl = $"{_configuration["D365:BaseUrl"]}/FCC_ESSIntgerationServiceGroup/FCC_ESSCertificateRequestService/getCertificateTypes";

                var requestBody = new LeaveNumIdRequestC
                {
                    contract = new ContractVMC
                    {
                        companyCode = companyid
                    }
                };

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = null,
                    PropertyNameCaseInsensitive = true
                };

                var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsync(apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Employee API Raw JSON: {Json}", responseContent);

                if (response.IsSuccessStatusCode)
                {
                    var certificateResponse = JsonSerializer.Deserialize<certificateTypes>(responseContent, jsonOptions);
                    employees = certificateResponse ?? new certificateTypes();
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
        public async Task<CertificateRequestViewModel?> CreateCertificateRequest([FromBody] CreateCertificateRequest input)
        {

            string? companyId = HttpContext.Session.GetString("CompanyCode");
            string? userId = HttpContext.Session.GetString("Username");
            ViewBag.Username = userId;
            input.userID = input.userID.Split('-')[0].Trim();

            // Validate session values
            if (string.IsNullOrEmpty(companyId) || string.IsNullOrEmpty(userId))
            {
                _logger.LogError("Missing CompanyCode or Username in session for user {UserId}", userId);
                return new CertificateRequestViewModel { Message = "Session data is missing." };
            }

            try
            {
                // Validate input
                if (input == null || string.IsNullOrEmpty(input.userID) || input.CertificateRequestId == null)
                {
                    _logger.LogError("Invalid input data for leave request by user {UserId}", userId);
                    return new CertificateRequestViewModel { Message = "Invalid input data." };
                }

                // Parse dates
                if (!_authentications.TryParseDate(input.transDate, out string startDate))
                {
                    _logger.LogError("Invalid date format for LeaveStart: {LeaveStart}", input.transDate);
                    return new CertificateRequestViewModel { Message = $"Invalid date format for LeaveStart: {input.transDate}" };
                }

                //if (!_authentications.TryParseDate(input.LeaveEnd, out string endDate))
                //{
                //    _logger.LogError("Invalid date format for LeaveEnd: {LeaveEnd}", input.LeaveEnd);
                //    return new CertificateRequestViewModel { Message = $"Invalid date format for LeaveEnd: {input.LeaveEnd}" };
                //}

                using var client = _httpClientFactory.CreateClient();

                var token = await _authentications.GetAccessTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("Failed to acquire access token for company {CompanyId}", companyId);
                    return new CertificateRequestViewModel { Message = "Failed to authenticate." };
                }
                string apiUrl;

                apiUrl = $"{_configuration["D365:BaseUrl"]}/FCC_ESSIntgerationServiceGroup/FCC_ESSCertificateRequestService/CertificateInfoCreation";



                //var jsonStringSession = HttpContext.Session.GetString("LeaveRequestBody");

                //if (!string.IsNullOrEmpty(jsonStringSession))
                //{
                //    //input.attachRefList = JsonSerializer.Deserialize<List<AttachmentRef>>(jsonStringSession);
                //    var wrapper = JsonSerializer.Deserialize<LeaveAttachWrapper>(jsonStringSession);
                //    if (wrapper?.attachRefList != null)
                //    {
                //        input.attachRefList ??= new List<AttachmentRef>();

                //        foreach (var item in wrapper.attachRefList)
                //        {
                //            input.attachRefList.Add(item);
                //        }
                //    }
                //}


                var requestBody = new CreateCertificateContractRequest
                {
                    certificateDetails = new CreateCertificateRequest
                    {
                        companyId = companyId,
                        userID = input.userID,
                        CertificateRequestId = input.CertificateRequestId,
                        transDate = Convert.ToString(startDate),
                        //de = Convert.ToString(startDate),
                        //LeaveEnd = Convert.ToString(endDate),
                        description = input.description,
                        type = input.type,
                        RaisedBy = userId,
                        wfComments = input.wfComments,
                        attachRefList = input.attachRefList

                    }
                };

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Match API's expected casing
                    PropertyNameCaseInsensitive = true
                };

                var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsync(apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("CreateLeaveRequest response: {StatusCode}", response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    var resultObj = JsonSerializer.Deserialize<CertificateRequestViewModel>(responseContent, jsonOptions);
                    return resultObj;
                }
                else
                {
                    _logger.LogError("API call failed: {StatusCode} - {ReasonPhrase} - {ResponseContent}",
                        response.StatusCode, response.ReasonPhrase, responseContent);
                    return new CertificateRequestViewModel { Message = $"API call failed: {response.ReasonPhrase}" };
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while creating leave request for user {UserId}", userId);
                return new CertificateRequestViewModel { Message = "An error occurred while processing the request." };
            }
        }



        public async Task<ReadCertificateResponeById> ReadCertificateRequest(string certificateid)
        {

            var certificate = new ReadCertificateResponeById();

            string companycode = HttpContext.Session.GetString("CompanyCode") ?? string.Empty;
            string? userid = HttpContext.Session.GetString("Username");
            ViewBag.Username = userid;

            try
            {
                using var client = _httpClientFactory.CreateClient();

                var token = await _authentications.GetAccessTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("Failed to acquire access token for user {UserId}", userid);
                    //return res; // or throw new Exception("Token failure");
                }

                var apiUrl = $"{_configuration["D365:BaseUrl"]}/FCC_ESSIntgerationServiceGroup/FCC_ESSCertificateRequestService/readUpdCertificateList";

                var requestBody = new ReadCertificateRequestInfo
                {
                    certificateRequestList = new certificateRequestList
                    {
                        companyId = companycode,
                        userID = userid,
                        CertificateRequestId = certificateid
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
                    try
                    {
                        // Log the value of noOfDays for leaveDetails[0]
                        using var document = JsonDocument.Parse(responseContent);

                        var employeeResponse = JsonSerializer.Deserialize<ReadCertificateResponeById>(responseContent, jsonOptions);
                        certificate = employeeResponse ?? new ReadCertificateResponeById();
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "Failed to deserialize JSON response for company {CompanyId}. Error at path: {Path}, Message: {Message}",
                            companycode, ex.Path, ex.Message);
                        return new ReadCertificateResponeById();
                    }


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

            return certificate;

        }

        public async Task<UpdateCertificate?> UpdateCertificateRequest([FromBody] UpdateCertificate input)
        {
            string? companyid = HttpContext.Session.GetString("CompanyCode");
            string? userid = HttpContext.Session.GetString("Username");
            ViewBag.Username = userid;

            try
            {


                using var client = _httpClientFactory.CreateClient();

                var token = await _authentications.GetAccessTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("Failed to acquire access token for company {CompanyId}", companyid);
                    return null;
                }

                var apiUrl = $"{_configuration["D365:BaseUrl"]}/FCC_ESSIntgerationServiceGroup/FCC_ESSCertificateRequestService/readUpdCertificateList";

                var requestBody = new ReadUpdateCertificateRequestInfo
                {
                    certificateRequestList = new UpdateCertificate
                    {
                        companyId = companyid,
                        requestType = "Update",
                        CertificateRequestId = input.CertificateRequestId,
                        description = input.description,
                        transDate = input.transDate,
                        //de = Convert.ToString(startDate),
                        //LeaveEnd = Convert.ToString(endDate),
                        type = input.type,
                        userID = userid,
                        wfComments = input.wfComments
                    }

                };
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = null,
                    PropertyNameCaseInsensitive = true
                };

                var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsync(apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("CreateLeaveRequest response: {response}", responseContent);

                if (response.IsSuccessStatusCode)
                {
                    var resultObj = JsonSerializer.Deserialize<UpdateCertificate>(responseContent, jsonOptions);
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


        public async Task<CertificateResponse?> DeleteCertificateRequest(string CertificateId)
        {
            string? companyid = HttpContext.Session.GetString("CompanyCode");
            string? userid = HttpContext.Session.GetString("Username");
            ViewBag.Username = userid;
            string CertificateRequestId = CertificateId;

            try
            {
                using var client = _httpClientFactory.CreateClient();

                var token = await _authentications.GetAccessTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("Failed to acquire access token for company {CompanyId}", companyid);
                    return null;
                }

                var apiUrl = $"{_configuration["D365:BaseUrl"]}/FCC_ESSIntgerationServiceGroup/FCC_ESSCertificateRequestService/deleteCertificateRequest";

                var requestBody = new DeleteCretificateResponseRequest
                {
                    certificateDetails = new DeleteContractRequestDetails
                    {
                        companyId = companyid,
                        CertificateRequestId = CertificateRequestId,

                    }
                };

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = null,
                    PropertyNameCaseInsensitive = true
                };

                var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsync(apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("CreateLeaveRequest response: {response}", responseContent);

                if (response.IsSuccessStatusCode)
                {
                    var resultObj = JsonSerializer.Deserialize<CertificateResponse>(responseContent, jsonOptions);
                    var msg = resultObj.Message;
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

                var apiUrl = $"{_configuration["D365:BaseUrl"]}/FCC_ESSIntgerationServiceGroup/FCC_ESSLeaveRequestService/createAttachFile";

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

                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = null,
                        PropertyNameCaseInsensitive = true
                    };

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

        public async Task<FileResponce> CreateBase64AttachFile([FromForm] FileUploadViewModel formData)
        {
            var fileres = new FileResponce();

            try
            {


                if (formData.Files != null && formData.Files.Count > 0)
                {
                    var attachList = new List<AttachmentRef>();


                    foreach (var file in formData.Files)
                    {
                        using var memoryStream = new MemoryStream();
                        await file.CopyToAsync(memoryStream);

                        string base64 = Convert.ToBase64String(memoryStream.ToArray());

                        attachList.Add(new AttachmentRef
                        {
                            fileName = file.FileName,
                            attachRef = base64
                        });
                        HttpContext.Session.SetString("fileName", file.FileName ?? "");
                        HttpContext.Session.SetString("attachRef", base64 ?? "");

                    }

                    var requestBody = new RootObjects
                    {
                        attachRefList = attachList
                    };


                    //var crdAttachFile = new CrdAttachFile
                    //{
                    //    attachRefList = attachList
                    //};

                    //var requestBody = new RootObjects
                    //{
                    //    CrdAttachFile = crdAttachFile
                    //};



                    //var crdAttachFile = new CrdAttachFile
                    //{
                    //    attachRefList = attachList
                    //};



                    //var requestBody = new RootObjects
                    //{
                    //    CrdAttachFile = crdAttachFile
                    //};

                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = null,
                        PropertyNameCaseInsensitive = true
                    };

                    var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
                    HttpContext.Session.SetString("LeaveRequestBody", jsonString);
                    // var jsonStringSession = HttpContext.Session.GetString("LeaveRequestBody");


                }
                else
                {
                    _logger.LogWarning("No files were uploaded for attach file.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while calling createAttachFile for company {CompanyId}");
            }

            return fileres;
        }

        public async Task<IActionResult> TrackRequestWorkFlow(long RecordId, int TableId, string TransType)
        {
            var leave = new WorkflowTrackingList();
            string? companyId = HttpContext.Session.GetString("CompanyCode");
            string? userId = HttpContext.Session.GetString("Username");
            ViewBag.Username = userId;

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

                var apiUrl = $"{_configuration["D365:BaseUrl"]}/FCC_ESSIntgerationServiceGroup/FCC_ESSWorkflowProcessService/readWFTrackingDetails";

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

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = null,
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };

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
                _logger.LogError(ex, "Exception while getting Leave Details for CompanyId: {CompanyId}, RecordId: {RecordId}", companyId, RecordId);
                return BadRequest(new { error = "An unexpected error occurred" });
            }

            return Ok(leave);
        }


        //public async Task<IActionResult> CertificateWorkFlow()
        //{
        //    var response = await GetCertificateWorkflowInformationAsync();
        //    var model = response?.CertificateDetails ?? new List<SMARTHR.WEB.Models.CertificateDetails>();
        //    return View(model);
        //}

    }
}
