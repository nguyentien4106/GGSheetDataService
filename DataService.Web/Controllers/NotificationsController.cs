using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataService.Infrastructure.Data;
using DataService.Infrastructure.Entities;
using DataService.Core.Contracts;
using DataWorkerService.Models;
using System.Net;

namespace DataService.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IGenericRepository<Notification> _repository;

        public NotificationsController(IGenericRepository<Notification> repos)
        {
            _repository = repos;
        }

        // GET: api/Notifications
        [HttpGet]
        public async Task<Result> GetNotifications()
        {
            var data = await _repository.GetAsync();
            return new()
            {
                Data = data,
                Code = (int)HttpStatusCode.OK
            };
        }

        // GET: api/Notifications/5
        [HttpGet("{id}")]
        public async Task<Notification> GetNotification(int id)
        {
            var notification = await _repository.GetById(id);
            
            return notification;
        }


        // POST: api/Notifications
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<Result> PostNotification(Notification notification)
        {
            var result = await _repository.Insert(notification);
            result.Data = notification;
            
            return result;
        }

    }
}
