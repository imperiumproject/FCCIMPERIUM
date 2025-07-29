using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SMARTHR.WEB.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using NuGet.Protocol.Plugins;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using SMARTHR.WEB.Models.SMARTHR.WEB.Models;
using SMARTHR.WEB.FCCHRServices.FCCHRBusinessInterface;
using SMARTHR.WEB.FCCHRServices.D365;

namespace SMARTHR.WEB.Controllers
{
    public class AuthenticationController : Controller
    {
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _configuration;
		private readonly ILogger<AuthenticationController> _logger;
		private readonly IAuthentications _authentications;		
		public AuthenticationController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<AuthenticationController> logger,
			IAuthentications authentications)
		{
			_httpClientFactory = httpClientFactory;
			_configuration = configuration;
			_logger = logger;
			_authentications = authentications;			
		}
		
		[HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]	
		public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = "Home/Index")
		{
			if (ModelState.IsValid)
			{
				try
				{
					// Create HttpClient
					using var client = _httpClientFactory.CreateClient();

					// Step 1: Acquire access token
					var token = await _authentications.GetAccessTokenAsync();
					string? cmpcode = _configuration["AppSettings:CompanyCode"];
					if (string.IsNullOrEmpty(token))
					{
						_logger.LogError("Failed to acquire access token for user {Username}", model.Username);
						ModelState.AddModelError(string.Empty, "Failed to authenticate with the server.");
						return View(model);
					}

					// Step 2: Prepare login request
					var apiUrl = _configuration["D365:BaseUrl"] + D365ApiURL.LoginUrl;

					var requestBody = new EssLoginRequest
					{
						EssLoginContract = new EssLoginContract
						{
							UserID = model.Username,
							Password = model.Password,
							CompanyCode = cmpcode
						}
					};

					//var jsonOptions = new JsonSerializerOptions
					//{
					//	PropertyNamingPolicy = null,
					//	WriteIndented = false
					//};
					var jsonOptions = _authentications.GetJsonOptions();
					var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
					_logger.LogInformation("Sending request body for user {Username}: {RequestBody}", model.Username, jsonString);

					var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

					// Step 3: Make API call
					var response = await client.PostAsync(apiUrl, content);

					if (response.IsSuccessStatusCode)
					{
						var responseContent = await response.Content.ReadAsStringAsync();
						_logger.LogInformation("API response for user {Username}: {Response}", model.Username, responseContent);

						LoginResponse? result;
						try
						{
							result = JsonSerializer.Deserialize<LoginResponse>(responseContent, jsonOptions);

							//String UserName = HttpContext.Session.GetString("Username");
                        }
						catch (JsonException ex)
						{
							_logger.LogError(ex, "Failed to deserialize API response for user {Username}: {ResponseContent}", model.Username, responseContent);
							ModelState.AddModelError(string.Empty, "Invalid response from server.");
							return View(model);
						}

						if (result?.Success == true)
						{							
							var claims = new List<Claim>
							{
								new Claim(ClaimTypes.Name, model.Username ?? string.Empty),
								new Claim(ClaimTypes.Role, "User"),
								new Claim("CompanyCode",cmpcode!=null?cmpcode:"")
							};

							var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
							var authProperties = new AuthenticationProperties
							{
								IsPersistent = model.RememberMe
							};

							await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
								new ClaimsPrincipal(claimsIdentity), authProperties);

							_logger.LogInformation("User {Username} logged in successfully", model.Username);

                            HttpContext.Session.SetString("Username", model.Username??"");
                            HttpContext.Session.SetString("CompanyCode", cmpcode??"");
                          //  HttpContext.Session.SetString("EmployeeName", result.username);


                            var profile = new ProfileViewModel { UserId = model.Username ?? "", CompanyId = cmpcode??"" };
                            HttpContext.Session.SetString("Profile", JsonSerializer.Serialize(profile));

                            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
							{
								return Redirect(returnUrl);
							}
							return RedirectToAction("Index", "Home");
						}
						else
						{
							var errorMessage = result?.ErrorMessage ?? "Invalid username, password, or company code.";
							_logger.LogWarning("Login failed for user {Username}: {ErrorMessage}", model.Username, errorMessage);
							ModelState.AddModelError(string.Empty, errorMessage);
						}
					}
					else
					{
						var errorContent = await response.Content.ReadAsStringAsync();
						_logger.LogError("API call failed for user {Username}: {StatusCode}, {ReasonPhrase}, {ErrorContent}",
							model.Username, response.StatusCode, response.ReasonPhrase, errorContent);
						ModelState.AddModelError(string.Empty, $"Authentication failed: {response.ReasonPhrase}");
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Login failed for user {Username}", model.Username);
					ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
				}
			}

			return View(model);
		}		

		[HttpGet]
        [AllowAnonymous] 
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous] 
        public IActionResult ForgotPassword()
        {
            return View();
        }
        public async Task<IActionResult> ChangePassword(ForgetPassword forget, string returnUrl = "Home/Index")
		{
			if (ModelState.IsValid)
			{

				try
				{
					using var client = _httpClientFactory.CreateClient();
					string? cmpcode = _configuration["AppSettings:CompanyCode"];
					// Step 1: Acquire access token
					var token = await _authentications.GetAccessTokenAsync();
                    if (string.IsNullOrEmpty(token))
					{
						_logger.LogError("Failed to acquire access token for user {UserName}", forget.UserName);
						ModelState.AddModelError(string.Empty, "Failed to authenticate with the server.");
						return View(forget);
					}

					// Step 2: Prepare request
					var apiUrl = _configuration["D365:BaseUrl"] + D365ApiURL.ForgotPasswordverifyUrl;

					DateTime dob;
					if (!DateTime.TryParse(forget.DateOfBirth, out dob))
					{
						ModelState.AddModelError("DateOfBirth", "Invalid date format.");
						return View(forget);
					}



					var requestBody = new serviceContractRequest
					{
						serviceContract = new serviceContract
						{
							UserID = forget.UserName,
							Password = dob.ToString("dd/MM/yyyy"),
							CompanyCode = cmpcode
						}
					};

					var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = null };
					var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
					_logger.LogInformation("Request body for user {UserName}: {RequestBody}", forget.UserName, jsonString);

					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

					var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

					// Step 3: Send request
					var response = await client.PostAsync(apiUrl, content);

					var responseContent = await response.Content.ReadAsStringAsync();
					_logger.LogInformation("Response for user {Username}: {Response}", forget.UserName, responseContent);

					if (!response.IsSuccessStatusCode)
					{
						_logger.LogError("API call failed for user {UserId}: {StatusCode}, {ReasonPhrase}, {ErrorContent}",
							forget.UserName, response.StatusCode, response.ReasonPhrase, responseContent);
						ModelState.AddModelError(string.Empty, $"Authentication failed: {response.ReasonPhrase}");
						return View(forget);
					}

					var result = JsonSerializer.Deserialize<ForgetPasswordResponse>(responseContent, jsonOptions);
					if (result?.MessageFlag == "Success")
					{
						var errorMessage = result?.Message ?? "Password reset instructions sent successfully.";
						ModelState.AddModelError(string.Empty, errorMessage);
						return RedirectToAction("ForgotPassword", "Authentication");
					}
					else
					{
						var errorMessage = result?.Message ?? "Invalid UserId, password, or company code.";
						_logger.LogWarning("Forgot password failed for user {UserName}: {ErrorMessage}", forget.UserName, errorMessage);
						ModelState.AddModelError(string.Empty, errorMessage);
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Forgot password failed for user {UserId}", forget.UserName);
					ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
				}
			}
			return View(forget);
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM change)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Create HttpClient
                    using var client = _httpClientFactory.CreateClient();
					string? cmpcode = _configuration["AppSettings:CompanyCode"];
					// Step 1: Acquire access token
					var token = await _authentications.GetAccessTokenAsync();
                    if (string.IsNullOrEmpty(token))
                    {
                        _logger.LogError("Failed to acquire access token for user {UserName}", change.UserName);
                        ModelState.AddModelError(string.Empty, "Failed to authenticate with the server.");
                        return View(change);
                    }

                    // Step 2: Prepare password change request
                    var apiUrl = _configuration["D365:BaseUrl"] + D365ApiURL.Updatepassword;

                    var requestBody = new ChangePasswordRequest
                    {
                        ChangePasswordContract = new ChangePasswordContract
                        {
                            UserID = change.UserName,
                            Password = change.Password,
                            ConfirmPassword = change.Password,
                            CompanyCode = cmpcode
						}
                    };

					//var jsonOptions = new JsonSerializerOptions
					//{
					//    PropertyNamingPolicy = null,
					//    WriteIndented = false
					//};
					var jsonOptions = _authentications.GetJsonOptions();
					var jsonString = JsonSerializer.Serialize(requestBody, jsonOptions);
                    _logger.LogInformation("Sending request body for user {UserName}: {RequestBody}", change.UserName, jsonString);

                    var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // Step 3: Make API call
                    var response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        _logger.LogInformation("API response for user {UserName}: {Response}", change.UserName, responseContent);

                        ChangePasswordResponse? result;
                        try
                        {
                            result = JsonSerializer.Deserialize<ChangePasswordResponse>(responseContent, jsonOptions);
                        }
                        catch (JsonException ex)
                        {
                            _logger.LogError(ex, "Failed to deserialize API response for user {UserName}: {ResponseContent}", change.UserName, responseContent);
                            ModelState.AddModelError(string.Empty, "Invalid response from server.");
                            return View(change);
                        }

                        if (result?.MessageFlag == "Success")
                        {
                            string errorMsg = result?.Message ?? "Password changed successfully.";
                            ModelState.AddModelError(string.Empty, errorMsg);
                            return RedirectToAction("Login", "Authentication");
                        }
                        else
                        {
                            var errorMessage = result?.Message ?? "Password change failed. Please try again.";
                            _logger.LogWarning("Password change failed for user {UserName}: {ErrorMessage}", change.UserName, errorMessage);
                            ModelState.AddModelError(string.Empty, errorMessage);
                        }
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogError("API call failed for user {UserName}: {StatusCode}, {ReasonPhrase}, {ErrorContent}",
                            change.UserName, response.StatusCode, response.ReasonPhrase, errorContent);
                        ModelState.AddModelError(string.Empty, $"Authentication failed: {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Password change failed for user {UserName}", change.UserName);
                    ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                }
            }

            return View(change);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Authentication");
        }

    }
}
