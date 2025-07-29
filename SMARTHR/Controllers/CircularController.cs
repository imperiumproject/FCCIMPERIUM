using Microsoft.AspNetCore.Mvc;
using SMARTHR.WEB.FCCHRServices.D365;
using SMARTHR.WEB.FCCHRServices.FCCHRBusinessInterface;
using SMARTHR.WEB.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SMARTHR.WEB.Controllers
{
    public class CircularController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LeaveController> _logger;
        private readonly IAuthentications _authentications;
        public CircularController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<LeaveController> logger,
            IAuthentications authentications)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
            _authentications = authentications;
         
        }


        public async Task<IActionResult> Circular(circularList input)
        {
            var response = await ReadCircularList(input);
            var model = response?.HrCircularDetails ?? new List<SMARTHR.WEB.Models.HrCircularDetail>();
            return View(model);
           
        }

        
        [HttpPost]
        public async Task<CircularResponse> ReadCircularList(circularList input)
        {
            var fileres = new CircularResponse();
            string? companyId = HttpContext.Session.GetString("CompanyCode");
            string? username = HttpContext.Session.GetString("Username");
            try
            {
                using var client = _httpClientFactory.CreateClient();
                var token = await _authentications.GetAccessTokenAsync();

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("Failed to acquire access token for company {CompanyId}", companyId);
                    return fileres;
                }

                var apiUrl = $"{_configuration["D365:BaseUrl"]}" + D365ApiURL.ReadUpdCircularListUrl;


                var requestBody = new circularListRequest
                {
                    circularList = new circularList
                    {
                        CompanyCode = companyId,
                        search = input.search ?? "",
                        hrCircularRecID = 0,
                        //empId = input.empId ??""

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
                    var fileResponse = JsonSerializer.Deserialize<CircularResponse>(responseContent, jsonOptions);
                    fileres = fileResponse ?? new CircularResponse();
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

            return fileres;
        }

        [HttpPost]
        public IActionResult ViewAttachment(string base64, string fileName)
        {
            if (string.IsNullOrEmpty(base64) || string.IsNullOrEmpty(fileName))
                return BadRequest("Invalid data.");

            try
            {
                var fileBytes = Convert.FromBase64String(base64);
                var mimeType = GetMimeType(fileName);

                Response.Headers.Add("Content-Disposition", $"inline; filename={fileName}");
                return File(fileBytes, mimeType);
            }
            catch
            {
                return BadRequest("Failed to decode base64 or file content is invalid.");
            }
        }

        private string GetMimeType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();

            return ext switch
            {
                // Documents
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".ppt" => "application/vnd.ms-powerpoint",
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ".txt" => "text/plain",
                ".rtf" => "application/rtf",
                ".csv" => "text/csv",
                ".xml" => "application/xml",
                ".json" => "application/json",

                // Images
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".svg" => "image/svg+xml",
                ".webp" => "image/webp",
                ".tif" or ".tiff" => "image/tiff",

                // Audio
                ".mp3" => "audio/mpeg",
                ".wav" => "audio/wav",
                ".ogg" => "audio/ogg",
                ".m4a" => "audio/mp4",

                // Video
                ".mp4" => "video/mp4",
                ".avi" => "video/x-msvideo",
                ".mov" => "video/quicktime",
                ".wmv" => "video/x-ms-wmv",
                ".webm" => "video/webm",
                ".mkv" => "video/x-matroska",

                // Archives
                ".zip" => "application/zip",
                ".rar" => "application/vnd.rar",
                ".7z" => "application/x-7z-compressed",
                ".tar" => "application/x-tar",
                ".gz" => "application/gzip",

                // Code
                ".html" => "text/html",
                ".css" => "text/css",
                ".js" => "application/javascript",
                ".cs" => "text/plain",
                ".jsonld" => "application/ld+json",

                // Fallback
                _ => "application/octet-stream"
            };
        }



        //[HttpPost]
        //public async Task<CircularResponse> ReadCircularAttachmentList(circularAttachmentList input)
        //{
        //    var fileres = new CircularResponse();
        //    string? companyId = HttpContext.Session.GetString("CompanyCode");
        //    string? username = HttpContext.Session.GetString("Username");
        //    try
        //    {
        //        using var client = _httpClientFactory.CreateClient();
        //        var token = await _authentications.GetAccessTokenAsync();

        //        if (string.IsNullOrEmpty(token))
        //        {
        //            _logger.LogError("Failed to acquire access token for company {CompanyId}", companyId);
        //            return fileres;
        //        }

        //        var apiUrl = $"{_configuration["D365:BaseUrl"]}" + D365ApiURL.ReadCircularAttachmentList;


        //        var requestBody = new circularAttachmentListRequest
        //        {
        //            circularAttachmentList = new circularAttachmentList
        //            {
        //                companyCode = companyId,
        //                uniqueID = input.uniqueID ?? "",
        //                requestRecID = input.requestRecID??"",
        //                attachRecID = input.attachRecID ??""

        //            }
        //        };


        //        var jsonOptions = _authentications.GetJsonOptions();
        //        var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
        //        var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //        var response = await client.PostAsync(apiUrl, content);
        //        var responseContent = await response.Content.ReadAsStringAsync();

        //        _logger.LogInformation("createAttachFile API Raw JSON: {Json}", responseContent);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var fileResponse = JsonSerializer.Deserialize<CircularResponse>(responseContent, jsonOptions);
        //            fileres = fileResponse ?? new CircularResponse();
        //        }
        //        else
        //        {
        //            _logger.LogError("API call failed: {StatusCode}, {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Exception while calling ReadCircularAttachmentList for company {CompanyId}", companyId);
        //    }

        //    return fileres;
        //}

        //[HttpPost]
        //public async Task<CircularResponse> UpdateCircularCount(circularList input)
        //{
        //    var fileres = new CircularResponse();
        //    string? companyId = HttpContext.Session.GetString("CompanyCode");
        //    string? username = HttpContext.Session.GetString("Username");
        //    try
        //    {
        //        using var client = _httpClientFactory.CreateClient();
        //        var token = await _authentications.GetAccessTokenAsync();

        //        if (string.IsNullOrEmpty(token))
        //        {
        //            _logger.LogError("Failed to acquire access token for company {CompanyId}", companyId);
        //            return fileres;
        //        }

        //        var apiUrl = $"{_configuration["D365:BaseUrl"]}" + D365ApiURL.UpdateCircularCount;


        //        var requestBody = new circularListRequest
        //        {
        //            circularList = new circularList
        //            {
        //                CompanyCode = companyId,
        //                search = input.search ?? "",
        //                hrCircularRecID = input.hrCircularRecID ?? null,
        //                empId = input.empId ??""

        //            }
        //        };


        //        var jsonOptions = _authentications.GetJsonOptions();
        //        var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
        //        var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //        var response = await client.PostAsync(apiUrl, content);
        //        var responseContent = await response.Content.ReadAsStringAsync();

        //        _logger.LogInformation("createAttachFile API Raw JSON: {Json}", responseContent);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var fileResponse = JsonSerializer.Deserialize<CircularResponse>(responseContent, jsonOptions);
        //            fileres = fileResponse ?? new CircularResponse();
        //        }
        //        else
        //        {
        //            _logger.LogError("API call failed: {StatusCode}, {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Exception while calling UpdateCircularCount for company {CompanyId}", companyId);
        //    }

        //    return fileres;
        //}



        public  IActionResult DocumentDownload(circularAttachmentList input,circularList list)
        {
            // var res = await ReadCircularAttachmentList(input);
            //var res = await UpdateCircularCount(list);
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SalaryRevision()
        {
            return View();
        }
    }
}
