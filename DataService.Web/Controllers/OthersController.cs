using Microsoft.AspNetCore.Mvc;

namespace DataService.Web.Controllers
{
    public class OthersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
