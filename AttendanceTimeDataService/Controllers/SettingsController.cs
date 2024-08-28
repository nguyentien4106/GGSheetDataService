using Microsoft.AspNetCore.Mvc;

namespace DataService.Web.Controllers
{
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
