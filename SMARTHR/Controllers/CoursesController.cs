using Microsoft.AspNetCore.Mvc;
using SMARTHR.WEB.FCCHRServices.D365;
using SMARTHR.WEB.FCCHRServices.FCCHRBusinessInterface;
using SMARTHR.WEB.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SMARTHR.WEB.Controllers
{
    public class CoursesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LeaveController> _logger;
        private readonly IAuthentications _authentications;
        public CoursesController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<LeaveController> logger,
            IAuthentications authentications)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
            _authentications = authentications;

        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Courses(string id)
        {
            // var res = await ReadCoursesInfo(id);
            var result = await ReadCoursesList();
            return View();
        }

        [HttpPost]
        public async Task<JobVacancyResponse> ReadCoursesList()
        {
            var jobs = new JobVacancyResponse();
            string? companyId = HttpContext.Session.GetString("CompanyCode");
            //string? username = HttpContext.Session.GetString("Username");
            try
            {
                using var client = _httpClientFactory.CreateClient();
                var token = await _authentications.GetAccessTokenAsync();

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("Failed to acquire access token for company {CompanyId}", companyId);
                    return jobs;
                }

                var apiUrl = $"{_configuration["D365:BaseUrl"]}" + D365ApiURL.ReadCoursesList;


                var requestBody = new ListcontractRequest
                {
                    Listcontract = new Listcontract
                    {
                        companyId = companyId

                    }
                };


                var jsonOptions = _authentications.GetJsonOptions();
                var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsync(apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("createAttachFile API Raw JSON: {Json}", responseContent);

                if (response.IsSuccessStatusCode)
                {
                    var jobresponse = JsonSerializer.Deserialize<JobVacancyResponse>(responseContent, jsonOptions);
                    jobs = jobresponse ?? new JobVacancyResponse();
                }
                else
                {
                    _logger.LogError("API call failed: {StatusCode}, {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while calling Courses List for company {CompanyId}", companyId);
            }

            return jobs;
        }


        [HttpPost]
        public async Task<JobVacancyResponse> ReadCoursesInfo(string description)
        {
            var res = new JobVacancyResponse();
            string? companyId = HttpContext.Session.GetString("CompanyCode");
            string? username = HttpContext.Session.GetString("Username");
            try
            {
                using var client = _httpClientFactory.CreateClient();
                var token = await _authentications.GetAccessTokenAsync();

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("Failed to acquire access token for company {CompanyId}", companyId);
                    return res;
                }

                var apiUrl = $"{_configuration["D365:BaseUrl"]}" + D365ApiURL.ReadCoursesInfo;


                var requestBody = new ListcontractRequest
                {
                    Listcontract = new Listcontract
                    {
                        description = description??""

                    }
                };


                var jsonOptions = _authentications.GetJsonOptions();
                var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsync(apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("createAttachFile API Raw JSON: {Json}", responseContent);

                if (response.IsSuccessStatusCode)
                {
                    var jresponse = JsonSerializer.Deserialize<JobVacancyResponse>(responseContent, jsonOptions);
                    res = jresponse ?? new JobVacancyResponse();
                }
                else
                {
                    _logger.LogError("API call failed: {StatusCode}, {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while calling circularList for company {CompanyId}", companyId);
            }

            return res;
        }

        [HttpPost]
        public async Task<IActionResult> CourseRegister(string Empid)
        {
            string? companyId = HttpContext.Session.GetString("CompanyCode");
            string? username = HttpContext.Session.GetString("Username");

            try
            {
                using var client = _httpClientFactory.CreateClient();
                var token = await _authentications.GetAccessTokenAsync();

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("Failed to acquire access token for company {CompanyId}", companyId);
                    return Json(new { success = false, message = "Access token could not be retrieved." });
                }

                var apiUrl = $"{_configuration["D365:BaseUrl"]}" + D365ApiURL.CourseRegister;

                var requestBody = new ListcontractRequest
                {
                    Listcontract = new Listcontract
                    {
                        employeeId = Empid

                    }
                };

                var jsonOptions = _authentications.GetJsonOptions();
                var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsync(apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("ApplyForJob API Raw JSON: {Json}", responseContent);

                if (response.IsSuccessStatusCode)
                {
                    var jresponse = JsonSerializer.Deserialize<JobApplicationResponse>(responseContent, jsonOptions);
                    var msg = jresponse?.Message ?? "Application submitted successfully!";
                    return Json(new { success = true, message = msg });
                }
                else
                {
                    _logger.LogError("API call failed: {StatusCode}, {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                    return Json(new { success = false, message = "Job application failed. Please try again." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while applying for job for company {CompanyId}", companyId);
                return Json(new { success = false, message = "An error occurred while processing the request." });
            }
        }
    }
}
