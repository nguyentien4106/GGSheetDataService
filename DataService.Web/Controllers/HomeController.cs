using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
namespace DataService.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;


        public HomeController(ILogger<HomeController> logger)
        {

        }

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

    }
}
