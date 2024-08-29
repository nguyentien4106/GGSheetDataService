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
        AppSettingsService _settingsServices;
        SDKHelper _sdk;
        public DevicesController()
        {
            _settingsServices = new AppSettingsService("C:\\appsettings.json");
            _sdk = new SDKHelper();
        }

        // GET: DevicesController
        public ActionResult Index()
        {
            var model = new IndexViewModel()
            {
                Devices = _settingsServices.GetCurrentDevicesSettings()
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
            var result = _sdk.sta_ConnectTCP(device, true);

            
            return Json(result);
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

        // GET: DevicesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DevicesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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
    }
}
