using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
//using SmartHR.Application.Interfaces;
using SMARTHR.WEB.Models;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using System.Data.SqlTypes;
using SMARTHR.WEB.FCCHRServices.FCCHRBusinessInterface;
using SMARTHR.WEB.FCCHRServices.D365;

namespace SMARTHR.WEB.Controllers
{
    public class ProfileController : Controller
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProfileController> _logger;
        private readonly IAuthentications _authentications;
        public ProfileController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<ProfileController> logger, IAuthentications authentications)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
            _authentications = authentications;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyProfile()
        {
            // Initialize the view model for errors
            var model = new MyProfileViewModel();

            // Check session for Profile data
            var profileJson = HttpContext.Session.GetString("Profile");
            ProfileViewModel? profile = null;

            if (string.IsNullOrEmpty(profileJson))
            {
                _logger.LogWarning("Session expired or Profile data missing. Redirecting to login.");
				await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
				return RedirectToAction("Login", "Authentication");				
            }

            try
            {
                profile = JsonSerializer.Deserialize<ProfileViewModel>(profileJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (profile == null || string.IsNullOrEmpty(profile.UserId))
                {
                    _logger.LogWarning("Invalid or missing UserId in session Profile data. Redirecting to login.");

					await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
					return RedirectToAction("Login", "Authentication");
				}
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize session Profile data.");
				await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
				return RedirectToAction("Login", "Authentication");
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
                    ModelState.AddModelError(string.Empty, "Failed to authenticate with the server.");
                    ViewBag.Profile = profile;
                    return View(model);
                }

                // Step 2: Prepare API request
                var apiUrl = $"{_configuration["D365:BaseUrl"]}"+D365ApiURL.GetEmpProfileUrl;
                var requestBody = new EssViewProfileRequest
                {
                    EssProfileContract = new ProfileViewModel
                    {
                        UserId = profile.UserId,
                        CompanyId = profile.CompanyId ?? "USMF"
                    }
                };

				//var jsonOptions = new JsonSerializerOptions
				//{
				//    PropertyNamingPolicy = null,
				//    WriteIndented = true
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
                        ModelState.AddModelError(string.Empty, "Empty response from server.");
                        ViewBag.Profile = profile;

                        return View(model);
                    }

                    _logger.LogInformation("API response for user {UserId}: {Response}", profile.UserId, responseContent);

                    try
                    {
                        var result = JsonSerializer.Deserialize<MyProfileViewModel>(responseContent, jsonOptions);
                        if (result == null)
                        {
                            _logger.LogError("Deserialized MyProfileViewModel is null for user {UserId}", profile.UserId);
                            ModelState.AddModelError(string.Empty, "Invalid response from server.");
                            ViewBag.Profile = profile;
                            return View(model);
                        }
                        HttpContext.Session.SetString("EmployeeName", result.EmployeeName ?? "");
                        HttpContext.Session.SetString("Email", model.Email ?? "");
                        return View(result);
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "Failed to deserialize API response for user {UserId}: {ResponseContent}", profile.UserId, responseContent);
                        ModelState.AddModelError(string.Empty, "Invalid response format from server.");
                        ViewBag.Profile = profile;
                        return View(model);
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("API call failed for user {UserId}: {StatusCode}, {ReasonPhrase}, {ErrorContent}",
                        profile.UserId, response.StatusCode, response.ReasonPhrase, errorContent);
                    ModelState.AddModelError(string.Empty, $"Authentication failed: {response.ReasonPhrase}");
                    ViewBag.Profile = profile;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Profile retrieval failed for user {UserId}", profile?.UserId ?? "Unknown");
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                ViewBag.Profile = profile;
                return View(model);
            }
        }
    }
}
