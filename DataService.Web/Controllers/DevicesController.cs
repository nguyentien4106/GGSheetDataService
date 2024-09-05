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
        public ActionResult Create(Device device)
        {
            var result = _deviceService.Add(device);
            if (result.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }
            return Json(_deviceService.Add(device));
        }

        // GET: DevicesController/Edit/5
        public ActionResult Edit([FromQuery] string ip)
        {
            var device = _deviceService.GetDevices().FirstOrDefault(item => item.IP == ip);
            return View(device);
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
            var result = _deviceService.Delete(ip);

            if(result.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }

            return Json(result);
        }
    }
}
