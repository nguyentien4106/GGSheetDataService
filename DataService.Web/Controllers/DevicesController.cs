using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataService.Application.Services;
using DataService.Infrastructure.Data;
using DataService.Infrastructure.Entities;

namespace DataService.Web.Controllers
{
    public class DevicesController(IDeviceService service) : Controller
    {
        private readonly AppDbContext _context;
        private readonly IDeviceService _service = service;

        // GET: Devices
        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAsync());
        }

        // GET: Devices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _service.GetById(id.Value, "Sheets");
            
            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        // GET: Devices/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Device device)
        {
            var result = await _service.Insert(device);

            return Json(result);
        }

        // GET: Devices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _service.GetById(id.Value, "Sheets");
            if (device == null)
            {
                return NotFound();
            }
            return View(device);
        }

        
        [HttpPost]
        public async Task<IActionResult> Edit(Device device)
        {
            var result = await _service.Update(device);
            return Json(result);
        }

        // POST: Devices/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _service.Delete(id);
            if (result.IsSuccess)
            {
                result.Message = Url.Action(nameof(Index), "Devices");
            }
            return Json(result);
        }

    }
}
