using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataService.Application.Services;
using DataService.Core.Contracts;
using DataService.Core.Models.AttMachine;
using DataService.Infrastructure.Data;
using DataService.Infrastructure.Entities;
using DataService.Web.Models.Att;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DataService.Web.Controllers
{
    public class AttendancesController : Controller
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IGenericRepository<Employee> _employeeRepository;
        private readonly IDeviceService _deviceRepository;
        public AttendancesController(IAttendanceService attendanceService, IGenericRepository<Employee> employeeRep, IDeviceService deviceService)
        {
            _attendanceService = attendanceService;
            _employeeRepository = employeeRep;
            _deviceRepository = deviceService;
        }

        // GET: Attendances
        public async Task<IActionResult> Index()
        {
            var attendances = await _attendanceService.GetAsync();
            return View(attendances.Select(i => new AttendanceViewModel(i)));
        }

        // GET: Attendances/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _attendanceService.GetById(id.Value);
            if (attendance == null)
            {
                return NotFound();
            }

            return View(attendance);
        }

        // GET: Attendances/Create
        public IActionResult Create()
        {
            ViewData["Employees"] = _employeeRepository.Get()
                .Select(item => new SelectListItem() { Value = item.Id.ToString(), Text = item.Name });
            ViewData["Devices"] = _deviceRepository.Get()
                .Select(item => new SelectListItem() { Value = item.Id.ToString(), Text = item.Ip });

            ViewData["VerifyTypes"] = VerifyMethod.VerifyMethods
                .Select(item => new SelectListItem() { Value = item.Key.ToString(), Text = item.Value });

            ViewData["VerifyStates"] = AttState.VeriryStates
                .Select(item => new SelectListItem() { Value = item.Key.ToString(), Text = item.Value });

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Attendance attendance)
        {
            var result = await _attendanceService.Insert(attendance);
            if (result.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", result.Message);
            return View(attendance);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _attendanceService.GetById(id.Value);
            if (attendance == null)
            {
                return NotFound();
            }
            return View(attendance);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,VerifyDate,VerifyType,VerifyState,WorkCode,Id")] Attendance attendance)
        {
            if (id != attendance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _attendanceService.Update(attendance);
                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(attendance);
        }

        // GET: Attendances/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _attendanceService.GetById(id.Value);
            if (attendance == null)
            {
                return NotFound();
            }

            return View(attendance);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _attendanceService.Delete(id);

            if(result.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("delete", result.Message);
            return View();
        }

    }
}
