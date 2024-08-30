using DataService.Web.Helper;
using DataService.Web.Models.Settings;
using DataService.Web.Services;
using DataService.Web.ViewModel.Home;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DataService.Web.Controllers
{
    public class DevicesController : Controller
    {
        private readonly IDeviceService _deviceService;
        public DevicesController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        // GET: DevicesController
        public ActionResult Index()
        {
            var model = new IndexViewModel()
            {
                Devices = _deviceService.GetDevices()
            };
            return View(model);
        }

        // GET: DevicesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DevicesController/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Create(Device device)
        {
            return Json(_deviceService.Add(device));
        }

        // GET: DevicesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DevicesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: DevicesController/Delete/5
        [HttpGet]
        public ActionResult Delete([FromQuery]string ip)
        {
            return Json(_deviceService.Delete(ip));

        }
    }
}
