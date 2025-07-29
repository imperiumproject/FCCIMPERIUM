using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMARTHR.Models;
using SMARTHR.WEB.Models;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SMARTHR.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;
		//private readonly IMenuItemService _menuServices;	IMenuItemService menuItemService : base(menuItemService)	
		public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)            
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            //_menuServices = menuItemService;			
        }

        [Authorize]
        public IActionResult Index()
        {
            //if (User?.Identity?.IsAuthenticated == true)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            _logger.LogDebug("HomeController.Index: Rendering dashboard for authenticated user.");
            return View(); // Render the dashboard view
        }

        public IActionResult Privacy()
        {
            _logger.LogDebug("HomeController.Privacy: Rendering privacy page.");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.LogDebug("HomeController.Error: Rendering error page.");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }		
	}
}
