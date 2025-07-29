using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace SMARTHR.Controllers
{
    public class BaseController : Controller
    {
		//private readonly IMenuItemService _menuItemService;IMenuItemService menuItemService

		public BaseController()
        {
            //_menuItemService = menuItemService;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Skip menu fetching for redirects
            if (context.Result is RedirectToActionResult || context.Result is RedirectResult)
            {
                await next();
                return;
            }

            try
            {
                // Cache menu items in HttpContext.Items
                if (!HttpContext.Items.ContainsKey("MenuItems"))
                {
                    //HttpContext.Items["MenuItems"] = await _menuItemService.GetMenuItemsFromD365Async();
                }
                ViewBag.MenuItems = HttpContext.Items["MenuItems"];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching menu items: {ex.Message}");
                //ViewBag.MenuItems = new List<SmartHR.Application.DTOs.MenuItemDTOs>();
            }

            await next();
        }
    }

}