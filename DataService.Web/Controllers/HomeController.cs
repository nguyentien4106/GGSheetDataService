using DataService.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using DataService.Web.Models.Settings;
using DataService.Web.Helper;
using DataService.Web.Services;
using DataService.Web.ViewModel.Home;
namespace DataService.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SDKHelper _sdk;
        private AppSettingsService _settingsService;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _sdk = new SDKHelper();
            _settingsService = new AppSettingsService("C:\\appsettings.json");
        }

        public IActionResult Index()
        {
            var status = _sdk.GetConnectState();
            ViewBag.ConnectState = status;
            var devices = _settingsService.GetCurrentDevicesSettings();
            var model = new IndexViewModel()
            {
                Devices = devices,
                
            };
            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Connect(Device model)
        {
            var result = _sdk.sta_ConnectTCP(model);
            return Ok();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
